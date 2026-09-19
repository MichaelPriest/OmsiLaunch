using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using OmsiLaunch.Api;

namespace OmsiLaunch.Process;

public sealed record RuntimeArtifact(string SourcePath, string DestinationRelativePath, string Sha256, long Size);

public sealed class RuntimeArtifactSet
{
    public IReadOnlyList<RuntimeArtifact> Artifacts { get; }
    private RuntimeArtifactSet(IReadOnlyList<RuntimeArtifact> artifacts) => Artifacts = artifacts;

    public static RuntimeArtifactSet Load(string pluginBuildDirectory, string nativeBuildPath)
    {
        if (!Directory.Exists(pluginBuildDirectory)) throw new DirectoryNotFoundException(pluginBuildDirectory);
        // CopyLocalLockFileAssemblies places the managed closure beside the
        // plugin. Stage only our named product assemblies plus DNNE metadata;
        // never sweep arbitrary DLLs from a build directory into OMSI.
        var required = new[] { "OmsiLaunch.Plugin.opl", "OmsiLaunch.PluginNE.dll", "OmsiLaunch.Plugin.deps.json", "OmsiLaunch.Plugin.runtimeconfig.json" }
            .Select(file => Path.Combine(pluginBuildDirectory, file));
        var managedClosure = Directory.EnumerateFiles(pluginBuildDirectory, "OmsiLaunch.*.dll", SearchOption.TopDirectoryOnly)
            .Where(path => !string.Equals(Path.GetFileName(path), "OmsiLaunch.PluginNE.dll", StringComparison.OrdinalIgnoreCase));
        var sources = required.Concat(managedClosure).Append(nativeBuildPath).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var artifacts = sources.Select(source =>
        {
            if (!File.Exists(source)) throw new FileNotFoundException("Required OmsiLaunch runtime artifact is missing.", source);
            var info = new FileInfo(source); return new RuntimeArtifact(source, Path.Combine("plugins", info.Name), Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(source))), info.Length);
        }).ToArray();
        if (!artifacts.Any(x => string.Equals(Path.GetFileName(x.SourcePath), "OmsiLaunch.Plugin.dll", StringComparison.OrdinalIgnoreCase)))
            throw new FileNotFoundException("The plugin assembly is not present in the managed dependency closure.", Path.Combine(pluginBuildDirectory, "OmsiLaunch.Plugin.dll"));
        return new RuntimeArtifactSet(artifacts);
    }

    // These are permanent product files under plugins\. A session validates
    // them, but never stages, snapshots, restores, or removes them.
    public void ValidateInstalled(string installationRoot)
    {
        foreach (var artifact in Artifacts)
        {
            var destination = Path.Combine(installationRoot, artifact.DestinationRelativePath);
            if (!File.Exists(destination)) throw new FileNotFoundException("OL_E_PERMANENT_PLUGIN_MISSING", destination);
            var hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(destination)));
            if (!string.Equals(hash, artifact.Sha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("OL_E_PERMANENT_PLUGIN_HASH_MISMATCH: " + artifact.DestinationRelativePath);
        }
    }
}

public sealed class CurrentStartupHandoffStore : IDisposable
{
    private readonly MemoryMappedFile mapping;
    public string Name { get; }
    public Guid SessionId { get; }
    private CurrentStartupHandoffStore(MemoryMappedFile mapping, string name, Guid sessionId) { this.mapping = mapping; Name = name; SessionId = sessionId; }

    public static CurrentStartupHandoffStore Create(StartupHandoff handoff)
    {
        var bytes = StartupHandoffWire.Serialize(handoff); var name = "OmsiLaunch.Handoff." + handoff.SessionId.ToString("N");
        var mapping = MemoryMappedFile.CreateNew(name, bytes.Length, MemoryMappedFileAccess.ReadWrite);
        using var view = mapping.CreateViewAccessor(0, bytes.Length, MemoryMappedFileAccess.Write); view.WriteArray(0, bytes, 0, bytes.Length); view.Flush();
        return new CurrentStartupHandoffStore(mapping, name, handoff.SessionId);
    }
    public void Dispose() => mapping.Dispose();
}

