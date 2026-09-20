using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using OmsiLaunch.Configuration;

namespace OmsiLaunch.Gui;

internal sealed record InstallationHealthItem(string Area, string Status, string Detail)
{
    public string Display => $"{Area} · {Status} · {Detail}";
}

internal sealed record UninstallResult(int RemovedFiles, string? PreservedDataPath, bool DeferredSelfRemoval);

internal static class HubServices
{
    private static readonly string[] RequiredPluginFiles =
    {
        "OmsiLaunch.Plugin.opl",
        "OmsiLaunch.PluginNE.dll",
        "OmsiLaunch.Plugin.dll",
        "OmsiLaunch.Plugin.deps.json",
        "OmsiLaunch.Plugin.runtimeconfig.json",
        "OmsiLaunch.Native.x86.dll"
    };

    public static IReadOnlyList<InstallationHealthItem> ScanInstallation(string root)
    {
        var items = new List<InstallationHealthItem>();
        var omsiExe = Path.Combine(root, "Omsi.exe");
        var pluginDirectory = Path.Combine(root, "plugins");
        var privateRuntime = Path.Combine(root, ".omsilaunch", "runtime", "win-x86", "dotnet.exe");
        var log = Path.Combine(root, "logfile.txt");

        items.Add(File.Exists(omsiExe)
            ? new("Omsi.exe", "OK", "Executável encontrado.")
            : new("Omsi.exe", "ERRO", "Executável não encontrado."));

        items.Add(Directory.Exists(pluginDirectory)
            ? new("Plugins", "OK", "Diretório plugins encontrado.")
            : new("Plugins", "ERRO", "Diretório plugins ausente."));

        foreach (var file in RequiredPluginFiles)
        {
            var path = Path.Combine(pluginDirectory, file);
            items.Add(File.Exists(path)
                ? new(file, "OK", new FileInfo(path).Length.ToString("N0") + " bytes")
                : new(file, "ERRO", "Arquivo obrigatório ausente."));
        }

        items.Add(File.Exists(privateRuntime)
            ? new(".NET x86 privado", "OK", "Runtime privado disponível para o plugin.")
            : new(".NET x86 privado", "AVISO", "Runtime privado não instalado na raiz selecionada."));

        items.Add(File.Exists(log)
            ? new("logfile.txt", "OK", "Log do OMSI disponível para diagnóstico.")
            : new("logfile.txt", "INFO", "O OMSI ainda não criou logfile.txt nesta instalação."));

        try
        {
            var probe = Path.Combine(root, ".omsilaunch", ".write-probe-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.GetDirectoryName(probe)!);
            File.WriteAllText(probe, "OmsiLaunch write probe");
            File.Delete(probe);
            items.Add(new("Permissão de escrita", "OK", "A instalação permite escrita transacional."));
        }
        catch (Exception exception)
        {
            items.Add(new("Permissão de escrita", "ERRO", exception.Message));
        }

        var diagnostics = Path.Combine(root, ".omsilaunch", "diagnostics");
        var diagnosticCount = Directory.Exists(diagnostics)
            ? Directory.EnumerateFiles(diagnostics, "*-host.log", SearchOption.TopDirectoryOnly).Count()
            : 0;
        items.Add(new("Diagnósticos OmsiLaunch", "INFO", $"{diagnosticCount} log(s) de sessão encontrado(s)."));

        return items;
    }

    public static string CreateSupportBundle(string root, string destinationZip)
    {
        var temp = Path.Combine(Path.GetTempPath(), "OmsiLaunch-Support-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temp);
        try
        {
            var summary = new StringBuilder();
            summary.AppendLine("OmsiLaunch support bundle");
            summary.AppendLine("GeneratedUtc=" + DateTimeOffset.UtcNow.ToString("O"));
            summary.AppendLine("InstallationRoot=" + root);
            summary.AppendLine("OS=" + Environment.OSVersion);
            summary.AppendLine("Is64BitOS=" + Environment.Is64BitOperatingSystem);
            summary.AppendLine("Is64BitProcess=" + Environment.Is64BitProcess);
            summary.AppendLine();
            summary.AppendLine("Health:");
            foreach (var item in ScanInstallation(root))
                summary.AppendLine(item.Display);
            File.WriteAllText(Path.Combine(temp, "summary.txt"), summary.ToString());

            var logfile = Path.Combine(root, "logfile.txt");
            if (File.Exists(logfile))
                File.Copy(logfile, Path.Combine(temp, "logfile.txt"), true);

            var plugins = Path.Combine(root, "plugins");
            if (Directory.Exists(plugins))
            {
                var inventory = Directory.EnumerateFiles(plugins, "*", SearchOption.TopDirectoryOnly)
                    .Select(path =>
                    {
                        var info = new FileInfo(path);
                        return $"{info.Name}\t{info.Length}\t{info.LastWriteTimeUtc:O}";
                    });
                File.WriteAllLines(Path.Combine(temp, "plugins-inventory.txt"), inventory);
            }

            var diagnostics = Path.Combine(root, ".omsilaunch", "diagnostics");
            if (Directory.Exists(diagnostics))
            {
                var destinationDiagnostics = Path.Combine(temp, "diagnostics");
                Directory.CreateDirectory(destinationDiagnostics);
                foreach (var source in Directory.EnumerateFiles(diagnostics, "*-host.log", SearchOption.TopDirectoryOnly)
                             .OrderByDescending(File.GetLastWriteTimeUtc)
                             .Take(10))
                    File.Copy(source, Path.Combine(destinationDiagnostics, Path.GetFileName(source)), true);
            }

            if (File.Exists(destinationZip)) File.Delete(destinationZip);
            ZipFile.CreateFromDirectory(temp, destinationZip, CompressionLevel.Optimal, false);
            return destinationZip;
        }
        finally
        {
            try { Directory.Delete(temp, true); } catch { }
        }
    }

    public static async Task<UninstallResult> UninstallFromOmsiAsync(string root, string packageRoot)
    {
        root = Path.GetFullPath(root);
        packageRoot = Path.GetFullPath(packageRoot);

        if (IsOmsiRunning(root))
            throw new InvalidOperationException("Feche o OMSI antes de desinstalar o OmsiLaunch.");

        var transaction = new FileConfigurationTransaction(root, new Dictionary<string, byte[]>());
        if (await transaction.HasPendingRecoveryAsync().ConfigureAwait(false))
            await transaction.RestorePendingAsync().ConfigureAwait(false);

        var preservedData = PreserveRecoveryData(root);
        var removed = 0;

        var plugins = Path.Combine(root, "plugins");
        if (Directory.Exists(plugins))
        {
            foreach (var file in Directory.EnumerateFiles(plugins, "OmsiLaunch.*", SearchOption.TopDirectoryOnly))
            {
                File.Delete(file);
                removed++;
            }
        }

        var productState = Path.Combine(root, ".omsilaunch");
        if (Directory.Exists(productState))
            Directory.Delete(productState, true);

        var sameRoot = string.Equals(
            packageRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
            root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
            StringComparison.OrdinalIgnoreCase);

        if (!sameRoot)
            return new UninstallResult(removed, preservedData, false);

        var deferredFiles = CollectVerifiedPackageFiles(root);
        foreach (var file in Directory.EnumerateFiles(root, "OmsiLaunch*", SearchOption.TopDirectoryOnly))
            deferredFiles.Add(file);
        deferredFiles.Add(Path.Combine(root, "release-manifest.json"));

        ScheduleDeferredDeletion(deferredFiles.Where(File.Exists).Distinct(StringComparer.OrdinalIgnoreCase));
        return new UninstallResult(removed, preservedData, true);
    }

    private static bool IsOmsiRunning(string root)
    {
        var expected = Path.Combine(root, "Omsi.exe");
        foreach (var process in Process.GetProcessesByName("Omsi"))
        {
            try
            {
                var path = process.MainModule?.FileName;
                if (!string.IsNullOrWhiteSpace(path)
                    && string.Equals(Path.GetFullPath(path), expected, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch
            {
                // A process from another account may deny module inspection.
            }
            finally
            {
                process.Dispose();
            }
        }
        return false;
    }

    private static string? PreserveRecoveryData(string root)
    {
        var productState = Path.Combine(root, ".omsilaunch");
        var candidates = new[]
        {
            Path.Combine(productState, "plugin-backup"),
            Path.Combine(productState, "diagnostics")
        };
        if (!candidates.Any(Directory.Exists)) return null;

        var destination = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OmsiLaunch",
            "uninstall-backups",
            DateTime.Now.ToString("yyyyMMdd-HHmmss"));
        Directory.CreateDirectory(destination);

        foreach (var source in candidates.Where(Directory.Exists))
            CopyDirectory(source, Path.Combine(destination, Path.GetFileName(source)));

        return destination;
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var directory in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(Path.Combine(destination, Path.GetRelativePath(source, directory)));
        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(destination, Path.GetRelativePath(source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target, true);
        }
    }

    private static HashSet<string> CollectVerifiedPackageFiles(string root)
    {
        var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var manifestPath = Path.Combine(root, "release-manifest.json");
        if (!File.Exists(manifestPath)) return files;

        using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
        if (!document.RootElement.TryGetProperty("files", out var entries) || entries.ValueKind != JsonValueKind.Array)
            return files;

        foreach (var entry in entries.EnumerateArray())
        {
            if (!entry.TryGetProperty("path", out var rawPath) || !entry.TryGetProperty("sha256", out var rawHash))
                continue;
            var relative = rawPath.GetString();
            var expectedHash = rawHash.GetString();
            if (string.IsNullOrWhiteSpace(relative) || string.IsNullOrWhiteSpace(expectedHash))
                continue;

            var normalized = relative.Replace('/', Path.DirectorySeparatorChar);
            var target = Path.GetFullPath(Path.Combine(root, normalized));
            if (!target.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                continue;
            if (!File.Exists(target)) continue;

            var actualHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(target)));
            if (actualHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
                files.Add(target);
        }

        return files;
    }

    private static void ScheduleDeferredDeletion(IEnumerable<string> files)
    {
        var targets = files.ToArray();
        if (targets.Length == 0) return;

        var script = Path.Combine(Path.GetTempPath(), "OmsiLaunch-uninstall-" + Guid.NewGuid().ToString("N") + ".cmd");
        var lines = new List<string>
        {
            "@echo off",
            "setlocal",
            ":wait",
            $"tasklist /FI \"PID eq {Environment.ProcessId}\" /FO CSV /NH | findstr /C:\"\\\"{Environment.ProcessId}\\\"\" >nul 2>&1",
            "if not errorlevel 1 (timeout /t 1 /nobreak >nul & goto wait)"
        };

        foreach (var file in targets)
            lines.Add($"del /f /q \"{file.Replace("\"", "\"\"")}\" >nul 2>&1");
        lines.Add("del /f /q \"%~f0\" >nul 2>&1");

        File.WriteAllLines(script, lines, Encoding.ASCII);
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c \"" + script + "\"",
            CreateNoWindow = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        });
    }

}
