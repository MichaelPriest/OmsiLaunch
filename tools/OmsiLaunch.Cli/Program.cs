using System.Text.Json;
using System.Diagnostics;
using OmsiLaunch.Api;
using OmsiLaunch.Core;
using OmsiLaunch.Configuration;
using OmsiLaunch.Process;

CliInput input;
try { input = CliInput.Parse(args); }
catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidDataException)
{
    CliInput.WriteError("cli", "OL_E_INVALID_ARGUMENT", exception.Message, args.Any(argument => argument.Equals("--json", StringComparison.OrdinalIgnoreCase) || argument.Equals("/json", StringComparison.OrdinalIgnoreCase)), PublicErrorCategory.InvalidArgument);
    return (int)PublicExitCode.InvalidArguments;
}
if (input.Version) { CliInput.WriteEnvelope("version", new { product = "OmsiLaunch", version = "0.1.0-beta1", protocol_version = PublicCapabilityRegistry.ProtocolVersion, supported_family = "OMSI_2_3_004_COMMON" }, input.JsonOutput); return 0; }
if (input.Command is "capabilities") { CliInput.WriteEnvelope("capabilities", PublicCapabilityRegistry.All.Where(x => x.Classification is PublicCapabilityClassification.PublicStableBeta or PublicCapabilityClassification.PublicExperimental), input.JsonOutput); return 0; }
if (input.Command is "help") { CliInput.WriteEnvelope("help", CliInput.HelpFor(input.CommandWords.SingleOrDefault()), input.JsonOutput); return 0; }
if (input.Command is "profiles") { CliInput.WriteEnvelope("profiles", new { family = "OMSI_2_3_004_COMMON", supported = new object[] { new { variant = "ALTERNATE_LAA", sha256 = "692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243", runtime_validated = true, validation_status = "runtime_validated" }, new { variant = "STEAM_LAA", sha256 = "7DAB063D1F62E73B3A2C7A6AC1921D7EDF5E5DB0FBC731481D117EEC8DE7D759", runtime_validated = false, validation_status = "pending_beta_field_validation" } } }, input.JsonOutput); return 0; }
if (string.IsNullOrWhiteSpace(input.Installation) && input.RuntimeOperation is not null)
{
    if (!PublicCapabilityRegistry.IsPublicRuntimeOperation(input.RuntimeOperation))
    {
        CliInput.WriteError(input.RuntimeOperation, "OL_E_RUNTIME_OPERATION_UNKNOWN", "The runtime operation is not part of the public Beta control surface.", input.JsonOutput, PublicErrorCategory.InvalidArgument);
        return (int)PublicExitCode.InvalidArguments;
    }
    var forwarded = await LocalControlPlane.TryRequestAsync(new(PublicCapabilityRegistry.ProtocolVersion, "runtime.execute", new Dictionary<string, string>(input.RuntimeArguments) { ["operation"] = input.RuntimeOperation }), TimeSpan.FromMilliseconds(750));
    if (forwarded is null) { CliInput.WriteError(input.RuntimeOperation, "OL_E_NO_ACTIVE_SESSION", "No active OmsiLaunch control instance was found.", input.JsonOutput); return (int)PublicExitCode.NoActiveSession; }
    if (!forwarded.Ok) { CliInput.WriteError(input.RuntimeOperation, forwarded.ErrorCode ?? "OL_E_CONTROL_FAILED", forwarded.Message ?? "Control request failed.", input.JsonOutput); return (int)PublicExitCode.OperationRejected; }
    CliInput.WriteEnvelope(input.RuntimeOperation, forwarded.Result!, input.JsonOutput); return (int)PublicExitCode.Success;
}
if (string.IsNullOrWhiteSpace(input.Installation) && input.Command is "session" && input.CommandWords.Count == 1 && input.CommandWords[0].Equals("status", StringComparison.OrdinalIgnoreCase))
{
    var forwarded = await LocalControlPlane.TryRequestAsync(new(PublicCapabilityRegistry.ProtocolVersion, "session.status"), TimeSpan.FromMilliseconds(750));
    if (forwarded is null) { CliInput.WriteError("session status", "OL_E_NO_ACTIVE_SESSION", "No active OmsiLaunch control instance was found.", input.JsonOutput); return 4; }
    CliInput.WriteEnvelope("session.status", forwarded.Result!, input.JsonOutput); return forwarded.Ok ? 0 : 7;
}
if (string.IsNullOrWhiteSpace(input.Installation) && input.Command is "session" && input.CommandWords.Count == 1 && input.CommandWords[0].Equals("stop", StringComparison.OrdinalIgnoreCase))
{
    var forwarded = await LocalControlPlane.TryRequestAsync(new(PublicCapabilityRegistry.ProtocolVersion, "session.stop"), TimeSpan.FromMilliseconds(750));
    if (forwarded is null) { CliInput.WriteError("session stop", "OL_E_NO_ACTIVE_SESSION", "No active OmsiLaunch control instance was found.", input.JsonOutput); return 4; }
    CliInput.WriteEnvelope("session.stop", forwarded.Result!, input.JsonOutput); return forwarded.Ok ? 0 : 7;
}
if (string.IsNullOrWhiteSpace(input.Installation) && input.Command is "events" && input.CommandWords.Count == 1 && input.CommandWords[0].Equals("read", StringComparison.OrdinalIgnoreCase))
{
    var forwarded = await LocalControlPlane.TryRequestAsync(new(PublicCapabilityRegistry.ProtocolVersion, "session.events"), TimeSpan.FromMilliseconds(750));
    if (forwarded is null) { CliInput.WriteError("events read", "OL_E_NO_ACTIVE_SESSION", "No active OmsiLaunch control instance was found.", input.JsonOutput); return (int)PublicExitCode.NoActiveSession; }
    CliInput.WriteEnvelope("events.read", forwarded.Result!, input.JsonOutput); return forwarded.Ok ? 0 : (int)PublicExitCode.OperationRejected;
}
if (string.IsNullOrWhiteSpace(input.Installation) && input.Command is "events" && input.CommandWords.Count == 1 && input.CommandWords[0].Equals("watch", StringComparison.OrdinalIgnoreCase))
    return await CliEventWatch.RunAsync(input);