public sealed class CurrentTelemetryStore : IDisposable
{
    private const int Capacity = 4096;
    private readonly MemoryMappedFile mapping;
    public string Name { get; }
    private CurrentTelemetryStore(MemoryMappedFile mapping, string name) { this.mapping = mapping; Name = name; }
    public static CurrentTelemetryStore Create(Guid sessionId)
    {
        var name = "OmsiLaunch.Telemetry." + sessionId.ToString("N");
        return new CurrentTelemetryStore(MemoryMappedFile.CreateNew(name, Capacity, MemoryMappedFileAccess.ReadWrite), name);
    }
    public string? ReadLatest()
    {
        using var view = mapping.CreateViewAccessor(0, Capacity, MemoryMappedFileAccess.Read);
        var length = view.ReadInt32(0);
        if (length <= 0 || length > Capacity - 4) return null;
        var data = new byte[length]; view.ReadArray(4, data, 0, length);
        // A producer commits by writing length last. Ignore a torn sample and
        // let the supervisor consume the next complete session-bound event.
        if (view.ReadInt32(0) != length) return null;
        return System.Text.Encoding.UTF8.GetString(data);
    }
    public void Dispose() => mapping.Dispose();
}

// Single-flight session mailbox. It has no global discovery, binds every
// request to the handoff session, and is disposed with the session transaction.
public sealed class CurrentRuntimeCommandStore : IDisposable
{
    private const int Capacity = 65_536;
    private const int StateOffset = 0;
    private const int LengthOffset = 4;
    private const int DataOffset = 8;
    private readonly MemoryMappedFile mapping;
    private readonly SemaphoreSlim gate = new(1, 1);
    public string Name { get; }
    public Guid SessionId { get; }
    private CurrentRuntimeCommandStore(MemoryMappedFile mapping, string name, Guid sessionId) { this.mapping = mapping; Name = name; SessionId = sessionId; }
    public static CurrentRuntimeCommandStore Create(Guid sessionId)
    {
        var name = "OmsiLaunch.Runtime." + sessionId.ToString("N");
        return new CurrentRuntimeCommandStore(MemoryMappedFile.CreateNew(name, Capacity, MemoryMappedFileAccess.ReadWrite), name, sessionId);
    }
    public async Task<RuntimeCommandResult> RequestAsync(RuntimeCommand command, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (command.SessionId != SessionId) throw new InvalidOperationException("OL_E_RUNTIME_SESSION_MISMATCH");
        var request = RuntimeCommandWire.SerializeRequest(command); if (request.Length > Capacity - DataOffset) throw new ArgumentOutOfRangeException(nameof(command));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            using var view = mapping.CreateViewAccessor(0, Capacity, MemoryMappedFileAccess.ReadWrite);
            if (view.ReadInt32(StateOffset) != 0) throw new InvalidOperationException("OL_E_RUNTIME_CHANNEL_BUSY");
            view.Write(LengthOffset, request.Length); view.WriteArray(DataOffset, request, 0, request.Length); view.Write(StateOffset, 1); view.Flush();
            var deadline = DateTimeOffset.UtcNow + timeout;
            while (view.ReadInt32(StateOffset) != 2)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTimeOffset.UtcNow >= deadline) { view.Write(StateOffset, 0); view.Flush(); throw new TimeoutException("OL_E_RUNTIME_REQUEST_TIMEOUT"); }
                await Task.Delay(20, cancellationToken).ConfigureAwait(false);
            }
            var length = view.ReadInt32(LengthOffset); if (length <= 0 || length > Capacity - DataOffset) throw new InvalidDataException("OL_E_RUNTIME_RESPONSE_INVALID");
            var response = new byte[length]; view.ReadArray(DataOffset, response, 0, length); view.Write(StateOffset, 0); view.Flush();
            if (!RuntimeCommandWire.TryDeserializeResponse(response, out var result) || result is null || result.SessionId != SessionId || result.RequestId != command.RequestId) throw new InvalidDataException("OL_E_RUNTIME_RESPONSE_INVALID");
            return result;
        }
        finally { gate.Release(); }
    }
    public void Dispose() { gate.Dispose(); mapping.Dispose(); }
}
