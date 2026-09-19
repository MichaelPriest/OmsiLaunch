using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using OmsiLaunch.Api;

namespace OmsiLaunch.Process;

public sealed record ProcessIdentity(int ProcessId, DateTimeOffset CreationTimeUtc, string ExecutablePath, string ExecutableSha256);
public sealed record StartupProcessRequest(string ExecutablePath, string WorkingDirectory, IReadOnlyDictionary<string, string> Environment);

// Owns CreateProcessW handles; PID is diagnostic identity, never ownership.
public sealed class LaunchedProcess : IDisposable
{
    internal LaunchedProcess(int pid, int tid, SafeWin32Handle process, SafeWin32Handle thread, ProcessIdentity identity) { ProcessId = pid; ThreadId = tid; ProcessHandle = process; ThreadHandle = thread; Identity = identity; }
    public int ProcessId { get; } public int ThreadId { get; } public ProcessIdentity Identity { get; }
    internal SafeWin32Handle ProcessHandle { get; } internal SafeWin32Handle ThreadHandle { get; }
    public void Dispose() { ThreadHandle.Dispose(); ProcessHandle.Dispose(); }
}

public interface IRuntimePlatform
{
    RuntimePlatformInfo Detect(string installationRoot); void ValidateCurrent(RuntimePlatformInfo platform); bool IsInstallationWritable(string installationRoot);
    Task<LaunchedProcess> StartAsync(StartupProcessRequest request, string executableSha256, CancellationToken cancellationToken);
    Task WaitForExitAsync(LaunchedProcess process, CancellationToken cancellationToken); void Terminate(LaunchedProcess process); bool HasExited(LaunchedProcess process);
}

