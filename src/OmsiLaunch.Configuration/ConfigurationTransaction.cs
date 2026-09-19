using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OmsiLaunch.Configuration;

public sealed record FileSnapshot(string Path, bool Existed, string Sha256, byte[] Bytes);
public sealed record JournalFile(string RelativePath, bool Existed, string Sha256, string BackupPath);
public enum TransactionState { Prepared, Applied, RuntimeDeployed, HandoffCreated, ProcessStarted, ProcessExited, Restoring, Restored, Completed }
public sealed record TransactionJournal(Guid SessionId, TransactionState State, IReadOnlyList<JournalFile> Files, int? ProcessId, long? ProcessStartFileTimeUtc, string? ExecutablePath);

public interface IConfigurationSnapshot { IReadOnlyList<FileSnapshot> Files { get; } }
public interface IConfigurationOverlay { IReadOnlyCollection<IConfigSemanticPatch> Patches { get; } }
public interface IConfigurationJournal { string SessionId { get; } bool RestorePending { get; } Task WriteAsync(IConfigurationSnapshot snapshot, CancellationToken cancellationToken = default); Task MarkRestoredAsync(CancellationToken cancellationToken = default); }
public interface IConfigurationRecovery { Task<bool> HasPendingRecoveryAsync(CancellationToken cancellationToken = default); Task RestorePendingAsync(CancellationToken cancellationToken = default); }
public interface IConfigurationTransaction : IAsyncDisposable { IReadOnlyList<FileSnapshot> Snapshots { get; } IConfigurationSnapshot Snapshot { get; } Task ApplyAsync(CancellationToken cancellationToken = default); Task RestoreAsync(CancellationToken cancellationToken = default); }
public interface IConfigSemanticPatch { string FileName { get; } byte[] Apply(byte[] source); }

public sealed class BracketTokenDocument
{
    private readonly byte[] original;
    private readonly Encoding encoding;
    private readonly string newline;
    private readonly List<string> lines;

