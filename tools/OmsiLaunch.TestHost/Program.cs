using OmsiLaunch.Api;
using OmsiLaunch.Configuration;
using OmsiLaunch.Content;
using OmsiLaunch.Process;
using System.IO.MemoryMappedFiles;

var tests = new List<(string Name, Action Run)>
{
    ("handoff.roundtrip", HandoffRoundTrip),
    ("handoff.hash-guard", HandoffHashGuard),
    ("runtime-command.wire-guard", RuntimeCommandWireGuard),
    ("runtime-command.session-binding", RuntimeCommandSessionBinding),
    ("options.noop", OptionsNoOp),
    ("options.vector-tail", OptionsVectorTail),
    ("discovery.fixture", DiscoveryFixture),
    ("transaction.restore", TransactionRestore),
    ("transaction.absent-overlay-restore", TransactionAbsentOverlayRestore),
    ("transaction.session-delete-restore", TransactionDeleteRestore),
    ("configuration.catalog", ConfigurationCatalogTest)
    ,("configuration.ranges-transforms", ConfigurationRanges)
    ,("configuration.compound-patches", ConfigurationCompoundPatches)
    ,("keyboard.patch", KeyboardPatch)
    ,("controller.patch", ControllerPatch)
    ,("runtime.manifest", RuntimeManifest),
    ("process.createprocess.fast-exit", ProcessProviderFastExit),
    ("lease.cross-thread-release", LeaseCrossThreadRelease),
    ("runtime.permanent-plugin-installation", RuntimePermanentPluginInstallation)
};
var failures = new List<string>();
foreach (var test in tests)
{
    try { test.Run(); Console.WriteLine("PASS " + test.Name); }
    catch (Exception error) { failures.Add(test.Name + ": " + error.Message); Console.Error.WriteLine("FAIL " + test.Name + " " + error); }
}
if (failures.Count != 0) { Console.Error.WriteLine(string.Join(Environment.NewLine, failures)); Environment.ExitCode = 1; }