public sealed class CurrentWindowsX64Platform : IRuntimePlatform
{
    private const uint CreateUnicodeEnvironment = 0x400, WaitObject0 = 0, WaitTimeout = 258;
    public RuntimePlatformInfo Detect(string root)
    {
        var os = RuntimeInformation.OSArchitecture.ToString(); var host = RuntimeInformation.ProcessArchitecture.ToString(); var ok = OperatingSystem.IsWindows() && RuntimeInformation.OSArchitecture == Architecture.X64 && RuntimeInformation.ProcessArchitecture == Architecture.X64 && Environment.OSVersion.Version.Major >= 10;
        return new RuntimePlatformInfo(OperatingSystem.IsWindows() ? "Windows" : RuntimeInformation.OSDescription, Environment.OSVersion.Version.ToString(), os, host, "X86", "X86", ok, false, Environment.Is64BitOperatingSystem, IsInstallationWritable(root), ok, ok, ok, ok, ok);
    }
    public void ValidateCurrent(RuntimePlatformInfo p) { if (p.OsFamily != "Windows") throw new PlatformNotSupportedException("OL_E_UNSUPPORTED_OPERATING_SYSTEM"); if (p.OsArchitecture != "X64" || p.HostArchitecture != "X64") throw new PlatformNotSupportedException("OL_E_UNSUPPORTED_OS_ARCHITECTURE"); if (!p.CurrentPlatformSupported) throw new PlatformNotSupportedException("OL_E_PLATFORM_CAPABILITY_MISSING"); if (!p.InstallationWritable) throw new UnauthorizedAccessException("OL_E_INSTALLATION_NOT_WRITABLE"); }
    public bool IsInstallationWritable(string root) { try { var d = new DirectoryInfo(root); return d.Exists && !d.Attributes.HasFlag(FileAttributes.ReadOnly) && Directory.Exists(Path.Combine(root, "plugins")); } catch (Exception e) when (e is IOException or UnauthorizedAccessException) { return false; } }
    public Task<LaunchedProcess> StartAsync(StartupProcessRequest request, string executableSha256, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); var start = new StartupInfo { cb = Marshal.SizeOf<StartupInfo>() }; var env = BuildEnvironment(request.Environment); var command = Marshal.StringToHGlobalUni("\"" + request.ExecutablePath + "\"");
        try
        {
            if (!CreateProcessW(request.ExecutablePath, command, IntPtr.Zero, IntPtr.Zero, false, CreateUnicodeEnvironment, env, request.WorkingDirectory, ref start, out var raw)) throw new InvalidOperationException("OL_E_PROCESS_START_FAILED: Win32=" + Marshal.GetLastWin32Error());
            var process = new SafeWin32Handle(raw.hProcess, true); var thread = new SafeWin32Handle(raw.hThread, true);
            try { if (!GetProcessTimes(process, out var creation, out _, out _, out _)) throw new InvalidOperationException("OL_E_PROCESS_CREATION_TIME_FAILED: Win32=" + Marshal.GetLastWin32Error()); var ticks = ((long)creation.HighDateTime << 32) | creation.LowDateTime; var identity = new ProcessIdentity((int)raw.pid, new DateTimeOffset(DateTime.FromFileTimeUtc(ticks)), request.ExecutablePath, executableSha256); return Task.FromResult(new LaunchedProcess((int)raw.pid, (int)raw.tid, process, thread, identity)); } catch { process.Dispose(); thread.Dispose(); throw; }
        }
        finally { Marshal.FreeHGlobal(command); Marshal.FreeHGlobal(env); }
    }
    public async Task WaitForExitAsync(LaunchedProcess process, CancellationToken token) { while (true) { token.ThrowIfCancellationRequested(); var result = WaitForSingleObject(process.ProcessHandle, 100); if (result == WaitObject0) return; if (result != WaitTimeout) throw new InvalidOperationException("OL_E_PROCESS_WAIT_FAILED: Win32=" + Marshal.GetLastWin32Error()); await Task.Delay(25, token).ConfigureAwait(false); } }
    public void Terminate(LaunchedProcess p) { if (!HasExited(p) && !TerminateProcess(p.ProcessHandle, 1)) throw new InvalidOperationException("OL_E_PROCESS_TERMINATE_FAILED: Win32=" + Marshal.GetLastWin32Error()); }
    public bool HasExited(LaunchedProcess p) => WaitForSingleObject(p.ProcessHandle, 0) == WaitObject0;
    private static IntPtr BuildEnvironment(IReadOnlyDictionary<string, string> changes) { var all = Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>().ToDictionary(e => (string)e.Key, e => (string?)e.Value ?? "", StringComparer.OrdinalIgnoreCase); foreach (var p in changes) all[p.Key] = p.Value; return Marshal.StringToHGlobalUni(string.Join('\0', all.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase).Select(p => p.Key + "=" + p.Value)) + "\0\0"); }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)] private struct StartupInfo { public int cb; public IntPtr reserved, desktop, title; public int x, y, xSize, ySize, xChars, yChars, fill, flags; public short show, reserved2; public IntPtr reserved3, stdin, stdout, stderr; }
    [StructLayout(LayoutKind.Sequential)] private struct ProcessInformation { public IntPtr hProcess, hThread; public uint pid, tid; }
    [StructLayout(LayoutKind.Sequential)] private struct FileTime { public uint LowDateTime, HighDateTime; }
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern bool CreateProcessW(string app, IntPtr command, IntPtr pa, IntPtr ta, bool inherit, uint flags, IntPtr environment, string cwd, ref StartupInfo start, out ProcessInformation info);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool GetProcessTimes(SafeWin32Handle handle, out FileTime creation, out FileTime exit, out FileTime kernel, out FileTime user);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern uint WaitForSingleObject(SafeWin32Handle handle, uint ms);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool TerminateProcess(SafeWin32Handle handle, uint code);
}

internal sealed class SafeWin32Handle : SafeHandleZeroOrMinusOneIsInvalid
{
    public SafeWin32Handle() : base(true) { } public SafeWin32Handle(IntPtr value, bool owns) : base(owns) => SetHandle(value);
    protected override bool ReleaseHandle() => CloseHandle(handle);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool CloseHandle(IntPtr handle);
}
