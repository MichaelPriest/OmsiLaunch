using System.IO.MemoryMappedFiles;
using System.Buffers.Binary;
using OmsiLaunch.Api;
using OmsiLaunch.Interop;
using OmsiLaunch.Plugin;

var handoff = new StartupHandoff(Guid.NewGuid(), "Omsi23004_692EBFBF", WorldMode.NewMap, "maps\\Grundorf\\global.cfg", -1, true, false, DateTimeMode.Unset, DateTimeMode.Unset, "Nordspitze Bauernhof");
var bytes = StartupHandoffWire.Serialize(handoff); var name = "OmsiLaunch.PluginTest." + handoff.SessionId.ToString("N");
using var mapping = MemoryMappedFile.CreateNew(name, bytes.Length, MemoryMappedFileAccess.ReadWrite);
using (var view = mapping.CreateViewAccessor(0, bytes.Length, MemoryMappedFileAccess.Write)) view.WriteArray(0, bytes, 0, bytes.Length);
Environment.SetEnvironmentVariable("OMSILAUNCH_HANDOFF_NAME", name);
var runtimeName = "OmsiLaunch.RuntimeTest." + handoff.SessionId.ToString("N");
using var runtimeMapping = MemoryMappedFile.CreateNew(runtimeName, 65_536, MemoryMappedFileAccess.ReadWrite);
Environment.SetEnvironmentVariable("OMSILAUNCH_RUNTIME_CHANNEL", runtimeName);
Action? callback = null; var native = new FakeNative(); var events = new List<(string Name, IReadOnlyDictionary<string, string> Data)>();
var runtimeControl = new FakeRuntimeControl();
var runtime = new PluginRuntime(native, (eventName, data) => events.Add((eventName, data)), runtimeControl);
if (!runtime.Start(action => callback = action) || callback is null) throw new InvalidOperationException("PluginRuntime did not accept the portable handoff.");
callback();
if (!native.Armed || native.Map != "maps\\Grundorf\\global.cfg" || native.Index != -1 || native.EntrypointIdentity != "Nordspitze Bauernhof" || !events.Any(x => x.Name == "gameplay.entered" && x.Data.TryGetValue("raw_label", out var label) && label == "Nordspitze Bauernhof")) throw new InvalidOperationException("PluginRuntime did not dispatch the semantic NEW_MAP request.");
Console.WriteLine("PASS plugin-runtime.handoff-new-map");

var savedHandoff = new StartupHandoff(Guid.NewGuid(), "Omsi23004_692EBFBF", WorldMode.SavedSituation, "", -1, true, false, DateTimeMode.Unset, DateTimeMode.Unset, "", "situations\\23A 2005.osn");
var savedBytes = StartupHandoffWire.Serialize(savedHandoff);
if (!StartupHandoffWire.TryDeserialize(savedBytes, out var decodedSaved) || decodedSaved is null || decodedSaved.SituationIdentity != savedHandoff.SituationIdentity || decodedSaved.WorldMode != WorldMode.SavedSituation) throw new InvalidOperationException("Startup handoff v4 did not preserve saved-situation identity.");
var savedName = "OmsiLaunch.PluginTest." + savedHandoff.SessionId.ToString("N");
using var savedMapping = MemoryMappedFile.CreateNew(savedName, savedBytes.Length, MemoryMappedFileAccess.ReadWrite);
using (var view = savedMapping.CreateViewAccessor(0, savedBytes.Length, MemoryMappedFileAccess.Write)) view.WriteArray(0, savedBytes, 0, savedBytes.Length);
Environment.SetEnvironmentVariable("OMSILAUNCH_HANDOFF_NAME", savedName);
var savedEvents = new List<(string Name, IReadOnlyDictionary<string, string> Data)>();
Action? savedCallback = null; var savedNative = new FakeNative();
if (!new PluginRuntime(savedNative, (eventName, data) => savedEvents.Add((eventName, data))).Start(action => savedCallback = action) || savedCallback is null) throw new InvalidOperationException("PluginRuntime did not accept the profiled SAVED_SITUATION handoff.");
savedCallback();
if (savedNative.Situation != savedHandoff.SituationIdentity || !savedEvents.Any(x => x.Name == "gameplay.entered" && x.Data.TryGetValue("situation", out var situation) && situation == savedHandoff.SituationIdentity)) throw new InvalidOperationException("PluginRuntime did not dispatch the full saved-situation Start flow.");
Environment.SetEnvironmentVariable("OMSILAUNCH_HANDOFF_NAME", name);
Console.WriteLine("PASS handoff.saved-situation-v4");

