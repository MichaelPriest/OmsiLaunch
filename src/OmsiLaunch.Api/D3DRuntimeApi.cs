using System.Globalization;

namespace OmsiLaunch.Api;

public enum D3DDeviceState : byte { NotReady, Ready, Lost, Resetting, Stopping, Stopped }
public enum D3DTextureFormat : byte { A8R8G8B8, X8R8G8B8, R5G6B5, X1R5G5B5, A1R5G5B5, A4R4G4B4, A8, L8, A8L8 }
public enum D3DTextureResourceState : byte { Live, Released, Stale }

public sealed record D3DTextureHandle(string Value);
public sealed record D3DDeviceStatus(
    bool Available,
    D3DDeviceState State,
    uint Generation,
    uint LiveTextureCount,
    bool ResetHookInstalled,
    uint ExecutionThreadId,
    uint LastResetThreadId,
    int QueryInterfaceHResult,
    int CooperativeLevelHResult,
    uint OwnedDeviceReferences);
public sealed record D3DTextureDescription(
    D3DTextureHandle Handle,
    D3DTextureResourceState State,
    D3DDeviceState DeviceState,
    uint Generation,
    uint Width,
    uint Height,
    D3DTextureFormat Format,
    uint Levels,
    uint Level,
    uint LevelWidth,
    uint LevelHeight,
    int HResult,
    uint ExecutionThreadId);
public sealed record D3DTextureUpdate(uint Level, uint X, uint Y, uint Width, uint Height, ReadOnlyMemory<byte> Pixels);

public sealed class OmsiRuntimeException : Exception
{
    public OmsiRuntimeException(string code, string? detail = null) : base(detail is null ? code : $"{code}: {detail}") => Code = code;
    public string Code { get; }
}

public static class D3DRuntimeApi
{
    private static long nextRequestId = 30_000;

    public static async Task<D3DDeviceStatus> GetD3DStatusAsync(this IOmsiLaunch launch, SessionHandle session, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var values = await ExecuteAsync(launch, session, "d3d.status", null, timeout, cancellationToken).ConfigureAwait(false);
        return new(
            Bool(values, "available"), DeviceState(values["state"]), UInt(values, "generation"), UInt(values, "live_textures"),
            Bool(values, "reset_hook_installed"), UInt(values, "execution_thread_id"), UInt(values, "last_reset_thread_id"),
            HResult(values, "query_interface_hresult"), HResult(values, "cooperative_level_hresult"), UInt(values, "owned_device_references"));
    }

    public static async Task<D3DTextureDescription> CreateD3DTextureAsync(this IOmsiLaunch launch, SessionHandle session, uint width, uint height, D3DTextureFormat format, uint levels = 1, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var values = await ExecuteAsync(launch, session, "d3d.texture.create", new Dictionary<string, string>
        {
            ["width"] = width.ToString(CultureInfo.InvariantCulture), ["height"] = height.ToString(CultureInfo.InvariantCulture),
            ["format"] = format.ToString(), ["levels"] = levels.ToString(CultureInfo.InvariantCulture)
        }, timeout ?? TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
        return Texture(values);
    }

    public static async Task<D3DTextureDescription> DescribeD3DTextureAsync(this IOmsiLaunch launch, SessionHandle session, D3DTextureHandle handle, uint level = 0, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var values = await ExecuteAsync(launch, session, "d3d.texture.describe", new Dictionary<string, string>
        {
            ["handle"] = handle.Value, ["level"] = level.ToString(CultureInfo.InvariantCulture)
        }, timeout ?? TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
        return Texture(values);
    }

    public static async Task<D3DTextureDescription> UpdateD3DTextureAsync(this IOmsiLaunch launch, SessionHandle session, D3DTextureHandle handle, D3DTextureUpdate update, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var values = await ExecuteAsync(launch, session, "d3d.texture.update", new Dictionary<string, string>
        {
            ["handle"] = handle.Value, ["level"] = update.Level.ToString(CultureInfo.InvariantCulture),
            ["x"] = update.X.ToString(CultureInfo.InvariantCulture), ["y"] = update.Y.ToString(CultureInfo.InvariantCulture),
            ["width"] = update.Width.ToString(CultureInfo.InvariantCulture), ["height"] = update.Height.ToString(CultureInfo.InvariantCulture),
            ["pixels_base64"] = Convert.ToBase64String(update.Pixels.Span)
        }, timeout ?? TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
        return Texture(values);
    }

    public static async Task<D3DTextureDescription> ReleaseD3DTextureAsync(this IOmsiLaunch launch, SessionHandle session, D3DTextureHandle handle, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var values = await ExecuteAsync(launch, session, "d3d.texture.release", new Dictionary<string, string> { ["handle"] = handle.Value }, timeout ?? TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
        return Texture(values);
    }

    private static async Task<IReadOnlyDictionary<string, string>> ExecuteAsync(IOmsiLaunch launch, SessionHandle session, string operation, IReadOnlyDictionary<string, string>? arguments, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var request = unchecked((ulong)Interlocked.Increment(ref nextRequestId));
        var result = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, request, operation, arguments), timeout, cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded) throw new OmsiRuntimeException(result.ErrorCode ?? "OL_E_RUNTIME_OPERATION_FAILED", result.Values is not null && result.Values.TryGetValue("detail", out var detail) ? detail : null);
        return result.Values ?? throw new OmsiRuntimeException("OL_E_RUNTIME_PROTOCOL_MISMATCH", "D3D result contained no values.");
    }

    private static D3DTextureDescription Texture(IReadOnlyDictionary<string, string> values) => new(
        new(values["handle"]), Enum.Parse<D3DTextureResourceState>(values["state"], true), DeviceState(values["device_state"]),
        UInt(values, "generation"), UInt(values, "width"), UInt(values, "height"), Enum.Parse<D3DTextureFormat>(values["format"], true),
        UInt(values, "levels"), UInt(values, "level"), UInt(values, "level_width"), UInt(values, "level_height"), HResult(values, "hresult"), UInt(values, "execution_thread_id"));
    private static D3DDeviceState DeviceState(string value) => value switch { "NOT_READY" => D3DDeviceState.NotReady, "READY" => D3DDeviceState.Ready, "LOST" => D3DDeviceState.Lost, "RESETTING" => D3DDeviceState.Resetting, "STOPPING" => D3DDeviceState.Stopping, "STOPPED" => D3DDeviceState.Stopped, _ => throw new OmsiRuntimeException("OL_E_RUNTIME_PROTOCOL_MISMATCH", "Unknown D3D device state.") };
    private static bool Bool(IReadOnlyDictionary<string, string> values, string key) => bool.Parse(values[key]);
    private static uint UInt(IReadOnlyDictionary<string, string> values, string key) => uint.Parse(values[key], NumberStyles.None, CultureInfo.InvariantCulture);
    private static int HResult(IReadOnlyDictionary<string, string> values, string key) => unchecked((int)uint.Parse(values[key].AsSpan(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
}