    private BracketTokenDocument(byte[] original, Encoding encoding, string newline, List<string> lines) { this.original = original; this.encoding = encoding; this.newline = newline; this.lines = lines; }
    public static BracketTokenDocument Parse(byte[] bytes)
    {
        var encoding = DetectEncoding(bytes);
        var text = encoding.GetString(StripBom(bytes));
        return new BracketTokenDocument(bytes, encoding, text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n", text.Replace("\r\n", "\n").Split('\n').ToList());
    }
    public bool HasToken(string token) => FindToken(token) >= 0;
    public string? GetValue(string token) { var index = FindToken(token); return index >= 0 && index + 1 < lines.Count && !IsToken(lines[index + 1]) ? lines[index + 1] : null; }
    public IReadOnlyList<string> GetValues(string token, int count)
    {
        var index = FindToken(token);
        if (index < 0) return Array.Empty<string>();
        var values = new List<string>();
        for (var valueIndex = index + 1; valueIndex < lines.Count && values.Count < count && !IsToken(lines[valueIndex]); valueIndex++) values.Add(lines[valueIndex]);
        return values;
    }
    public void SetPresence(string token, bool present) { var index = FindToken(token); if (present && index < 0) lines.Add("[" + token + "]"); if (!present && index >= 0) RemoveTokenAt(index); }
    public void SetValue(string token, string value)
    {
        var index = FindToken(token);
        if (index < 0) { lines.Add("[" + token + "]"); lines.Add(value); return; }
        if (index + 1 < lines.Count && !IsToken(lines[index + 1])) lines[index + 1] = value; else lines.Insert(index + 1, value);
    }
    public void SetValues(string token, IReadOnlyList<string> values)
    {
        var index = FindToken(token);
        if (index < 0)
        {
            lines.Add("[" + token + "]");
            lines.AddRange(values);
            return;
        }

        var removeAt = index + 1;
        while (removeAt < lines.Count && !IsToken(lines[removeAt])) lines.RemoveAt(removeAt);
        lines.InsertRange(removeAt, values);
    }
    public byte[] Serialize(bool unchanged)
    {
        if (unchanged) return original;
        var text = string.Join(newline, lines);
        return WithBom(encoding, encoding.GetBytes(text));
    }
    private int FindToken(string token) => lines.FindIndex(x => string.Equals(x.Trim(), "[" + token + "]", StringComparison.OrdinalIgnoreCase));
    private void RemoveTokenAt(int index) { lines.RemoveAt(index); if (index < lines.Count && !IsToken(lines[index])) lines.RemoveAt(index); }
    private static bool IsToken(string value) => value.Trim().StartsWith("[", StringComparison.Ordinal) && value.Trim().EndsWith("]", StringComparison.Ordinal);
    private static Encoding DetectEncoding(byte[] bytes) => bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE ? Encoding.Unicode : bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? new UTF8Encoding(true) : Encoding.Default;
    private static byte[] StripBom(byte[] bytes) { var offset = bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE ? 2 : bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0; return bytes[offset..]; }
    private static byte[] WithBom(Encoding encoding, byte[] body) { var bom = encoding.GetPreamble(); return bom.Length == 0 ? body : bom.Concat(body).ToArray(); }
}

public sealed record TokenPatch(string FileName, string Token, string? Value, bool? Presence = null) : IConfigSemanticPatch
{
    public byte[] Apply(byte[] source)
    {
        var document = BracketTokenDocument.Parse(source);
        if (Presence is { } present) document.SetPresence(Token, present); else if (Value is not null) document.SetValue(Token, Value);
        return document.Serialize(false);
    }
}

public sealed class OptionsDocument
{
    private readonly BracketTokenDocument document;
    private bool changed;
    public OptionsDocument(byte[] bytes) => document = BracketTokenDocument.Parse(bytes);
    public void SetBoolean(string semanticName, bool value)
    {
        var (token, inverted) = semanticName switch {
            "collision_player_terrain" => ("no_collision_terrain", true), "collision_player_vehicle" => ("no_collision_vehToVeh", true),
            "collision_pedestrians" => ("no_collision_pedastrians", true), "reduced_multithreading_calculate" => ("no_multithreading_calculate", false),
            "reduced_multithreading_texload" => ("no_multithreading_texload", false), _ => (semanticName, false) };
        document.SetPresence(token, inverted ? !value : value); changed = true;
    }
    public void SetValue(string semanticName, string value) { document.SetValue(semanticName, value); changed = true; }
    public void SetAiMaxCountRandom(int roadTraffic, int humans)
    {
        var existing = (document.GetValue("AIMaxCountRandom") ?? "0 0 0 0 0 0 0 0 0").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).ToList();
        while (existing.Count < 9) existing.Add("0"); existing[0] = roadTraffic.ToString(System.Globalization.CultureInfo.InvariantCulture); existing[1] = humans.ToString(System.Globalization.CultureInfo.InvariantCulture);
        document.SetValue("AIMaxCountRandom", string.Join(" ", existing)); changed = true;
    }
    public void SetAiMaxCountRandomComponent(int index, int value)
    {
        if (index is < 0 or > 8) throw new ArgumentOutOfRangeException(nameof(index));
        var existing = (document.GetValue("AIMaxCountRandom") ?? "0 0 0 0 0 0 0 0 0").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).ToList();
        while (existing.Count < 9) existing.Add("0");
        existing[index] = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        document.SetValue("AIMaxCountRandom", string.Join(" ", existing)); changed = true;
    }
    public byte[] Serialize() => document.Serialize(!changed);
}
public sealed record KeyboardBinding(string OmsiEventId, int Key, bool Ctrl, bool Shift);
public sealed class KeyboardDocument
{
    private readonly byte[] original; private readonly Encoding encoding; private readonly string newline; private readonly List<string> lines; private bool changed;
    public KeyboardDocument(byte[] bytes)
    {
        original = bytes; encoding = bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE ? Encoding.Unicode : Encoding.Default;
        var offset = bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE ? 2 : 0; var text = encoding.GetString(bytes[offset..]); newline = text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n"; lines = text.Replace("\r\n", "\n").Split('\n').ToList();
    }
    public IReadOnlyList<KeyboardBinding> Bindings => ReadBindings().ToArray();
    public void SetBinding(KeyboardBinding binding)
    {
        var index = FindEntry(binding.OmsiEventId);
        var flags = (binding.Ctrl ? 4 : 0) | (binding.Shift ? 2 : 0);
        if (index < 0) { lines.Add("[entry]"); lines.Add(binding.OmsiEventId); lines.Add(binding.Key.ToString(System.Globalization.CultureInfo.InvariantCulture)); lines.Add(flags.ToString(System.Globalization.CultureInfo.InvariantCulture)); }
        else { lines[index + 2] = binding.Key.ToString(System.Globalization.CultureInfo.InvariantCulture); var prior = int.TryParse(lines[index + 3], out var value) ? value : 0; lines[index + 3] = ((prior & ~6) | flags).ToString(System.Globalization.CultureInfo.InvariantCulture); }
        changed = true;
    }
    public byte[] Serialize() { if (!changed) return original; var body = encoding.GetBytes(string.Join(newline, lines)); var bom = encoding.GetPreamble(); return bom.Length == 0 ? body : bom.Concat(body).ToArray(); }
    public byte[] SerializeNoOp() => original;
    private IEnumerable<KeyboardBinding> ReadBindings()
    {
        for (var index = 0; index + 3 < lines.Count; index++) if (string.Equals(lines[index].Trim(), "[entry]", StringComparison.OrdinalIgnoreCase) && int.TryParse(lines[index + 2], out var key) && int.TryParse(lines[index + 3], out var flags)) yield return new(lines[index + 1], key, (flags & 4) != 0, (flags & 2) != 0);
    }
    private int FindEntry(string eventId)
    {
        for (var index = 0; index + 3 < lines.Count; index++) if (string.Equals(lines[index].Trim(), "[entry]", StringComparison.OrdinalIgnoreCase) && string.Equals(lines[index + 1], eventId, StringComparison.OrdinalIgnoreCase)) return index;
        return -1;
    }
}
public sealed record ControllerDevice(string Name, bool Active, string ForceFeedbackScale, string ForceFeedbackCentering);
public sealed class GameControllerDocument
{
    private readonly byte[] original; private readonly Encoding encoding; private readonly string newline; private readonly List<string> lines; private bool changed;
    public GameControllerDocument(byte[] bytes)
    {
        original = bytes; encoding = bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE ? Encoding.Unicode : Encoding.Default;
        var offset = bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE ? 2 : 0; var text = encoding.GetString(bytes[offset..]); newline = text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n"; lines = text.Replace("\r\n", "\n").Split('\n').ToList();
    }
    public IReadOnlyList<ControllerDevice> Devices => ReadDevices().ToArray();
    public void SetActive(string name, bool active) { var ctrl = FindController(name); if (ctrl < 0) throw new KeyNotFoundException("Controller not found: " + name); lines[ctrl + 2] = active ? "1" : "0"; changed = true; }
    public void SetForceFeedback(string name, string scale, string centering)
    {
        var ctrl = FindController(name); if (ctrl < 0) throw new KeyNotFoundException("Controller not found: " + name); var ff = FindSection(ctrl, "[FFScale]"); if (ff < 0 || ff + 2 >= lines.Count) throw new InvalidDataException("Controller has no valid FFScale block: " + name); lines[ff + 1] = scale; lines[ff + 2] = centering; changed = true;
    }
    public byte[] Serialize() { if (!changed) return original; var body = encoding.GetBytes(string.Join(newline, lines)); var bom = encoding.GetPreamble(); return bom.Length == 0 ? body : bom.Concat(body).ToArray(); }
    public byte[] SerializeNoOp() => original;
    private IEnumerable<ControllerDevice> ReadDevices()
    {
        for (var index = 0; index + 2 < lines.Count; index++)
        {
            if (!string.Equals(lines[index].Trim(), "[ctrl]", StringComparison.OrdinalIgnoreCase) || !int.TryParse(lines[index + 2], out var active)) continue;
            var ff = FindSection(index, "[FFScale]"); if (ff >= 0 && ff + 2 < lines.Count) yield return new(lines[index + 1], active != 0, lines[ff + 1], lines[ff + 2]);
        }
    }
    private int FindController(string name) { for (var index = 0; index + 2 < lines.Count; index++) if (string.Equals(lines[index].Trim(), "[ctrl]", StringComparison.OrdinalIgnoreCase) && string.Equals(lines[index + 1], name, StringComparison.OrdinalIgnoreCase)) return index; return -1; }
    private int FindSection(int start, string section) { for (var index = start + 1; index < lines.Count; index++) { if (index > start + 1 && string.Equals(lines[index].Trim(), "[ctrl]", StringComparison.OrdinalIgnoreCase)) return -1; if (string.Equals(lines[index].Trim(), section, StringComparison.OrdinalIgnoreCase)) return index; } return -1; }
}