static void HandoffRoundTrip()
{
    var expected = new StartupHandoff(Guid.NewGuid(), "Omsi23004_692EBFBF", WorldMode.NewMap, "maps\\Grundorf\\global.cfg", -1, true, false, DateTimeMode.Unset, DateTimeMode.Unset, "Nordspitze Bauernhof");
    Assert(StartupHandoffWire.TryDeserialize(StartupHandoffWire.Serialize(expected), out var actual) && actual == expected, "portable handoff did not round-trip");
}
static void HandoffHashGuard()
{
    var data = StartupHandoffWire.Serialize(new StartupHandoff(Guid.NewGuid(), "profile", WorldMode.NewMap, "maps\\Grundorf\\global.cfg", 1, true, false, DateTimeMode.Unset, DateTimeMode.Unset)); data[^1] ^= 0x01;
    Assert(!StartupHandoffWire.TryDeserialize(data, out _), "corrupt handoff was accepted");
}
static void RuntimeCommandWireGuard()
{
    var expected = new RuntimeCommand(Guid.NewGuid(), 42, "time.read", new Dictionary<string, string> { ["scope"] = "session" });
    var bytes = RuntimeCommandWire.SerializeRequest(expected);
    Assert(RuntimeCommandWire.TryDeserializeRequest(bytes, out var decoded) && decoded is not null && decoded.SessionId == expected.SessionId && decoded.RequestId == expected.RequestId && decoded.Operation == expected.Operation && decoded.Arguments is not null && decoded.Arguments["scope"] == "session", "runtime command did not round-trip");
    bytes[^1] ^= 1; Assert(!RuntimeCommandWire.TryDeserializeRequest(bytes, out _), "corrupt runtime request was accepted");
}
static void RuntimeCommandSessionBinding()
{
    var session = Guid.NewGuid(); using var store = CurrentRuntimeCommandStore.Create(session);
    var request = new RuntimeCommand(session, 9, "time.read");
    var pending = store.RequestAsync(request, TimeSpan.FromSeconds(2));
    using (var mapping = MemoryMappedFile.OpenExisting(store.Name, MemoryMappedFileRights.ReadWrite))
    using (var view = mapping.CreateViewAccessor(0, 65_536, MemoryMappedFileAccess.ReadWrite))
    {
        var until = DateTimeOffset.UtcNow.AddSeconds(1); while (view.ReadInt32(0) != 1 && DateTimeOffset.UtcNow < until) Thread.Sleep(10);
        Assert(view.ReadInt32(0) == 1, "runtime request was not staged");
        var response = RuntimeCommandWire.SerializeResponse(new RuntimeCommandResult(session, 9, true, Values: new Dictionary<string, string> { ["ok"] = "true" }));
        view.Write(4, response.Length); view.WriteArray(8, response, 0, response.Length); view.Write(0, 2); view.Flush();
    }
    Assert(pending.GetAwaiter().GetResult().Succeeded, "runtime response was not consumed");
}
static void RuntimePermanentPluginInstallation()
{
    var root = Path.Combine(Path.GetTempPath(), "OmsiLaunch-Orphan-Test-" + Guid.NewGuid().ToString("N"));
    var source = Path.Combine(root, "source"); Directory.CreateDirectory(source); Directory.CreateDirectory(Path.Combine(root, "plugins"));
    try
    {
        foreach (var file in new[] { "OmsiLaunch.Plugin.opl", "OmsiLaunch.PluginNE.dll", "OmsiLaunch.Plugin.deps.json", "OmsiLaunch.Plugin.runtimeconfig.json", "OmsiLaunch.Plugin.dll", "OmsiLaunch.Api.dll" })
            File.WriteAllText(Path.Combine(source, file), file + "-source");
        var native = Path.Combine(root, "OmsiLaunch.Native.x86.dll"); File.WriteAllText(native, "native-source");
        var deployment = RuntimeArtifactSet.Load(source, native);
        foreach (var artifact in deployment.Artifacts) File.Copy(artifact.SourcePath, Path.Combine(root, artifact.DestinationRelativePath), true);
        deployment.ValidateInstalled(root);
        var collision = deployment.Artifacts.Single(x => x.DestinationRelativePath.EndsWith("OmsiLaunch.Api.dll", StringComparison.OrdinalIgnoreCase));
        File.WriteAllText(Path.Combine(root, collision.DestinationRelativePath), "partial-product-update");
        AssertThrows(() => deployment.ValidateInstalled(root), "permanent plugin hash mismatch was accepted");
        Assert(File.Exists(Path.Combine(root, collision.DestinationRelativePath)), "plugin validation removed a permanent product file");
    }
    finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
}
static void OptionsNoOp()
{
    var input = System.Text.Encoding.ASCII.GetBytes("[unknown]\r\nvalue\r\n[useActTime]\r\n\r\n");
    Assert(input.SequenceEqual(new OptionsDocument(input).Serialize()), "no-op options round-trip changed bytes");
}
static void OptionsVectorTail()
{
    var input = System.Text.Encoding.ASCII.GetBytes("[AIMaxCountRandom]\r\n1 2 3 4 5 6 7 8 9\r\n"); var document = new OptionsDocument(input); document.SetAiMaxCountRandom(11, 12);
    var output = System.Text.Encoding.ASCII.GetString(document.Serialize()); Assert(output.Contains("11 12 3 4 5 6 7 8 9", StringComparison.Ordinal), "AIMaxCountRandom tail was not preserved");
}
static void DiscoveryFixture()
{
    var root = Fixture(); try
    {
        Write(root, "maps\\Grundorf\\global.cfg", "[entrypoints]\r\n1\r\n4\r\n103\r\n0\r\n202.000\r\n9.000\r\n136.000\r\n0.000\r\n0.173\r\n0.000\r\n0.985\r\n4\r\nNordspitze Bauernhof\r\n"); Write(root, "situations\\23A 2005.osn", "maps\\Grundorf\\global.cfg"); Write(root, "Vehicles\\Demo\\demo.bus", "[friendlyname]\r\nDemo\r\nBus\r\n[number]\r\nnumbers.org\r\n[registration_automatic]\r\nD-A\r\n[registration_free]\r\n"); Write(root, "Vehicles\\Demo\\demo.cti", "[item]\r\nBlue\r\nvar\r\nblue.png\r\n"); Write(root, "Vehicles\\Demo\\demo.hof", "");
        var catalog = new FileSystemContentCatalog(root); Assert(catalog.ResolveMap("maps\\Grundorf\\global.cfg").IsReadable, "map was not discovered"); Assert(catalog.EnumerateEntrypoints("maps\\Grundorf\\global.cfg").Single().Identity.StartsWith("maps\\Grundorf\\global.cfg#entrypoint:", StringComparison.Ordinal) && catalog.EnumerateEntrypoints("maps\\Grundorf\\global.cfg").Single().DisplayName == "Nordspitze Bauernhof", "stable opaque entrypoint identity was not discovered"); Assert(catalog.EnumerateSituations().Single().MapIdentity == "maps\\Grundorf\\global.cfg", "situation map was not parsed"); Assert(catalog.EnumerateVehicles().Single().DisplayName == "Demo Bus", "vehicle friendly name was not parsed"); Assert(catalog.EnumerateRepaints("Vehicles\\Demo\\demo.bus").Single().Identity.EndsWith("#item:1", StringComparison.Ordinal) && catalog.EnumerateRepaints("Vehicles\\Demo\\demo.bus").Single().DisplayName == "Blue", "stable repaint identity was not discovered"); Assert(catalog.EnumerateFleetNumbers("Vehicles\\Demo\\demo.bus").Single().SourceIdentity == "numbers.org", "fleet number source was not discovered"); Assert(catalog.EnumerateRegistrations("Vehicles\\Demo\\demo.bus").Select(x => x.Mode).OrderBy(x => x).SequenceEqual(new[] { "registration_automatic", "registration_free" }), "registration modes were not discovered");
    }
    finally { Directory.Delete(root, true); }
}
static void TransactionRestore()
{
    var root = Fixture(); try
    {
        Write(root, "options.cfg", "original"); var transaction = new FileConfigurationTransaction(root, new Dictionary<string, byte[]> { ["options.cfg"] = System.Text.Encoding.ASCII.GetBytes("temporary") }); transaction.ApplyAsync().GetAwaiter().GetResult(); Assert(File.ReadAllText(Path.Combine(root, "options.cfg")) == "temporary", "transaction did not apply"); transaction.RestoreAsync().GetAwaiter().GetResult(); Assert(File.ReadAllText(Path.Combine(root, "options.cfg")) == "original", "transaction did not restore exact original");
    }
    finally { Directory.Delete(root, true); }
}
static void TransactionAbsentOverlayRestore()
{
    var root = Fixture(); try
    {
        const string overlay = "GUI\\NewSplashscreen_PTB.bmp";
        var path = Path.Combine(root, overlay);
        var transaction = new FileConfigurationTransaction(root, new Dictionary<string, byte[]> { [overlay] = System.Text.Encoding.ASCII.GetBytes("session-splash") });
        transaction.ApplyAsync().GetAwaiter().GetResult();
        Assert(File.Exists(path), "session overlay was not created for an absent destination");
        transaction.RestoreAsync().GetAwaiter().GetResult();
        Assert(!File.Exists(path), "restore did not remove a session overlay whose original destination was absent");
        Assert(!File.Exists(Path.Combine(root, ".omsilaunch", "journal.json")), "journal remained after absent-overlay restore");
    }
    finally { Directory.Delete(root, true); }
}
static void TransactionDeleteRestore()
{
    var root = Fixture(); try
    {
        Write(root, "Texture\\standard.ipr", "original-cache"); Write(root, "Sceneryobjects\\Demo\\texture\\target.tga", "original-target");
        var transaction = new FileConfigurationTransaction(root, new Dictionary<string, byte[]> { ["Texture\\standard.itx"] = System.Text.Encoding.ASCII.GetBytes("override") }, new[] { "Texture\\standard.ipr", "Sceneryobjects\\Demo\\texture\\target.tga" });
        transaction.ApplyAsync().GetAwaiter().GetResult(); Assert(!File.Exists(Path.Combine(root, "Texture\\standard.ipr")) && !File.Exists(Path.Combine(root, "Sceneryobjects\\Demo\\texture\\target.tga")), "session deletion did not apply");
        transaction.RestoreAsync().GetAwaiter().GetResult(); Assert(File.ReadAllText(Path.Combine(root, "Texture\\standard.ipr")) == "original-cache" && File.ReadAllText(Path.Combine(root, "Sceneryobjects\\Demo\\texture\\target.tga")) == "original-target", "session deletion did not restore exact originals");
    }
    finally { Directory.Delete(root, true); }
}
static void ConfigurationCatalogTest()
{
    var input = System.Text.Encoding.ASCII.GetBytes("[unknown]\r\nvalue\r\n[no_collision_terrain]\r\n");
    var patch = ConfigurationCatalog.CreatePatch("simulation.collisionTerrain", "true");
    var enabled = System.Text.Encoding.ASCII.GetString(patch.Apply(input)); Assert(!enabled.Contains("[no_collision_terrain]", StringComparison.Ordinal) && enabled.Contains("[unknown]", StringComparison.Ordinal), "positive collision semantic did not remove the inverted token");
    var disabled = System.Text.Encoding.ASCII.GetString(ConfigurationCatalog.CreatePatch("simulation.collisionTerrain", "false").Apply(input)); Assert(disabled.Contains("[no_collision_terrain]", StringComparison.Ordinal), "false collision semantic did not preserve the inverted token");
    var vectorInput = System.Text.Encoding.ASCII.GetBytes("[AIMaxCountRandom]\r\n1 2 3 4 5 6 7 8 9\r\n");
    var vector = System.Text.Encoding.ASCII.GetString(ConfigurationCatalog.CreatePatch("traffic.humans", "12").Apply(vectorInput)); Assert(vector.Contains("1 12 3 4 5 6 7 8 9", StringComparison.Ordinal), "semantic traffic patch did not preserve vector tail");
    AssertThrows(() => ConfigurationCatalog.CreatePatch("unknown.key", "true"), "unknown semantic setting was accepted");
    AssertThrows(() => ConfigurationCatalog.CreatePatch("graphics.texture", "1"), "non-writable semantic setting was accepted");
}
static void ConfigurationRanges()
{
    var input = System.Text.Encoding.ASCII.GetBytes("[performance_minObjSize]\r\n0.010\r\n[maxFPS]\r\n60\r\n[noAutoSave]\r\n");
    var percent = System.Text.Encoding.ASCII.GetString(ConfigurationCatalog.CreatePatch("graphics.minObjectScreenPercent", "1.50").Apply(input)); Assert(percent.Contains("[performance_minObjSize]\r\n0.015", StringComparison.Ordinal), "percent transform was not applied");
    var fps = System.Text.Encoding.ASCII.GetString(ConfigurationCatalog.CreatePatch("graphics.maxFPS", "200").Apply(input)); Assert(fps.Contains("[maxFPS]\r\n200", StringComparison.Ordinal), "maxFPS range endpoint was rejected");
    var autosave = System.Text.Encoding.ASCII.GetString(ConfigurationCatalog.CreatePatch("general.autoSave", "true").Apply(input)); Assert(!autosave.Contains("[noAutoSave]", StringComparison.Ordinal), "positive autoSave did not remove negative raw token");
    AssertThrows(() => ConfigurationCatalog.CreatePatch("graphics.tileDistance", "21"), "out of range tileDistance was accepted");
    AssertThrows(() => ConfigurationCatalog.CreatePatch("sound.masterVolume", "1.1"), "out of range volume was accepted");
    AssertThrows(() => ConfigurationCatalog.CreatePatch("traffic.humans", "1001"), "out of range human count was accepted");
}
static void ConfigurationCompoundPatches()
{
    var input = System.Text.Encoding.ASCII.GetBytes("[no_multithreading_calculate]\r\n[unknown]\r\nkeep\r\n[smokesystems]\r\n1\r\n200\r\n0\r\n1\r\n");
    var reduced = System.Text.Encoding.ASCII.GetString(ConfigurationCatalog.CreatePatch("advanced.reducedMultithreading", "true").Apply(input));
    Assert(reduced.Contains("[no_multithreading_calculate]", StringComparison.Ordinal) && reduced.Contains("[no_multithreading_texload]", StringComparison.Ordinal), "reduced multithreading did not synchronize both native flags");
    var particles = System.Text.Encoding.ASCII.GetString(ConfigurationCatalog.CreatePatch("graphics.particles", "true,800,true,true").Apply(input));
    Assert(particles.Contains("[smokesystems]\r\n1\r\n800\r\n1\r\n0", StringComparison.Ordinal) && particles.Contains("[unknown]\r\nkeep", StringComparison.Ordinal), "particle semantic patch changed unrelated tokens or inverted reflection state");
    var stencil = System.Text.Encoding.ASCII.GetString(ConfigurationCatalog.CreatePatch("graphics.stencilShadows", "false").Apply(input));
    Assert(stencil.Contains("[shadow_stencil]\r\noff", StringComparison.Ordinal), "stencil shadow boolean was not translated to native on/off");
    AssertThrows(() => ConfigurationCatalog.CreatePatch("graphics.realTimeReflections", "disabled"), "unproven reflection disabled value was accepted");
}
static void KeyboardPatch()
{
    var input = System.Text.Encoding.ASCII.GetBytes("[entry]\r\nquicksave\r\n31\r\n4\r\n[unknown]\r\nvalue\r\n"); var document = new KeyboardDocument(input);
    Assert(document.Bindings.Single() == new KeyboardBinding("quicksave", 31, true, false), "keyboard binding was not parsed");
    document.SetBinding(new KeyboardBinding("quicksave", 30, false, true)); var output = System.Text.Encoding.ASCII.GetString(document.Serialize());
    Assert(output.Contains("quicksave\r\n30\r\n2", StringComparison.Ordinal) && output.Contains("[unknown]\r\nvalue", StringComparison.Ordinal), "keyboard patch changed unknown data or modifiers incorrectly");
}
static void ControllerPatch()
{
    var input = System.Text.Encoding.ASCII.GetBytes("[ctrl]\r\nWheel\r\n0\r\n[axis]\r\n0\r\n[buttons]\r\n0\r\n[FFScale]\r\n1.000\r\n1.000\r\n[unknown]\r\nvalue\r\n"); var document = new GameControllerDocument(input);
    Assert(document.Devices.Single() == new ControllerDevice("Wheel", false, "1.000", "1.000"), "controller block was not parsed"); document.SetActive("Wheel", true); document.SetForceFeedback("Wheel", "0.750", "0.500"); var output = System.Text.Encoding.ASCII.GetString(document.Serialize());
    Assert(output.Contains("[ctrl]\r\nWheel\r\n1", StringComparison.Ordinal) && output.Contains("[FFScale]\r\n0.750\r\n0.500", StringComparison.Ordinal) && output.Contains("[unknown]\r\nvalue", StringComparison.Ordinal), "controller patch changed unrelated data");
}
static void RuntimeManifest()
{
    var root = Fixture(); try
    {
        var plugin = Path.Combine(root, "plugin"); Directory.CreateDirectory(plugin);
        foreach (var file in new[] { "OmsiLaunch.Plugin.opl", "OmsiLaunch.PluginNE.dll", "OmsiLaunch.Plugin.dll", "OmsiLaunch.Api.dll", "OmsiLaunch.Interop.dll", "OmsiLaunch.Builds.Omsi23004.dll", "OmsiLaunch.Plugin.deps.json", "OmsiLaunch.Plugin.runtimeconfig.json" }) File.WriteAllText(Path.Combine(plugin, file), file);
        var native = Path.Combine(root, "OmsiLaunch.Native.x86.dll"); File.WriteAllText(native, "native");
        var artifacts = RuntimeArtifactSet.Load(plugin, native); Assert(artifacts.Artifacts.Count == 9, "runtime manifest omitted an artifact"); Assert(artifacts.Artifacts.Any(x => x.DestinationRelativePath.EndsWith(Path.Combine("plugins", "OmsiLaunch.Interop.dll"), StringComparison.OrdinalIgnoreCase)) && artifacts.Artifacts.Any(x => x.DestinationRelativePath.EndsWith(Path.Combine("plugins", "OmsiLaunch.Builds.Omsi23004.dll"), StringComparison.OrdinalIgnoreCase)), "managed dependency closure was not delivered to permanent plugins"); Assert(artifacts.Artifacts.All(x => x.DestinationRelativePath.StartsWith("plugins" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)), "runtime destination escaped plugins");
    }
    finally { Directory.Delete(root, true); }
}
static void ProcessProviderFastExit()
{
    var platform = new CurrentWindowsX64Platform(); var executable = Path.Combine(Environment.SystemDirectory, "where.exe");
    using var process = platform.StartAsync(new StartupProcessRequest(executable, Environment.SystemDirectory, new Dictionary<string, string> { ["OMSILAUNCH_PROCESS_TEST"] = "1" }), "test", CancellationToken.None).GetAwaiter().GetResult();
    Assert(process.ProcessId > 0 && process.ThreadId > 0, "CreateProcessW did not return raw PROCESS_INFORMATION"); Assert(process.Identity.CreationTimeUtc.Year > 2000, "GetProcessTimes did not produce a creation timestamp");
    platform.WaitForExitAsync(process, CancellationToken.None).GetAwaiter().GetResult(); Assert(platform.HasExited(process), "owned process handle did not observe exit");
}
static void LeaseCrossThreadRelease()
{
    var root = Fixture(); try { using var first = InstallationLease.Acquire(root); AssertThrows(() => InstallationLease.Acquire(root), "second lease holder was accepted"); Task.Run(first.Dispose).GetAwaiter().GetResult(); using var second = InstallationLease.Acquire(root); }
    finally { Directory.Delete(root, true); }
}
static string Fixture() { var root = Path.Combine(Path.GetTempPath(), "OmsiLaunch-Test-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root); return root; }
static void Write(string root, string relative, string text) { var path = Path.Combine(root, relative); Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllText(path, text); }
static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
static void AssertThrows(Action action, string message) { try { action(); } catch (ArgumentException) { return; } catch (InvalidOperationException) { return; } catch (InvalidDataException) { return; } throw new InvalidOperationException(message); }