if (input.Command is "detect" || (string.IsNullOrWhiteSpace(input.Installation) && string.IsNullOrWhiteSpace(input.Executable) && input.Command is null && !input.Help && input.SpecFile is null && !input.LaunchRequested))
{
    var discovered = Process.GetProcessesByName("Omsi").Select(process =>
    {
        try { return new { state = "OMSI_FOUND_UNMANAGED", process_id = process.Id, executable_path = process.MainModule?.FileName, responsive = process.Responding }; }
        catch { return new { state = "UNKNOWN_BINARY_FOUND", process_id = process.Id, executable_path = (string?)null, responsive = false }; }
    }).ToArray();
    var active = await LocalControlPlane.TryRequestAsync(new(PublicCapabilityRegistry.ProtocolVersion, "session.status"), TimeSpan.FromMilliseconds(250));
    CliInput.WriteEnvelope("detect", new { state = discovered.Length == 0 ? "NO_OMSI_FOUND" : "OMSI_FOUND_UNMANAGED", processes = discovered, active_omsilaunch_instance = active?.Ok == true, managed_session = active?.Result }, input.JsonOutput);
    return 0;
}
if (input.Help || (string.IsNullOrWhiteSpace(input.Installation) && string.IsNullOrWhiteSpace(input.Executable) && input.SpecFile is null && !input.LaunchRequested))
{
    Console.WriteLine(CliInput.Usage);
    return input.Help ? 0 : 2;
}

var packagedRuntime = Path.Combine(AppContext.BaseDirectory, "plugins");
var repositoryRuntime = Path.Combine(Directory.GetCurrentDirectory(), "artifacts", "bin", "OmsiLaunch.Plugin", "x86", "Debug", "net6.0-windows");
var pluginRuntime = File.Exists(Path.Combine(packagedRuntime, "OmsiLaunch.Plugin.opl")) ? packagedRuntime : repositoryRuntime;
var nativeRuntime = Path.Combine(pluginRuntime, "OmsiLaunch.Native.x86.dll");
if (!File.Exists(nativeRuntime)) nativeRuntime = Path.Combine(Directory.GetCurrentDirectory(), "artifacts", "x86", "Debug", "OmsiLaunch.Native.x86.dll");
IOmsiLaunch launch = new OmsiLaunchService(new CurrentWindowsX64Platform(), new OmsiLaunchRuntimePaths(pluginRuntime, nativeRuntime));
var spec = await input.BuildSpecAsync();

if (input.Recovery)
{
    var transaction = new OmsiLaunch.Configuration.FileConfigurationTransaction(spec.Installation.RootPath, new Dictionary<string, byte[]>());
    var pending = await transaction.HasPendingRecoveryAsync();
    if (input.Recover && pending) await transaction.RestorePendingAsync();
    CliInput.WriteEnvelope("recover", new { pending, recovered = input.Recover && pending }, input.JsonOutput);
    return 0;
}

if (input.List is not null)
{
    if (!Enum.TryParse<ContentQueryKind>(input.List, true, out var kind)) throw new ArgumentException("Unknown discovery category: " + input.List);
    var scope = kind == ContentQueryKind.Entrypoints ? input.Map : input.VehicleScope;
    var result = await launch.DiscoverAsync(spec.Installation, kind, scope is null ? OptionalValue<string>.Unset : OptionalValue<string>.Set(scope));
    CliInput.WriteEnvelope("content.list", result, input.JsonOutput);
    return 0;
}

var plan = await launch.PlanSessionAsync(spec);
CliInput.Write(plan, input.JsonOutput);
if (input.PlanOnly || input.ValidateOnly) return plan.IsRunnable ? 0 : 1;
if (!plan.IsRunnable) return 1;

var session = await launch.StartSessionAsync(plan);
var running = await launch.WaitForAsync(session, SessionState.Running, TimeSpan.FromSeconds(spec.Behavior.StartupTimeoutSeconds + 5));
CliInput.Write(running, input.JsonOutput);
if (running.State != SessionState.Running)
{
    // Do not let the CLI process exit while its session supervisor still owns
    // a staged runtime deployment or a restore obligation.
    await launch.CloseAsync(session);
    return 1;
}
var runtimeResults = input.D3DBatch
    ? await D3DValidationBatch.ExecuteAsync(launch, session, plan.BuildProfileId)
    : input.RuntimeBatch || input.RuntimeWriteBatch ? await RuntimeBatch.ExecuteAsync(launch, session, plan.BuildProfileId, input.RuntimeWriteBatch) : Array.Empty<RuntimeBatchResult>();
if (input.RuntimeBatch || input.RuntimeWriteBatch) RuntimeBatch.WriteArtifact(spec.Installation.RootPath, session.SessionId, plan.BuildProfileId, runtimeResults, input.RuntimeWriteBatch);
if (input.D3DBatch) D3DValidationBatch.WriteArtifact(spec.Installation.RootPath, session.SessionId, plan.BuildProfileId, runtimeResults);
foreach (var result in runtimeResults) CliInput.Write(result, input.JsonOutput);
var controlStopped = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
ulong forwardedRequestId = 50_000;
await using var control = input.Serve ? new LocalControlPlane(async request =>
{
    if (request.Command == "session.status") return new(true, await launch.GetStatusAsync(session));
    if (request.Command == "session.events") return new(true, (await launch.GetStatusAsync(session)).RuntimeEvents ?? Array.Empty<RuntimeEvent>());
    if (request.Command == "session.stop") { controlStopped.TrySetResult(); return new(true, new { accepted = true, session_id = session.SessionId }); }
    if (request.Command == "runtime.execute" && request.Arguments is not null && request.Arguments.TryGetValue("operation", out var operation))
    {
        var arguments = request.Arguments.Where(x => x.Key != "operation").ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
        var result = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, unchecked(++forwardedRequestId), operation, arguments), TimeSpan.FromSeconds(8));
        return result.Succeeded ? new(true, result) : new(false, result, result.ErrorCode, "Runtime operation was rejected.");
    }
    return new(false, ErrorCode: "OL_E_CONTROL_COMMAND_UNKNOWN", Message: "Unsupported local control command.");
}) : null;
control?.Start();
if (input.RuntimeOperation is not null)
{
    try
    {
        var runtimeTimeout = string.Equals(input.RuntimeOperation, "internal.road-vehicles.make-basic", StringComparison.Ordinal)
            ? TimeSpan.FromSeconds(15) // Oracle execution was 8.5 s on the exact profile.
            : TimeSpan.FromSeconds(5);
        var runtimeResult = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, 10_001, input.RuntimeOperation, input.RuntimeArguments), runtimeTimeout);
        var diagnosticDirectory = Path.Combine(spec.Installation.RootPath, ".omsilaunch", "diagnostics"); Directory.CreateDirectory(diagnosticDirectory);
        File.WriteAllText(Path.Combine(diagnosticDirectory, session.SessionId.ToString("N") + "-runtime-operation.json"), JsonSerializer.Serialize(new { session_id = session.SessionId, operation = input.RuntimeOperation, result = runtimeResult }, CliInput.Json));
        CliInput.Write(runtimeResult, input.JsonOutput);
    }
    catch (Exception exception)
    {
        // A command timeout is not permission to abandon a live transaction.
        // The session remains supervisor-owned until normal stop and restore.
        CliInput.Write(new { runtime_error = exception.Message }, input.JsonOutput);
        await launch.StopAsync(session);
        var failed = await launch.WaitForAsync(session, SessionState.Completed, TimeSpan.FromSeconds(spec.Behavior.ShutdownTimeoutSeconds));
        CliInput.Write(failed, input.JsonOutput);
        await launch.CloseAsync(session);
        return 1;
    }
}
var shouldRequestStop = false;
if (input.Serve)
{
    await controlStopped.Task;
    shouldRequestStop = true;
}
else if (input.ObserveSecondsSpecified || input.RuntimeOperation is not null || input.RuntimeBatch || input.RuntimeWriteBatch || input.D3DBatch)
{
    // Explicit observation and validation/one-shot runtime modes retain the
    // bounded lifecycle used by the test and automation surface.
    await Task.Delay(TimeSpan.FromSeconds(input.ObserveSeconds));
    shouldRequestStop = true;
}
else
{
    // Normal launcher mode: keep the managed session alive until the user
    // closes OMSI. The previous default stopped every successful session after
    // eight seconds, which made the game appear to launch and immediately exit.
    while (true)
    {
        var status = await launch.GetStatusAsync(session);
        if (status.State is SessionState.Completed or SessionState.Failed) break;
        await Task.Delay(250);
    }
}
if (shouldRequestStop) await launch.StopAsync(session);
var completed = await launch.WaitForAsync(session, SessionState.Completed, TimeSpan.FromSeconds(spec.Behavior.ShutdownTimeoutSeconds));
CliInput.Write(completed, input.JsonOutput);
await launch.CloseAsync(session);
return completed.State == SessionState.Completed ? 0 : 1;