var command = new RuntimeCommand(handoff.SessionId, 7, "time.read"); var commandBytes = RuntimeCommandWire.SerializeRequest(command);
using (var view = runtimeMapping.CreateViewAccessor(0, 65_536, MemoryMappedFileAccess.ReadWrite)) { view.Write(4, commandBytes.Length); view.WriteArray(8, commandBytes, 0, commandBytes.Length); view.Write(0, 1); view.Flush(); }
// Lifecycle probing is deferred until OMSI crosses gameplay readiness.
Thread.Sleep(2_050);
runtime.PollRuntimeCommands();
using (var view = runtimeMapping.CreateViewAccessor(0, 65_536, MemoryMappedFileAccess.Read)) { if (view.ReadInt32(0) != 2) throw new InvalidOperationException("PluginRuntime did not complete runtime request."); var length = view.ReadInt32(4); var response = new byte[length]; view.ReadArray(8, response, 0, length); if (!RuntimeCommandWire.TryDeserializeResponse(response, out var result) || result is null || !result.Succeeded || result.RequestId != 7) throw new InvalidOperationException("Runtime command response did not preserve session binding."); }
if (!events.Any(entry => entry.Name == "d3d.ready")) throw new InvalidOperationException("PluginRuntime did not publish the runtime-control lifecycle event.");
runtime.Shutdown();
if (!runtimeControl.ShutdownObserved) throw new InvalidOperationException("PluginRuntime did not transfer shutdown ownership to runtime control.");
Console.WriteLine("PASS plugin-runtime.command-channel");

var memory = new FakeMemory();
memory.WriteUInt32(0x100, 0x200); memory.WriteInt32(0x1FC, 4); memory.WriteBytes(0x200, System.Text.Encoding.Unicode.GetBytes("Test"));
if (await memory.ReadStringAsync(0x100, OmsiStringEncoding.Unicode) != "Test") throw new InvalidOperationException("UnicodeString marshalling failed.");
memory.WriteUInt32(0x300, 0x400); memory.WriteInt32(0x3FC, 2); memory.WriteUInt32(0x400, 0x500); memory.WriteUInt32(0x404, 0x600);
memory.WriteInt32(0x4FC, 1); memory.WriteBytes(0x500, System.Text.Encoding.Unicode.GetBytes("A")); memory.WriteInt32(0x5FC, 1); memory.WriteBytes(0x600, System.Text.Encoding.Unicode.GetBytes("B"));
var strings = await memory.ReadStringArrayAsync(0x300, OmsiStringEncoding.Unicode);
if (strings.Length != 2 || strings[0] != "A" || strings[1] != "B") throw new InvalidOperationException("UnicodeString array marshalling failed.");
memory.WriteUInt32(0x700 + 0x04, 0x800); memory.WriteInt32(0x700 + 0x08, 2); memory.WriteInt32(0x7FC, 2); memory.WriteUInt32(0x800, 0x900); memory.WriteUInt32(0x804, 0);
var objects = await new OmsiPointerList<FakeObject>(memory, 0x700, OmsiPointerListLayout.DelphiTList, (source, address) => new FakeObject(source, address)).SnapshotAsync();
if (objects.Count != 2 || objects[0]?.Address != 0x900 || objects[1] is not null) throw new InvalidOperationException("TList pointer collection marshalling failed.");
memory.WriteUInt32(0xA00, 0xB00); memory.WriteInt32(0xA10, 42);
var profiled = new OmsiProfiledObject(memory, 0xA00, new OmsiObjectLayout("TestObject", 0xB00, new Dictionary<string, uint> { ["Answer"] = 0x10 }));
if (!await profiled.ValidateAsync() || await profiled.ReadFieldAsync<int>("Answer") != 42) throw new InvalidOperationException("Profiled object field read failed.");
await profiled.WriteFieldAsync("Answer", 43); if (await profiled.ReadFieldAsync<int>("Answer") != 43) throw new InvalidOperationException("Profiled object field write failed.");
memory.WriteUInt32(0xB10 + 0x04, 0xB40); memory.WriteInt32(0xB10 + 0x08, 1); memory.WriteUInt32(0xB40, 0xB80); memory.WriteInt32(0xB7C, 2); memory.WriteBytes(0xB80, new byte[] { (byte)'P', (byte)'T' });
var ansiList = await new OmsiStringList(memory, 0xB10, OmsiPointerListLayout.DelphiTList, OmsiStringEncoding.Ansi).SnapshotAsync();
if (ansiList.Count != 1 || ansiList[0] != "PT") throw new InvalidOperationException("TList string marshalling failed.");
var availability = OmsiRuntimeSurface.Resolve(symbol => symbol == "NewSituation");
if (!availability.Single(entry => entry.Operation.Id == "program.new-situation").Available || availability.Single(entry => entry.Operation.Id == "weather.write").Available) throw new InvalidOperationException("Runtime surface capability resolution failed.");
Console.WriteLine("PASS interop.delphi-memory-primitives");