public sealed record AiMaxCountRandomPatch(int Index, int Value) : IConfigSemanticPatch
{
    public string FileName => "options.cfg";
    public byte[] Apply(byte[] source) { var document = new OptionsDocument(source); document.SetAiMaxCountRandomComponent(Index, Value); return document.Serialize(); }
}

// OMSI stores this UI choice as two independent negative presence tokens.
public sealed record ReducedMultithreadingPatch(bool Enabled) : IConfigSemanticPatch
{
    public string FileName => "options.cfg";
    public byte[] Apply(byte[] source)
    {
        var document = BracketTokenDocument.Parse(source);
        document.SetPresence("no_multithreading_calculate", Enabled);
        document.SetPresence("no_multithreading_texload", Enabled);
        return document.Serialize(false);
    }
}

public sealed record BooleanValueTokenPatch(string Token, bool Value, string TrueValue, string FalseValue) : IConfigSemanticPatch
{
    public string FileName => "options.cfg";
    public byte[] Apply(byte[] source)
    {
        var document = BracketTokenDocument.Parse(source);
        document.SetValue(Token, Value ? TrueValue : FalseValue);
        return document.Serialize(false);
    }
}

// The four smoke-system values are a single options.cfg block, not unrelated tokens.
public sealed record SmokeSystemsPatch(bool Enabled, int MaxPerEmitter, bool PlayerVehicleOnly, bool InReflections) : IConfigSemanticPatch
{
    public string FileName => "options.cfg";
    public byte[] Apply(byte[] source)
    {
        var document = BracketTokenDocument.Parse(source);
        document.SetValues("smokesystems", new[]
        {
            Enabled ? "1" : "0",
            MaxPerEmitter.ToString(System.Globalization.CultureInfo.InvariantCulture),
            PlayerVehicleOnly ? "1" : "0",
            InReflections ? "0" : "1"
        });
        return document.Serialize(false);
    }
}

