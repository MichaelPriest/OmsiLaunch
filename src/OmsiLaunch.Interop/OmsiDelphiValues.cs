// Derived in concept from OmsiHook Memory.cs and MemArray/* at upstream commit
// 7687b6623f5f74b4419695257bd2a4eef54dd93e (LGPL-3.0-only).
// No upstream OMSI address, RPC transport, or process-attach behavior is copied.
using System.Runtime.CompilerServices;
using System.Text;

namespace OmsiLaunch.Interop;

public enum OmsiStringEncoding
{
    Ansi,
    Unicode,
}

public readonly record struct OmsiRemoteAddress(uint Value)
{
    public bool IsNull => Value == 0;
    public override string ToString() => $"0x{Value:X8}";
}

public static class OmsiDelphiValues
{
    private const int DynamicArrayHeaderSize = 8;
    private const int LongStringHeaderSize = 12;
    private static readonly Encoding Ansi1252 = CreateAnsi1252();

    public static async ValueTask<string?> ReadStringAsync(this IOmsiMemory memory, uint pointerAddress, OmsiStringEncoding encoding, bool pointerIsData = false, int maximumCharacters = 16 * 1024, CancellationToken cancellationToken = default)
    {
        var dataAddress = pointerIsData ? pointerAddress : await memory.ReadPointer32Async(pointerAddress, cancellationToken).ConfigureAwait(false);
        if (dataAddress == 0) return null;

        var length = await memory.ReadDelphiLengthAsync(dataAddress, cancellationToken).ConfigureAwait(false);
        if (length < 0 || length > maximumCharacters) throw new InvalidDataException("Invalid Delphi string length.");

        var byteCount = encoding == OmsiStringEncoding.Unicode ? checked(length * sizeof(char)) : length;
        var bytes = new byte[byteCount];
        await memory.ReadExactAsync(dataAddress, bytes, cancellationToken).ConfigureAwait(false);
        return encoding == OmsiStringEncoding.Unicode
            ? Encoding.Unicode.GetString(bytes)
            : Ansi1252.GetString(bytes);
    }

    public static async ValueTask<string?[]> ReadStringArrayAsync(this IOmsiMemory memory, uint pointerAddress, OmsiStringEncoding encoding, bool pointerIsData = false, int maximumElements = 16 * 1024, CancellationToken cancellationToken = default)
    {
        var dataAddress = pointerIsData ? pointerAddress : await memory.ReadPointer32Async(pointerAddress, cancellationToken).ConfigureAwait(false);
        var pointers = await memory.ReadDelphiPointerArrayAsync(dataAddress, maximumElements, cancellationToken).ConfigureAwait(false);
        var result = new string?[pointers.Length];
        for (var index = 0; index < pointers.Length; index++)
            result[index] = await memory.ReadStringAsync(pointers[index], encoding, pointerIsData: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result;
    }

    public static async ValueTask<T[]> ReadStructArrayAsync<T>(this IOmsiMemory memory, uint pointerAddress, bool pointerIsData = false, int maximumElements = 16 * 1024, CancellationToken cancellationToken = default) where T : unmanaged
    {
        var dataAddress = pointerIsData ? pointerAddress : await memory.ReadPointer32Async(pointerAddress, cancellationToken).ConfigureAwait(false);
        if (dataAddress == 0) return Array.Empty<T>();
        var length = await memory.ReadDelphiLengthAsync(dataAddress, cancellationToken).ConfigureAwait(false);
        if (length < 0 || length > maximumElements) throw new InvalidDataException("Invalid Delphi dynamic-array length.");

        var result = new T[length];
        if (length == 0) return result;
        var bytes = new byte[checked(length * Unsafe.SizeOf<T>())];
        await memory.ReadExactAsync(dataAddress, bytes, cancellationToken).ConfigureAwait(false);
        bytes.AsSpan().CopyTo(System.Runtime.InteropServices.MemoryMarshal.AsBytes(result.AsSpan()));
        return result;
    }

    public static async ValueTask<OmsiRemoteAddress[]> ReadObjectArrayAsync(this IOmsiMemory memory, uint pointerAddress, bool pointerIsData = false, int maximumElements = 16 * 1024, CancellationToken cancellationToken = default)
    {
        var dataAddress = pointerIsData ? pointerAddress : await memory.ReadPointer32Async(pointerAddress, cancellationToken).ConfigureAwait(false);
        var values = await memory.ReadDelphiPointerArrayAsync(dataAddress, maximumElements, cancellationToken).ConfigureAwait(false);
        return Array.ConvertAll(values, value => new OmsiRemoteAddress(value));
    }

    // The returned address points to Delphi string data, not its header.
    // Callers must free HeaderAddress through the same allocator when ownership ends.
    public static async ValueTask<OmsiAllocatedString> AllocateStringAsync(this IOmsiMemory memory, IOmsiRemoteAllocator allocator, string value, OmsiStringEncoding encoding, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);
        var payload = encoding == OmsiStringEncoding.Unicode ? Encoding.Unicode.GetBytes(value) : Ansi1252.GetBytes(value);
        var header = await allocator.AllocateAsync(checked((uint)(LongStringHeaderSize + payload.Length + 2)), cancellationToken).ConfigureAwait(false);
        if (header == 0) throw new InvalidOperationException("OMSI heap allocation returned null.");

        var metadata = new byte[LongStringHeaderSize];
        BitConverter.GetBytes((short)1252).CopyTo(metadata, 0);
        BitConverter.GetBytes((short)(encoding == OmsiStringEncoding.Unicode ? 2 : 1)).CopyTo(metadata, 2);
        BitConverter.GetBytes(1).CopyTo(metadata, 4);
        BitConverter.GetBytes(value.Length).CopyTo(metadata, 8);
        await memory.WriteAsync((nint)header, metadata, cancellationToken).ConfigureAwait(false);
        await memory.WriteAsync((nint)(header + LongStringHeaderSize), payload, cancellationToken).ConfigureAwait(false);
        return new OmsiAllocatedString(header, header + LongStringHeaderSize, encoding, value.Length);
    }

    public static async ValueTask<OmsiAllocatedArray<T>> AllocateStructArrayAsync<T>(this IOmsiMemory memory, IOmsiRemoteAllocator allocator, ReadOnlyMemory<T> values, CancellationToken cancellationToken = default) where T : unmanaged
    {
        var bytes = System.Runtime.InteropServices.MemoryMarshal.AsBytes(values.Span).ToArray();
        var header = await allocator.AllocateAsync(checked((uint)(DynamicArrayHeaderSize + bytes.Length)), cancellationToken).ConfigureAwait(false);
        if (header == 0) throw new InvalidOperationException("OMSI heap allocation returned null.");
        await memory.WriteValueAsync(header, 1, cancellationToken).ConfigureAwait(false);
        await memory.WriteValueAsync(header + sizeof(int), values.Length, cancellationToken).ConfigureAwait(false);
        if (bytes.Length > 0) await memory.WriteAsync((nint)(header + DynamicArrayHeaderSize), bytes, cancellationToken).ConfigureAwait(false);
        return new OmsiAllocatedArray<T>(header, header + DynamicArrayHeaderSize, values.Length);
    }

    private static Encoding CreateAnsi1252()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(1252);
    }
}

public readonly record struct OmsiAllocatedString(uint HeaderAddress, uint DataAddress, OmsiStringEncoding Encoding, int Length);
public readonly record struct OmsiAllocatedArray<T>(uint HeaderAddress, uint DataAddress, int Length) where T : unmanaged;