sealed class FakeNative : IPluginNativeServices
{
    public bool Armed { get; private set; } public string? Map { get; private set; } public int Index { get; private set; } public string? EntrypointIdentity { get; private set; }
    public bool ValidateBuild(string profileIdentity) => profileIdentity == "Omsi23004_692EBFBF";
    public bool ArmHeadlessStart() { Armed = true; return true; }
    public bool RevealStartFormFromHeadless() => true;
    public int StartNewMap(string mapIdentity, int presentedEntrypointIndex, string entrypointIdentity) { Map = mapIdentity; Index = presentedEntrypointIndex; EntrypointIdentity = entrypointIdentity; return 0; }
    public string? Situation { get; private set; }
    public int StartSavedSituation(string situationIdentity) { Situation = situationIdentity; return 0; }
    public bool TryGetLastEntrypointSelection(out NativeEntrypointSelection selection) { selection = new(1, 0, "", "Nordspitze Bauernhof"); return true; }
    public void InstallMainThreadGateway() { }
}

sealed class FakeRuntimeControl : IPluginRuntimeControl
{
    private bool emitted;
    public bool ShutdownObserved { get; private set; }
    public RuntimeCommandResult Execute(RuntimeCommand command) => new(command.SessionId, command.RequestId, true, Values: new Dictionary<string, string> { ["source"] = "fake" });
    public IReadOnlyList<PluginRuntimeEvent> PollLifecycle()
    {
        if (emitted) return Array.Empty<PluginRuntimeEvent>();
        emitted = true;
        return new[] { new PluginRuntimeEvent("d3d.ready", new Dictionary<string, string> { ["state"] = "READY" }) };
    }
    public void Shutdown() => ShutdownObserved = true;
}

sealed class FakeMemory : IOmsiMemory
{
    private readonly byte[] bytes = new byte[0x1000];
    public ValueTask<int> ReadAsync(nint address, Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        bytes.AsMemory((int)address, destination.Length).CopyTo(destination);
        return ValueTask.FromResult(destination.Length);
    }
    public ValueTask WriteAsync(nint address, ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
    {
        source.CopyTo(bytes.AsMemory((int)address, source.Length));
        return ValueTask.CompletedTask;
    }
    public void WriteUInt32(int address, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(address), value);
    public void WriteInt32(int address, int value) => BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(address), value);
    public void WriteBytes(int address, byte[] value) => value.CopyTo(bytes, address);
}

sealed class FakeObject : OmsiRemoteObject
{
    public FakeObject(IOmsiMemory memory, uint address) : base(memory, address) { }
}