public sealed class FileConfigurationTransaction : IConfigurationTransaction, IConfigurationSnapshot, IConfigurationRecovery
{
    private readonly string root; private readonly string journalPath; private readonly Dictionary<string, byte[]> overlays; private readonly HashSet<string> deletions; private readonly Guid sessionId;
    public IReadOnlyList<FileSnapshot> Snapshots { get; private set; } = Array.Empty<FileSnapshot>(); public IConfigurationSnapshot Snapshot => this; IReadOnlyList<FileSnapshot> IConfigurationSnapshot.Files => Snapshots;
    public FileConfigurationTransaction(string installationRoot, IReadOnlyDictionary<string, byte[]> overlays, IEnumerable<string>? deletions = null, Guid? sessionId = null) { root = Path.GetFullPath(installationRoot); this.overlays = new(overlays, StringComparer.OrdinalIgnoreCase); this.deletions = new(deletions ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase); if (this.deletions.Overlaps(this.overlays.Keys)) throw new InvalidOperationException("A transaction file cannot be staged and deleted."); this.sessionId = sessionId ?? Guid.NewGuid(); journalPath = Path.Combine(root, ".omsilaunch", "journal.json"); }
    public async Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(journalPath)!);
        Snapshots = overlays.Keys.Concat(deletions).Distinct(StringComparer.OrdinalIgnoreCase).Select(SnapshotFile).ToArray(); await Persist(TransactionState.Prepared, cancellationToken).ConfigureAwait(false);
        foreach (var (relative, bytes) in overlays) await AtomicWrite(Path.Combine(root, relative), bytes, cancellationToken).ConfigureAwait(false);
        foreach (var relative in deletions) { var path = Path.Combine(root, relative); if (File.Exists(path)) File.Delete(path); }
        await Persist(TransactionState.Applied, cancellationToken).ConfigureAwait(false);
    }
    public async Task RestoreAsync(CancellationToken cancellationToken = default)
    {
        await Persist(TransactionState.Restoring, cancellationToken).ConfigureAwait(false);
        foreach (var file in Snapshots) { var path = Path.Combine(root, file.Path); if (file.Existed) await AtomicWrite(path, file.Bytes, cancellationToken).ConfigureAwait(false); else if (File.Exists(path)) File.Delete(path); }
        foreach (var file in Snapshots.Where(x => x.Existed)) if (!string.Equals(Hash(File.ReadAllBytes(Path.Combine(root, file.Path))), file.Sha256, StringComparison.OrdinalIgnoreCase)) throw new IOException("Restore hash mismatch: " + file.Path);
        await Persist(TransactionState.Restored, cancellationToken).ConfigureAwait(false); File.Delete(journalPath);
    }
    public Task<bool> HasPendingRecoveryAsync(CancellationToken cancellationToken = default) => Task.FromResult(File.Exists(journalPath));
    public async Task RestorePendingAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(journalPath)) return; var journal = JsonSerializer.Deserialize<TransactionJournal>(await File.ReadAllTextAsync(journalPath, cancellationToken).ConfigureAwait(false)) ?? throw new InvalidDataException("Invalid OmsiLaunch journal.");
        if (OwnerIsAlive(journal)) throw new IOException("OL_E_INSTALLATION_BUSY: a journaled OMSI process is still alive.");
        Snapshots = journal.Files.Select(x => new FileSnapshot(x.RelativePath, x.Existed, x.Sha256, x.Existed ? File.ReadAllBytes(x.BackupPath) : Array.Empty<byte>())).ToArray(); await RestoreAsync(cancellationToken).ConfigureAwait(false);
    }
    public Task MarkStateAsync(TransactionState state, CancellationToken cancellationToken = default) => Persist(state, cancellationToken);
    public Task RecordProcessAsync(int processId, DateTimeOffset creationTimeUtc, string executablePath, CancellationToken cancellationToken = default) => Persist(TransactionState.ProcessStarted, cancellationToken, processId, creationTimeUtc.UtcDateTime.Ticks, executablePath);
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    private FileSnapshot SnapshotFile(string relative) { var path = Path.Combine(root, relative); var bytes = File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>(); return new(relative, File.Exists(path), Hash(bytes), bytes); }
    private async Task Persist(TransactionState state, CancellationToken cancellationToken, int? processId = null, long? processStartFileTimeUtc = null, string? executablePath = null)
    {
        var backupRoot = Path.Combine(Path.GetDirectoryName(journalPath)!, "backup"); Directory.CreateDirectory(backupRoot);
        var files = Snapshots.Select(x => { var backup = Path.Combine(backupRoot, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(x.Path))) + ".bin"); if (x.Existed) File.WriteAllBytes(backup, x.Bytes); return new JournalFile(x.Path, x.Existed, x.Sha256, backup); }).ToArray();
        await AtomicWrite(journalPath, JsonSerializer.SerializeToUtf8Bytes(new TransactionJournal(sessionId, state, files, processId, processStartFileTimeUtc, executablePath)), cancellationToken).ConfigureAwait(false);
    }
    private static async Task AtomicWrite(string path, byte[] bytes, CancellationToken token) { Directory.CreateDirectory(Path.GetDirectoryName(path)!); var temporary = path + ".omsilaunch.tmp"; await File.WriteAllBytesAsync(temporary, bytes, token).ConfigureAwait(false); File.Move(temporary, path, true); }
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
    private static bool OwnerIsAlive(TransactionJournal journal)
    {
        if (journal.ProcessId is not { } processId || journal.ProcessStartFileTimeUtc is not { } expected) return false;
        try
        {
            using var process = System.Diagnostics.Process.GetProcessById(processId);
            if (process.HasExited || process.StartTime.ToUniversalTime().Ticks != expected) return false;
            if (string.IsNullOrWhiteSpace(journal.ExecutablePath)) return true;
            // PID and start time reject reuse; the recorded executable path
            // prevents a live unrelated process from retaining this lease.
            return string.Equals(process.MainModule?.FileName, journal.ExecutablePath, StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException) { return false; }
        catch (InvalidOperationException) { return false; }
        catch (System.ComponentModel.Win32Exception) { return false; }
    }
}
