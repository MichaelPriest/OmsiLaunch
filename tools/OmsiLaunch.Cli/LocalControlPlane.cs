using System.IO.Pipes;
using System.Text.Json;
using OmsiLaunch.Api;

internal sealed record LocalControlRequest(string ProtocolVersion, string Command, IReadOnlyDictionary<string, string>? Arguments = null);
internal sealed record LocalControlResponse(bool Ok, object? Result = null, string? ErrorCode = null, string? Message = null);

// A local, current-user-only command pipe. This is deliberately distinct from
// the host-to-plugin runtime mailbox and exposes only semantic session actions.
internal sealed class LocalControlPlane : IAsyncDisposable
{
    private const string PipeName = "OmsiLaunch.Control.0.1";
    private const int MaxMessageBytes = 65_536;
    private readonly Func<LocalControlRequest, Task<LocalControlResponse>> handler;
    private readonly CancellationTokenSource stop = new();
    private Task? loop;

    public LocalControlPlane(Func<LocalControlRequest, Task<LocalControlResponse>> handler) => this.handler = handler;
    public void Start() => loop = Task.Run(ListenAsync);

    public static async Task<LocalControlResponse?> TryRequestAsync(LocalControlRequest request, TimeSpan timeout)
    {
        try
        {
            using var cancellation = new CancellationTokenSource(timeout);
            await using var pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
            await pipe.ConnectAsync(cancellation.Token).ConfigureAwait(false);
            await WriteAsync(pipe, request, cancellation.Token).ConfigureAwait(false);
            return await ReadAsync<LocalControlResponse>(pipe, cancellation.Token).ConfigureAwait(false);
        }
        catch (TimeoutException) { return null; }
        catch (OperationCanceledException) { return null; }
        catch (IOException) { return null; }
    }

    private async Task ListenAsync()
    {
        while (!stop.IsCancellationRequested)
        {
            await using var pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
            try
            {
                await pipe.WaitForConnectionAsync(stop.Token).ConfigureAwait(false);
                var request = await ReadAsync<LocalControlRequest>(pipe, stop.Token).ConfigureAwait(false);
                var response = request is null || request.ProtocolVersion != PublicCapabilityRegistry.ProtocolVersion
                    ? new LocalControlResponse(false, ErrorCode: "OL_E_CONTROL_PROTOCOL", Message: "Unsupported local control protocol.")
                    : await handler(request).ConfigureAwait(false);
                await WriteAsync(pipe, response, stop.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        }
    }

    private static async Task WriteAsync<T>(Stream stream, T value, CancellationToken cancellationToken)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        if (bytes.Length > MaxMessageBytes) throw new InvalidDataException("OL_E_CONTROL_MESSAGE_TOO_LARGE");
        await stream.WriteAsync(BitConverter.GetBytes(bytes.Length), cancellationToken).ConfigureAwait(false);
        await stream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task<T?> ReadAsync<T>(Stream stream, CancellationToken cancellationToken)
    {
        var lengthBytes = new byte[sizeof(int)];
        if (!await ReadExactlyAsync(stream, lengthBytes, cancellationToken).ConfigureAwait(false)) return default;
        var length = BitConverter.ToInt32(lengthBytes, 0);
        if (length < 0 || length > MaxMessageBytes) throw new InvalidDataException("OL_E_CONTROL_MESSAGE_INVALID");
        var bytes = new byte[length];
        if (!await ReadExactlyAsync(stream, bytes, cancellationToken).ConfigureAwait(false)) return default;
        return JsonSerializer.Deserialize<T>(bytes);
    }

    private static async Task<bool> ReadExactlyAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
    {
        var read = 0;
        while (read < buffer.Length)
        {
            var count = await stream.ReadAsync(buffer.AsMemory(read), cancellationToken).ConfigureAwait(false);
            if (count == 0) return false;
            read += count;
        }
        return true;
    }

    public async ValueTask DisposeAsync()
    {
        stop.Cancel();
        if (loop is not null) await loop.ConfigureAwait(false);
        stop.Dispose();
    }
}