internal sealed class CliInput
{
    internal const string Usage = "OmsiLaunch.exe [detect|capabilities|profiles] [<installation>] [/exe:<path-to-Omsi.exe>] [/new|/saved:<file.osn>|/last] [/map:<identity>] [/entrypoint:<identity>|/entrypoint-index:<n>] [/splash:Managed|Native|Unset /splash-language:PTB|ENG|FRA|DEU /splash-assets:<directory>] [/internet-textures:Native|Disabled|Override /internet-textures-profile:<file.itx>] [/spec:<path-to-json>] [/plan|/validate|/runtime:<operation> /runtime-arg:<key=value>] [/recover] [--json].\n\nCommands: detect (default), capabilities, profiles, recover. Managed splash is the default; Native and Unset preserve OMSI files. Runtime operations remain session-scoped and use /runtime:<operation>; consult `capabilities --json` for the canonical Beta catalog.";
    internal static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
    public string? Installation { get; private set; } public string? Executable { get; private set; } public string? Command { get; private set; } public List<string> CommandWords { get; } = new(); public bool Help { get; private set; } public bool Version { get; private set; } public bool JsonOutput { get; private set; } public bool Quiet { get; private set; } public bool Verbose { get; private set; } public bool Log { get; private set; } public bool LogAll { get; private set; } public bool OmsiLogAll { get; private set; } public bool TraceProcess { get; private set; } public bool TracePlugin { get; private set; } public bool TraceNative { get; private set; } public bool PlanOnly { get; private set; } public bool ValidateOnly { get; private set; } public bool Serve { get; private set; } public bool LaunchRequested { get; private set; }
    public SplashMode Splash { get; private set; } = SplashMode.Managed; public bool SplashSpecified { get; private set; } public string? SplashLanguage { get; private set; } public string? SplashAssets { get; private set; } public InternetTexturesMode InternetTextures { get; private set; } = InternetTexturesMode.Native; public string? InternetTexturesProfile { get; private set; }
    public bool Recovery { get; private set; } public bool Recover { get; private set; } public bool RuntimeBatch { get; private set; } public bool RuntimeWriteBatch { get; private set; } public bool D3DBatch { get; private set; } public string? RuntimeOperation { get; private set; } public Dictionary<string, string> RuntimeArguments { get; } = new(StringComparer.Ordinal); public string? List { get; private set; } public string? VehicleScope { get; private set; } public string? SpecFile { get; private set; }
    public WorldMode WorldMode { get; private set; } = WorldMode.NewMap; public string? Map { get; private set; } public string? Situation { get; private set; } public int? EntrypointIndex { get; private set; } public string? EntrypointIdentity { get; private set; }
    public string? Date { get; private set; } public string? Time { get; private set; } public string? Year { get; private set; } public WeatherMode WeatherMode { get; private set; } public string? Weather { get; private set; } public string? Icao { get; private set; }
    public bool NoVehicle { get; private set; } public string? Vehicle { get; private set; } public string? Repaint { get; private set; } public string? Hof { get; private set; } public string? Fleet { get; private set; } public string? Registration { get; private set; }
    public int StartupTimeout { get; private set; } = 180; public int ShutdownTimeout { get; private set; } = 30; public int ObserveSeconds { get; private set; } = 8; public bool ObserveSecondsSpecified { get; private set; } public Dictionary<string, string> Settings { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static CliInput Parse(string[] args)
    {
        var output = new CliInput();
        foreach (var raw in args)
        {
            if (raw.StartsWith("--", StringComparison.Ordinal) && raw.Contains('='))
            {
                var argument = raw[2..].Split('=', 2);
                output.RuntimeArguments[argument[0]] = argument[1];
                continue;
            }
            if (!raw.StartsWith('/') && !raw.StartsWith('-'))
            {
                if (output.Command is null && raw is "capabilities" or "profiles" or "detect" or "help" or "session" or "events" or "time" or "weather" or "map" or "camera" or "vehicles" or "player" or "humans" or "timetable" or "scripts" or "constants" or "curves" or "hof" or "drivers" or "tickets" or "d3d") { output.Command = raw; continue; }
                if (output.Command is not null && output.Installation is null) output.CommandWords.Add(raw);
                else if (output.Installation is null) output.Installation = raw;
                else output.CommandWords.Add(raw);
                continue;
            }
            var pair = raw.TrimStart('/', '-').Split(':', 2); var key = pair[0].ToLowerInvariant(); var value = pair.Length == 2 ? pair[1] : null;
            switch (key)
            {
                case "?": case "help": output.Help = true; break; case "version": output.Version = true; break; case "json": output.JsonOutput = true; break; case "quiet": output.Quiet = true; break; case "verbose": output.Verbose = true; break; case "serve": output.Serve = true; break; case "log": output.Log = true; break; case "logall": output.LogAll = true; break; case "omsi-logall": output.OmsiLogAll = true; break; case "trace": case "trace-process": output.TraceProcess = true; break; case "trace-plugin": output.TracePlugin = true; break; case "trace-native": output.TraceNative = true; break; case "plan": output.PlanOnly = true; break; case "validate": output.ValidateOnly = true; break; case "runtime-batch": output.RuntimeBatch = true; break;
                case "runtime-write-batch": output.RuntimeWriteBatch = true; break;
                case "exe": output.Executable = value ?? throw new ArgumentException("/exe requires the full path to Omsi.exe"); break;
                case "splash": output.Splash = Enum.Parse<SplashMode>(value ?? throw new ArgumentException("/splash requires Unset, Native, or Managed"), true); output.SplashSpecified = true; break;
                case "splash-language": output.SplashLanguage = value ?? throw new ArgumentException("/splash-language requires a locale"); break;
                case "splash-assets": output.SplashAssets = value ?? throw new ArgumentException("/splash-assets requires a directory"); break;
                case "internet-textures": output.InternetTextures = Enum.Parse<InternetTexturesMode>(value ?? throw new ArgumentException("/internet-textures requires Native, Disabled, or Override"), true); break;
                case "internet-textures-profile": output.InternetTexturesProfile = value ?? throw new ArgumentException("/internet-textures-profile requires an .itx path"); break;
                case "d3d-batch": output.D3DBatch = true; break;
                case "runtime": output.RuntimeOperation = value ?? throw new ArgumentException("/runtime requires an operation"); break;
                case "runtime-arg": var runtimeArgument = value!.Split('=', 2); if (runtimeArgument.Length != 2) throw new ArgumentException("/runtime-arg requires key=value"); output.RuntimeArguments[runtimeArgument[0]] = runtimeArgument[1]; break;
                case "new": output.LaunchRequested = true; output.WorldMode = WorldMode.NewMap; break; case "saved": output.LaunchRequested = true; output.WorldMode = WorldMode.SavedSituation; output.Situation = value; break; case "last": output.LaunchRequested = true; output.WorldMode = WorldMode.LastMapState; break;
                case "map": output.Map = value; break; case "entrypoint": output.EntrypointIdentity = value; break; case "entrypoint-index": output.EntrypointIndex = int.Parse(value!); break;
                case "date": output.Date = value; break; case "time": output.Time = value; break; case "year": output.Year = value; break;
                case "weather": output.WeatherMode = WeatherMode.Preset; output.Weather = value; break; case "weather-icao": output.WeatherMode = WeatherMode.Icao; output.Icao = value; break; case "weather-real": output.WeatherMode = WeatherMode.RealCurrent; break;
                case "vehicle": output.Vehicle = value; break; case "repaint": output.Repaint = value; break; case "hof": output.Hof = value; break; case "fleet": output.Fleet = value; break; case "registration": output.Registration = value; break; case "no-vehicle": output.NoVehicle = true; break;
                case "set": var setting = value!.Split('=', 2); if (setting.Length != 2) throw new ArgumentException("/set requires key=value"); output.Settings[setting[0]] = setting[1]; break;
                case "spec": output.SpecFile = value; break; case "list": output.List = value; break; case "vehicle-scope": output.VehicleScope = value; break;
                case "startup-timeout": output.StartupTimeout = int.Parse(value!); break; case "shutdown-timeout": output.ShutdownTimeout = int.Parse(value!); break; case "observe-seconds": output.ObserveSeconds = int.Parse(value!); output.ObserveSecondsSpecified = true; break;
                case "recovery-status": output.Recovery = true; break; case "recover": output.Recovery = true; output.Recover = true; break; default: throw new ArgumentException("Unknown argument: " + raw);
            }
        }
        output.ResolveHierarchicalCommand();
        return output;
    }

    private void ResolveHierarchicalCommand()
    {
        if (RuntimeOperation is not null || CommandWords.Count == 0) return;
        if (Command is "session" or "events" or "help") return;
        var words = Command is null ? CommandWords : new[] { Command }.Concat(CommandWords).ToList();
        var route = string.Join(' ', words.Select(x => x.ToLowerInvariant()));
        RuntimeOperation = route switch
        {
            "time get" => "time.read", "time set" => "time.set",
            "weather get" => "weather.read", "weather set" => "weather.set", "weather actual get" => "weather.actual.read",
            "map get" => "map.read", "camera get" => "camera.read", "camera set" => "camera.set",
            "vehicles list" => "road-vehicles.list", "vehicles get" => "road-vehicle.read", "vehicles place-random" => "road-vehicles.place-random",
            "player get" => "player-vehicle.read", "humans list" => "humans.list", "humans get" => "human.read",
            "timetable get" => "timetable.read", "timetable tracks list" => "timetable.tracks.list", "timetable trips list" => "timetable.trips.list", "timetable lines list" => "timetable.lines.list", "timetable tours list" => "timetable.tours.list", "timetable profiles list" => "timetable.profiles.list", "timetable bus-stops list" => "timetable.bus-stops.list", "timetable station-links list" => "timetable.station-links.list", "timetable logs list" => "timetable.logs.read", "drivers list" => "drivers.read", "tickets get" => "tickets.read",
            "hof get" => "vehicle.hofs.read", "constants list" => "vehicle.constants.list", "constants get" => "vehicle.constant.get",
            "curves list" => "vehicle.curves.list", "curves evaluate" => "vehicle.curve.evaluate",
            "scripts variable list" => "vehicle.variables.list", "scripts variable get" => "vehicle.variable.get", "scripts variable set" => "vehicle.variable.set",
            "scripts string list" => "vehicle.string-variables.list", "scripts string get" => "vehicle.string-variable.get",
            "d3d status" => "d3d.status", "d3d texture create" => "d3d.texture.create", "d3d texture describe" => "d3d.texture.describe", "d3d texture update" => "d3d.texture.update", "d3d texture release" => "d3d.texture.release",
            _ => throw new ArgumentException("Unknown public command route: " + route)
        };
    }

    public async Task<LaunchSpec> BuildSpecAsync()
    {
        LaunchSpec seed = Defaults(Installation ?? AppContext.BaseDirectory);
        if (SpecFile is not null) await using (var stream = File.OpenRead(SpecFile)) seed = (await JsonSerializer.DeserializeAsync<LaunchSpec>(stream, Json)) ?? throw new InvalidDataException("Invalid LaunchSpec JSON.");
        var installationRoot = Installation ?? seed.Installation.RootPath;
        // A portable Release example uses RootPath=".". It means the directory
        // containing OmsiLaunch.exe, not the caller's arbitrary working folder.
        if (installationRoot == ".") installationRoot = AppContext.BaseDirectory;
        installationRoot = Path.GetFullPath(installationRoot);

        // Standalone/manual installations do not need Steam discovery. When an
        // explicit executable is supplied, derive the OMSI root from that file
        // and keep the existing exact-build validation in SessionPlanner.
        var explicitExecutable = Executable;
        if (!string.IsNullOrWhiteSpace(explicitExecutable))
        {
            var executablePath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(explicitExecutable));
            if (!File.Exists(executablePath)) throw new FileNotFoundException("OL_E_OMSI_EXECUTABLE_NOT_FOUND: " + executablePath, executablePath);
            if (!Path.GetFileName(executablePath).Equals("Omsi.exe", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("/exe must point to Omsi.exe.");

            var executableRoot = Path.GetDirectoryName(executablePath)
                ?? throw new ArgumentException("/exe does not contain a valid installation directory.");

            if (!string.IsNullOrWhiteSpace(Installation) &&
                !Path.GetFullPath(installationRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Equals(Path.GetFullPath(executableRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The positional installation root and /exe point to different OMSI installations.");

            installationRoot = Path.GetFullPath(executableRoot);
        }
        var vehicle = NoVehicle ? OptionalValue<PlayerVehicleSpec>.Unset : VehicleSpec(seed.PlayerVehicle);
        var settings = new Dictionary<string, OptionalValue<string>>(seed.Environment.General, StringComparer.OrdinalIgnoreCase);
        foreach (var pair in Settings)
        {
            if (!ConfigurationCatalog.TryGet(pair.Key, out var setting)) throw new ArgumentException("OL_E_UNKNOWN_SETTING: " + pair.Key);
            if (!setting.Writable) throw new InvalidOperationException("OL_E_SETTING_NOT_WRITABLE: " + pair.Key);
            settings[pair.Key] = OptionalValue<string>.Set(pair.Value);
        }
        var entrypointIdentity = Text(EntrypointIdentity, seed.World.EntrypointIdentity);
        var entrypointIndex = EntrypointIdentity is not null
            ? OptionalValue<int>.Unset
            : EntrypointIndex is { } index ? OptionalValue<int>.Set(index) : seed.World.PresentedEntrypointIndex;
        if (WorldMode == WorldMode.SavedSituation && (Map is not null || EntrypointIndex is not null || EntrypointIdentity is not null)) throw new ArgumentException("SAVED_SITUATION derives map and position from the selected .osn; /map and /entrypoint are not valid with /saved.");
        var world = WorldMode == WorldMode.SavedSituation
            ? new WorldSpec(WorldMode.SavedSituation, OptionalValue<string>.Unset, Text(Situation, seed.World.SituationIdentity), OptionalValue<int>.Unset, OptionalValue<string>.Unset)
            : new WorldSpec(WorldMode, Text(Map, seed.World.MapIdentity), Text(Situation, seed.World.SituationIdentity), entrypointIndex, entrypointIdentity);
        return seed with { Installation = new InstallationSpec(installationRoot), World = world, Date = ParseDate(Date, seed.Date), Time = ParseTime(Time, seed.Time), Year = ParseYear(Year, seed.EffectiveYear), Weather = new(WeatherMode == WeatherMode.Unset ? seed.EffectiveWeather.Mode : WeatherMode, Text(Weather, seed.EffectiveWeather.Preset), Text(Icao, seed.EffectiveWeather.Icao)), PlayerVehicle = vehicle, Environment = seed.Environment with { General = settings }, Behavior = seed.Behavior with { StartupTimeoutSeconds = StartupTimeout, ShutdownTimeoutSeconds = ShutdownTimeout }, Presentation = new SessionPresentationSpec(SplashSpecified ? Splash : seed.EffectivePresentation.Splash, Text(SplashLanguage, seed.EffectivePresentation.Language), Text(SplashAssets, seed.EffectivePresentation.CustomAssetDirectory)), InternetTextures = new InternetTexturesSpec(InternetTextures, Text(InternetTexturesProfile, seed.EffectiveInternetTextures.OverrideProfilePath)), Diagnostics = new DiagnosticsSpec(Log || seed.EffectiveDiagnostics.Log, Verbose || LogAll || seed.EffectiveDiagnostics.Verbose, OmsiLogAll || seed.EffectiveDiagnostics.OmsiLogAll, TraceProcess || LogAll || seed.EffectiveDiagnostics.ProcessTrace, TracePlugin || LogAll || seed.EffectiveDiagnostics.PluginTrace, TraceNative || LogAll || seed.EffectiveDiagnostics.NativeTrace) };
    }

    public static void Write<T>(T value, bool json) => Console.WriteLine(json ? JsonSerializer.Serialize(value, Json) : value is SessionPlan plan ? $"Plan: {(plan.IsRunnable ? "READY" : "NOT RUNNABLE")} profile={plan.BuildProfileId}" : JsonSerializer.Serialize(value, Json));
    public static void WriteEnvelope(string command, object result, bool json) => Console.WriteLine(json ? JsonSerializer.Serialize(new { ok = true, command, protocol_version = PublicCapabilityRegistry.ProtocolVersion, result }, Json) : JsonSerializer.Serialize(result, Json));
    public static void WriteError(string command, string code, string message, bool json, string category = PublicErrorCategory.Runtime) => Console.WriteLine(json ? JsonSerializer.Serialize(new { ok = false, command, protocol_version = PublicCapabilityRegistry.ProtocolVersion, error = new { code, category, message } }, Json) : $"{code}: {message}");
    public static object HelpFor(string? family) => new
    {
        usage = Usage,
        product_version = "0.1.0-beta1",
        protocol_version = PublicCapabilityRegistry.ProtocolVersion,
        family = string.IsNullOrWhiteSpace(family) ? null : family,
        commands = PublicCapabilityRegistry.All
            .Where(x => x.Classification is PublicCapabilityClassification.PublicStableBeta or PublicCapabilityClassification.PublicExperimental)
            .Where(x => string.IsNullOrWhiteSpace(family) || x.Family.Equals(family, StringComparison.OrdinalIgnoreCase))
            .Select(x => new { x.CliRoute, x.Description, x.Classification, x.RuntimeValidation })
    };
    private static LaunchSpec Defaults(string root) { var none = new Dictionary<string, OptionalValue<string>>(); return new(new(root), new(WorldMode.NewMap, OptionalValue<string>.Set("maps\\Grundorf\\global.cfg"), OptionalValue<string>.Unset, OptionalValue<int>.Set(1)), new(DateTimeMode.Unset, OptionalValue<SemanticDate>.Unset), new(DateTimeMode.Unset, OptionalValue<SemanticTime>.Unset), OptionalValue<PlayerVehicleSpec>.Unset, new(none, none, none, none, none, none, none, none), new()); }
    private OptionalValue<PlayerVehicleSpec> VehicleSpec(OptionalValue<PlayerVehicleSpec> old) { if (Vehicle is null && Repaint is null && Hof is null && Fleet is null && Registration is null) return old; var current = old.IsSet ? old.Value! : new(OptionalValue<string>.Unset, OptionalValue<string>.Unset, OptionalValue<string>.Unset, OptionalValue<string>.Unset, OptionalValue<string>.Unset); return OptionalValue<PlayerVehicleSpec>.Set(new(Text(Vehicle, current.Model), Text(Repaint, current.Repaint), Text(Hof, current.Hof), Text(Fleet, current.FleetNumber), Text(Registration, current.Registration))); }
    private static OptionalValue<string> Text(string? value, OptionalValue<string> fallback) => value is null ? fallback : OptionalValue<string>.Set(value);
    private static DateSpec ParseDate(string? value, DateSpec fallback) { if (value is null) return fallback; if (value.Equals("system", StringComparison.OrdinalIgnoreCase)) return new(DateTimeMode.System, OptionalValue<SemanticDate>.Unset); var parsed = System.DateOnly.Parse(value); return new(DateTimeMode.Explicit, OptionalValue<SemanticDate>.Set(new(parsed.Year, parsed.Month, parsed.Day))); }
    private static TimeSpec ParseTime(string? value, TimeSpec fallback) { if (value is null) return fallback; if (value.Equals("system", StringComparison.OrdinalIgnoreCase)) return new(DateTimeMode.System, OptionalValue<SemanticTime>.Unset); var parsed = System.TimeOnly.Parse(value); return new(DateTimeMode.Explicit, OptionalValue<SemanticTime>.Set(new(parsed.Hour, parsed.Minute, parsed.Second))); }
    private static YearSpec ParseYear(string? value, YearSpec fallback) => value is null ? fallback : value.Equals("system", StringComparison.OrdinalIgnoreCase) ? new(DateTimeMode.System, OptionalValue<int>.Unset) : new(DateTimeMode.Explicit, OptionalValue<int>.Set(int.Parse(value)));
}

internal static class CliEventWatch
{
    public static async Task<int> RunAsync(CliInput input)
    {
        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) => { eventArgs.Cancel = true; cancellation.Cancel(); };
        long lastSequence = 0;
        try
        {
            while (!cancellation.IsCancellationRequested)
            {
                var response = await LocalControlPlane.TryRequestAsync(new(PublicCapabilityRegistry.ProtocolVersion, "session.events"), TimeSpan.FromMilliseconds(750));
                if (response is null)
                {
                    CliInput.WriteError("events watch", "OL_E_NO_ACTIVE_SESSION", "No active OmsiLaunch control instance was found.", input.JsonOutput);
                    return (int)PublicExitCode.NoActiveSession;
                }
                if (!response.Ok)
                {
                    CliInput.WriteError("events watch", response.ErrorCode ?? "OL_E_CONTROL_FAILED", response.Message ?? "Control request failed.", input.JsonOutput);
                    return (int)PublicExitCode.OperationRejected;
                }
                if (response.Result is JsonElement events && events.ValueKind == JsonValueKind.Array)
                {
                    foreach (var runtimeEvent in events.EnumerateArray())
                    {
                        if (!runtimeEvent.TryGetProperty("Sequence", out var sequence) || sequence.GetInt64() <= lastSequence) continue;
                        lastSequence = sequence.GetInt64();
                        CliInput.WriteEnvelope("events.watch", runtimeEvent, input.JsonOutput);
                    }
                }
                await Task.Delay(250, cancellation.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) { }
        return (int)PublicExitCode.Success;
    }
}

internal sealed record RuntimeBatchResult(string Operation, Guid SessionId, ulong RequestId, string Profile, bool Succeeded, string? ErrorCode, IReadOnlyDictionary<string, string>? Values, long LatencyMilliseconds, string ExecutionPath);
internal static class RuntimeBatch
{
    public static async Task<IReadOnlyList<RuntimeBatchResult>> ExecuteAsync(IOmsiLaunch launch, SessionHandle session, string profile, bool includeTimeWrite)
    {
        var results = new List<RuntimeBatchResult>(); ulong requestId = 1;
        foreach (var operation in new[] { "time.read", "map.read", "weather.read", "weather.actual.read", "camera.read", "road-vehicles.read", "player-vehicle.read", "humans.read", "timetable.read", "timetable.tracks.list", "timetable.trips.list", "timetable.lines.list", "timetable.rv-files.list", "timetable.track-entries.list", "timetable.bus-stops.list", "timetable.station-links.list", "timetable.tours.list", "timetable.profiles.list", "timetable.tour-entries.list", "drivers.read", "tickets.read", "timetable.logs.read" })
        {
            var started = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var result = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, requestId++, operation), TimeSpan.FromSeconds(5));
                results.Add(new RuntimeBatchResult(operation, session.SessionId, result.RequestId, profile, result.Succeeded, result.ErrorCode, result.Values, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            catch (Exception exception)
            {
                results.Add(new RuntimeBatchResult(operation, session.SessionId, requestId - 1, profile, false, exception.Message, null, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
        }
        var vehicleHandle = await ExecuteEntitySnapshotAsync("road-vehicles.list", "vehicle.0.handle", "road-vehicle.read", "handle");
        if (vehicleHandle is not null)
        {
            await ExecuteNamedValueAsync("vehicle.variables.list", "vehicle.variable.get", vehicleHandle);
            await ExecuteNamedValueAsync("vehicle.string-variables.list", "vehicle.string-variable.get", vehicleHandle);
            await ExecuteNamedValueAsync("vehicle.constants.list", "vehicle.constant.get", vehicleHandle);
            await ExecuteFirstCurveAsync(vehicleHandle);
            await ExecuteAsync("vehicle.hofs.read", new Dictionary<string, string> { ["handle"] = vehicleHandle }, "vehicle.hofs.read");
            if (includeTimeWrite)
            {
                await ExecuteAsync("vehicle.variable.set", new Dictionary<string, string> { ["handle"] = vehicleHandle, ["name"] = "Refresh_Strings", ["value"] = "1" }, "vehicle.variable.set");
                await ExecuteAsync("vehicle.variable.set.restore", new Dictionary<string, string> { ["handle"] = vehicleHandle, ["name"] = "Refresh_Strings", ["value"] = "0" }, "vehicle.variable.set");
            }
        }
        await ExecuteEntitySnapshotAsync("humans.list", "human.0.handle", "human.read", "handle");
        if (includeTimeWrite)
        {
            var baseline = results.SingleOrDefault(result => result.Operation == "time.read" && result.Succeeded)?.Values;
            if (baseline is null || !baseline.TryGetValue("minute", out var text) || !byte.TryParse(text, out var minute))
            {
                results.Add(new RuntimeBatchResult("time.set", session.SessionId, requestId++, profile, false, "OL_E_RUNTIME_BASELINE_UNAVAILABLE", null, 0, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop -> Native.x86"));
            }
            else
            {
                var changedMinute = (byte)((minute + 1) % 60);
                await ExecuteAsync("time.set", new Dictionary<string, string> { ["minute"] = changedMinute.ToString(System.Globalization.CultureInfo.InvariantCulture) });
                await ExecuteAsync("time.set.restore", new Dictionary<string, string> { ["minute"] = minute.ToString(System.Globalization.CultureInfo.InvariantCulture) }, "time.set");
            }

            var weather = results.SingleOrDefault(result => result.Operation == "weather.read" && result.Succeeded)?.Values;
            if (weather is null || !weather.TryGetValue("wind_speed", out var windText) || !float.TryParse(windText, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var wind))
            {
                results.Add(new RuntimeBatchResult("weather.set", session.SessionId, requestId++, profile, false, "OL_E_RUNTIME_BASELINE_UNAVAILABLE", null, 0, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            else
            {
                var changedWind = Math.Abs(wind - 1f) < 0.001f ? 0f : 1f;
                await ExecuteAsync("weather.set", new Dictionary<string, string> { ["wind_speed"] = changedWind.ToString(System.Globalization.CultureInfo.InvariantCulture) }, "weather.set");
                await ExecuteAsync("weather.set.restore", new Dictionary<string, string> { ["wind_speed"] = wind.ToString(System.Globalization.CultureInfo.InvariantCulture) }, "weather.set");
            }

            var camera = results.SingleOrDefault(result => result.Operation == "camera.read" && result.Succeeded)?.Values;
            if (camera is null || !camera.TryGetValue("field_of_view", out var fovText) || !float.TryParse(fovText, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var fov))
            {
                results.Add(new RuntimeBatchResult("camera.set", session.SessionId, requestId++, profile, false, "OL_E_RUNTIME_BASELINE_UNAVAILABLE", null, 0, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            else
            {
                var changedFov = Math.Abs(fov - 46f) < 0.001f ? 45f : 46f;
                await ExecuteAsync("camera.set", new Dictionary<string, string> { ["field_of_view"] = changedFov.ToString(System.Globalization.CultureInfo.InvariantCulture) }, "camera.set");
                await ExecuteAsync("camera.set.restore", new Dictionary<string, string> { ["field_of_view"] = fov.ToString(System.Globalization.CultureInfo.InvariantCulture) }, "camera.set");
            }
        }
        return results;

        async Task ExecuteAsync(string label, IReadOnlyDictionary<string, string> arguments, string operation = "time.set")
        {
            var started = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var result = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, requestId++, operation, arguments), TimeSpan.FromSeconds(5));
                results.Add(new RuntimeBatchResult(label, session.SessionId, result.RequestId, profile, result.Succeeded, result.ErrorCode, result.Values, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop -> Native.x86"));
            }
            catch (Exception exception)
            {
                results.Add(new RuntimeBatchResult(label, session.SessionId, requestId - 1, profile, false, exception.Message, null, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop -> Native.x86"));
            }
        }

        async Task<string?> ExecuteEntitySnapshotAsync(string listOperation, string handleKey, string readOperation, string argumentName)
        {
            var started = System.Diagnostics.Stopwatch.StartNew(); RuntimeCommandResult? list = null;
            try
            {
                list = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, requestId++, listOperation), TimeSpan.FromSeconds(5));
                results.Add(new RuntimeBatchResult(listOperation, session.SessionId, list.RequestId, profile, list.Succeeded, list.ErrorCode, list.Values, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            catch (Exception exception)
            {
                results.Add(new RuntimeBatchResult(listOperation, session.SessionId, requestId - 1, profile, false, exception.Message, null, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            if (list?.Succeeded != true || list.Values is null || !list.Values.TryGetValue(handleKey, out var handle)) return null;
            var readStarted = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var read = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, requestId++, readOperation, new Dictionary<string, string> { [argumentName] = handle }), TimeSpan.FromSeconds(5));
                results.Add(new RuntimeBatchResult(readOperation, session.SessionId, read.RequestId, profile, read.Succeeded, read.ErrorCode, read.Values, readStarted.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            catch (Exception exception)
            {
                results.Add(new RuntimeBatchResult(readOperation, session.SessionId, requestId - 1, profile, false, exception.Message, null, readStarted.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            return handle;
        }

        async Task ExecuteNamedValueAsync(string listOperation, string readOperation, string handle)
        {
            var started = System.Diagnostics.Stopwatch.StartNew(); RuntimeCommandResult? list = null;
            try
            {
                list = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, requestId++, listOperation, new Dictionary<string, string> { ["handle"] = handle }), TimeSpan.FromSeconds(5));
                results.Add(new RuntimeBatchResult(listOperation, session.SessionId, list.RequestId, profile, list.Succeeded, list.ErrorCode, list.Values, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            catch (Exception exception)
            {
                results.Add(new RuntimeBatchResult(listOperation, session.SessionId, requestId - 1, profile, false, exception.Message, null, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            }
            if (list?.Succeeded != true || list.Values is null || !list.Values.TryGetValue("name.0", out var name)) return;
            await ExecuteAsync(readOperation, new Dictionary<string, string> { ["handle"] = handle, ["name"] = name }, readOperation);
        }

        async Task ExecuteFirstCurveAsync(string handle)
        {
            var list = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, requestId++, "vehicle.curves.list", new Dictionary<string, string> { ["handle"] = handle }), TimeSpan.FromSeconds(5));
            results.Add(new RuntimeBatchResult("vehicle.curves.list", session.SessionId, list.RequestId, profile, list.Succeeded, list.ErrorCode, list.Values, 0, "host -> session mailbox -> PluginRuntime UI timer -> profiled Interop"));
            if (list.Succeeded && list.Values is not null && list.Values.TryGetValue("name.0", out var name)) await ExecuteAsync("vehicle.curve.evaluate", new Dictionary<string, string> { ["handle"] = handle, ["name"] = name, ["x"] = "0" }, "vehicle.curve.evaluate");
        }
    }

    public static void WriteArtifact(string root, Guid sessionId, string profile, IReadOnlyList<RuntimeBatchResult> results, bool includesWrite)
    {
        var directory = Path.Combine(root, ".omsilaunch", "diagnostics"); Directory.CreateDirectory(directory);
        var suffix = includesWrite ? "runtime-write-batch" : "runtime-read-batch";
        var path = Path.Combine(directory, sessionId.ToString("N") + "-" + suffix + ".json");
        File.WriteAllText(path, JsonSerializer.Serialize(new { session_id = sessionId, build_profile = profile, utc = DateTimeOffset.UtcNow, operations = results }, CliInput.Json));
    }
}

internal static class D3DValidationBatch
{
    public static async Task<IReadOnlyList<RuntimeBatchResult>> ExecuteAsync(IOmsiLaunch launch, SessionHandle session, string profile)
    {
        var results = new List<RuntimeBatchResult>();
        ulong requestId = 20_000;

        async Task<RuntimeCommandResult> ExecuteAsync(string operation, IReadOnlyDictionary<string, string>? arguments = null)
        {
            var started = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var result = await launch.ExecuteRuntimeAsync(session, new RuntimeCommand(session.SessionId, requestId++, operation, arguments), TimeSpan.FromSeconds(8));
                results.Add(new RuntimeBatchResult(operation, session.SessionId, result.RequestId, profile, result.Succeeded, result.ErrorCode, result.Values, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime main/render thread -> Native.x86 D3D9"));
                return result;
            }
            catch (Exception exception)
            {
                results.Add(new RuntimeBatchResult(operation, session.SessionId, requestId - 1, profile, false, exception.Message, null, started.ElapsedMilliseconds, "host -> session mailbox -> PluginRuntime main/render thread -> Native.x86 D3D9"));
                return new RuntimeCommandResult(session.SessionId, requestId - 1, false, exception.Message);
            }
        }

        async Task ExpectReleasedAsync(string operation, IReadOnlyDictionary<string, string> arguments)
        {
            var result = await ExecuteAsync(operation, arguments);
            if (result.Succeeded || !string.Equals(result.ErrorCode, "OL_E_D3D_RESOURCE_RELEASED", StringComparison.Ordinal))
                return;

            // A released opaque handle must be rejected; record this as a passed
            // validation assertion rather than treating the intended rejection as a batch failure.
            results[^1] = new RuntimeBatchResult(
                operation + ".released-rejection",
                session.SessionId,
                result.RequestId,
                profile,
                true,
                null,
                new Dictionary<string, string> { ["expected_error"] = result.ErrorCode! },
                results[^1].LatencyMilliseconds,
                "host -> session mailbox -> PluginRuntime main/render thread -> Native.x86 D3D9");
        }

        await ExecuteAsync("d3d.status");
        var first = await ExecuteAsync("d3d.texture.create", new Dictionary<string, string> { ["width"] = "8", ["height"] = "8", ["format"] = "A8R8G8B8", ["levels"] = "1" });
        if (first.Succeeded && first.Values is not null && first.Values.TryGetValue("handle", out var firstHandle))
        {
            await ExecuteAsync("d3d.texture.describe", new Dictionary<string, string> { ["handle"] = firstHandle, ["level"] = "0" });
            var fullPixels = Enumerable.Range(0, 64).SelectMany(index => new byte[] { (byte)index, (byte)(255 - index), 0x55, 0xFF }).ToArray();
            await ExecuteAsync("d3d.texture.update", new Dictionary<string, string> { ["handle"] = firstHandle, ["level"] = "0", ["x"] = "0", ["y"] = "0", ["width"] = "8", ["height"] = "8", ["pixels_base64"] = Convert.ToBase64String(fullPixels) });
            var rectPixels = Enumerable.Repeat(new byte[] { 0x10, 0x20, 0x30, 0xFF }, 4).SelectMany(value => value).ToArray();
            await ExecuteAsync("d3d.texture.update", new Dictionary<string, string> { ["handle"] = firstHandle, ["level"] = "0", ["x"] = "2", ["y"] = "3", ["width"] = "2", ["height"] = "2", ["pixels_base64"] = Convert.ToBase64String(rectPixels) });

            var second = await ExecuteAsync("d3d.texture.create", new Dictionary<string, string> { ["width"] = "4", ["height"] = "4", ["format"] = "X8R8G8B8", ["levels"] = "1" });
            await ExecuteAsync("d3d.texture.release", new Dictionary<string, string> { ["handle"] = firstHandle });
            await ExpectReleasedAsync("d3d.texture.describe", new Dictionary<string, string> { ["handle"] = firstHandle, ["level"] = "0" });
            await ExpectReleasedAsync("d3d.texture.release", new Dictionary<string, string> { ["handle"] = firstHandle });
            if (second.Succeeded && second.Values is not null && second.Values.TryGetValue("handle", out var secondHandle))
                await ExecuteAsync("d3d.texture.release", new Dictionary<string, string> { ["handle"] = secondHandle });
        }
        for (var cycle = 0; cycle < 3; cycle++)
        {
            var created = await ExecuteAsync("d3d.texture.create", new Dictionary<string, string> { ["width"] = "2", ["height"] = "2", ["format"] = "A8R8G8B8", ["levels"] = "1" });
            if (created.Succeeded && created.Values is not null && created.Values.TryGetValue("handle", out var handle))
                await ExecuteAsync("d3d.texture.release", new Dictionary<string, string> { ["handle"] = handle });
        }
        await ExecuteAsync("d3d.status");
        return results;
    }

    public static void WriteArtifact(string root, Guid sessionId, string profile, IReadOnlyList<RuntimeBatchResult> results)
    {
        var directory = Path.Combine(root, ".omsilaunch", "diagnostics");
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, sessionId.ToString("N") + "-d3d-wave-d-batch.json"), JsonSerializer.Serialize(new
        {
            session_id = sessionId,
            build_profile = profile,
            utc = DateTimeOffset.UtcNow,
            operations = results
        }, CliInput.Json));
    }
}
