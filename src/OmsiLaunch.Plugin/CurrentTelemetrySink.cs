using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.Json;

namespace OmsiLaunch.Plugin;

internal static class CurrentTelemetrySink
{
    private const int Capacity = 4096;
    // Plugin callbacks can emit on more than one OMSI callback path. Serialize
    // the length/payload commit so a later event cannot overwrite an earlier one mid-read.
    private static readonly object gate = new();
    public static void Emit(string name, IReadOnlyDictionary<string, string> data)
    {
        var mappingName = Environment.GetEnvironmentVariable("OMSILAUNCH_TELEMETRY_NAME");
        if (string.IsNullOrWhiteSpace(mappingName)) return;
        var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { name, data }));
        if (payload.Length > Capacity - 4) return;
        try
        {
            lock (gate)
            {
                using var mapping = MemoryMappedFile.OpenExisting(mappingName, MemoryMappedFileRights.ReadWrite);
                using var view = mapping.CreateViewAccessor(0, Capacity, MemoryMappedFileAccess.ReadWrite);
                view.Write(0, 0); view.Flush();
                view.WriteArray(4, payload, 0, payload.Length); view.Flush();
                view.Write(0, payload.Length); view.Flush();
            }
        }
        catch (FileNotFoundException) { }
        catch (UnauthorizedAccessException) { }
    }
}
