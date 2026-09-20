using System.IO.Compression;
using System.Text;

namespace OmsiLaunch.Gui;

internal sealed record InstallationHealthItem(string Area, string Status, string Detail)
{
    public string Display => $"{Area} · {Status} · {Detail}";
}

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
}
