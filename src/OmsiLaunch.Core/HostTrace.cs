using System.Diagnostics;

namespace OmsiLaunch.Core;

internal sealed class HostTrace
{
    private readonly string path; private readonly Stopwatch clock = Stopwatch.StartNew(); private readonly Guid sessionId; private readonly object gate = new();
    public HostTrace(string installationRoot, Guid sessionId) { this.sessionId = sessionId; var directory = System.IO.Path.Combine(installationRoot, ".omsilaunch", "diagnostics"); Directory.CreateDirectory(directory); path = System.IO.Path.Combine(directory, sessionId.ToString("N") + "-host.log"); Write("TRACE_CREATED"); }
    public void Write(string boundary, string? data = null) { lock (gate) File.AppendAllText(path, $"{DateTimeOffset.UtcNow:O}\t{clock.ElapsedMilliseconds}\t{sessionId:D}\t{Environment.CurrentManagedThreadId}\t{boundary}\t{data ?? ""}{Environment.NewLine}"); }
    public string Path => path;
}
