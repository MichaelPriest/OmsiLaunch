using System.Collections.Concurrent;
using System.Text.Json;
using OmsiLaunch.Api;
using OmsiLaunch.Configuration;
using OmsiLaunch.Content;
using OmsiLaunch.Process;

namespace OmsiLaunch.Core;

public sealed record OmsiLaunchRuntimePaths(string PluginBuildDirectory, string NativeBridgePath);

public sealed class OmsiLaunchService : IOmsiLaunch
{
    private readonly IRuntimePlatform platform;
    private readonly SessionPlanner planner;
    private readonly OmsiLaunchRuntimePaths runtimePaths;
    private readonly ConcurrentDictionary<Guid, LiveSession> sessions = new();

    public OmsiLaunchService(IRuntimePlatform platform, OmsiLaunchRuntimePaths runtimePaths)
    {
        this.platform = platform; this.runtimePaths = runtimePaths; planner = new SessionPlanner(platform);
    }
    public async Task<SessionPlan> PlanSessionAsync(LaunchSpec spec, CancellationToken cancellationToken = default)
    {
        var plan = await planner.PlanAsync(spec, cancellationToken).ConfigureAwait(false);
        try
        {
            var artifacts = RuntimeArtifactSet.Load(runtimePaths.PluginBuildDirectory, runtimePaths.NativeBridgePath);
            return plan with { RuntimeArtifacts = artifacts.Artifacts.Select(x => x.DestinationRelativePath).Append("OmsiLaunch startup handoff v4").ToArray() };
        }
        catch (Exception exception) when (exception is FileNotFoundException or DirectoryNotFoundException)
        {
            var diagnostics = plan.Diagnostics.Append(new LaunchDiagnostic("OL_E_RUNTIME_ARTIFACT_MISSING", exception.Message)).ToArray();
            return plan with { Diagnostics = diagnostics, IsRunnable = false };
        }
    }
    public async Task<SessionHandle> StartSessionAsync(SessionPlan plan, CancellationToken cancellationToken = default)
    {
        if (!plan.IsRunnable) throw new InvalidOperationException("OL_E_PLAN_NOT_RUNNABLE");
        var session = new LiveSession(plan.SessionId); if (!sessions.TryAdd(plan.SessionId, session)) throw new InvalidOperationException("Duplicate session id.");
        try { await StartAsync(session, plan, cancellationToken).ConfigureAwait(false); return new SessionHandle(plan.SessionId); }
        catch { sessions.TryRemove(plan.SessionId, out _); throw; }
    }
    public Task<SessionStatus> GetStatusAsync(SessionHandle session, CancellationToken cancellationToken = default) => Task.FromResult(Get(session).Status());
    public async Task<SessionStatus> WaitForAsync(SessionHandle session, SessionState state, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var live = Get(session); using var timeoutSource = new CancellationTokenSource(timeout); using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);
        while (live.State != state && live.State != SessionState.Failed && live.State != SessionState.Completed) await Task.Delay(100, linked.Token).ConfigureAwait(false);
        return live.Status();
    }
    public Task StopAsync(SessionHandle session, CancellationToken cancellationToken = default) { Get(session).RequestStop(); return Task.CompletedTask; }
    public async Task CloseAsync(SessionHandle session, CancellationToken cancellationToken = default)
    {
        var live = Get(session);
        // Closing a consumer handle must never strand a live transaction. A caller that
        // closes early requests the normal stop path, then waits for supervisor-owned
        // process exit, restoration, and lease release before the session is forgotten.
        if (!live.IsTerminal) live.RequestStop();
        if (live.LifecycleTask is { } lifecycle) await lifecycle.WaitAsync(cancellationToken).ConfigureAwait(false);
        sessions.TryRemove(session.SessionId, out _);
    }
    public Task<RuntimeCommandResult> ExecuteRuntimeAsync(SessionHandle session, RuntimeCommand command, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var live = Get(session);
        if (command.SessionId != session.SessionId) throw new InvalidOperationException("OL_E_RUNTIME_SESSION_MISMATCH");
        if (live.State != SessionState.Running) throw new InvalidOperationException("OL_E_SESSION_NOT_RUNNING");
        return live.RequestRuntimeAsync(command, timeout, cancellationToken);
    }
    public Task<IReadOnlyList<Capability>> GetCapabilitiesAsync(InstallationSpec installation, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var platformInfo = platform.Detect(installation.RootPath);
        IReadOnlyList<Capability> capabilities = new[]
        {
            new Capability("runtime.current-windows-x64", platformInfo.CurrentPlatformSupported, "STATICALLY_VALIDATED"),
            new Capability("runtime.command-channel", true, "STATICALLY_VALIDATED"),
            new Capability("runtime.time.read", true, "RUNTIME_VALIDATED"),
            new Capability("runtime.map.read", true, "RUNTIME_VALIDATED"),
            new Capability("runtime.weather.read", true, "RUNTIME_VALIDATED"),
            new Capability("runtime.weather.actual.read", true, "RUNTIME_VALIDATED"),
            new Capability("runtime.camera.read", true, "RUNTIME_VALIDATED"),
            new Capability("runtime.camera.write", true, "RUNTIME_VALIDATED", "FOV read-back and restoration passed; family is constrained to 0..3."),
            new Capability("runtime.d3d.status", true, "RUNTIME_VALIDATED", "Exact-build device QI, cooperative state, single owned reference and main/render-thread execution passed."),
            new Capability("runtime.d3d.texture.create", true, "RUNTIME_VALIDATED", "Dynamic default-pool A8R8G8B8 and X8R8G8B8 resources passed."),
            new Capability("runtime.d3d.texture.update", true, "RUNTIME_VALIDATED", "Full and bounded rectangular pitch-aware updates returned S_OK."),
            new Capability("runtime.d3d.texture.describe", true, "RUNTIME_VALIDATED", "Level dimensions, format and level count passed."),
            new Capability("runtime.d3d.texture.release", true, "RUNTIME_VALIDATED", "Explicit release and deterministic repeated-release rejection passed."),
            new Capability("runtime.d3d.lifecycle.reset", true, "IMPLEMENTED_NOT_RUNTIME_VALIDATED", "IDirect3DDevice9::Reset is intercepted to invalidate default-pool resources before Reset; a safe real loss/reset transition has not yet been induced."),
            new Capability("runtime.road-vehicles.read", true, "RUNTIME_VALIDATED"),
            new Capability("internal.road-vehicles.make-basic", true, "RUNTIME_VALIDATED", "Internal-only profiled primitive: exact one-object RoadVehicles delta, profiled VMT validation, and normal session cleanup passed. It is not PlayerVehicle creation."),
            new Capability("runtime.road-vehicles.place-random", true, "RUNTIME_VALIDATED", "Profiled PlaceRandomBus call increased the road-vehicle collection from 2 to 4 and completed normal cleanup."),
            new Capability("runtime.vehicle.variable.write", true, "RUNTIME_VALIDATED", "Profiled named public-variable write and immediate read-back passed with Refresh_Strings 0 -> 1 -> 0."),
            new Capability("runtime.humans.read", true, "RUNTIME_VALIDATED"),
            new Capability("runtime.timetable.read", true, "RUNTIME_VALIDATED"),
            new Capability("runtime.timetable.track-entries.read", true, "RUNTIME_VALIDATED", "Bounded profile-scoped TrackEntry snapshots returned 91 records on the canonical Grundorf session."),
            new Capability("runtime.time.write", true, "RUNTIME_VALIDATED", "Validated clock scalar write, profiled SetTime and read-back."),
            new Capability("runtime.time.actual-date-time.write", false, "RELEASE_IF_CLOSED", "SetActualDateTime ABI and calendar semantics are not yet closed."),
            new Capability("runtime.weather.write", true, "RUNTIME_VALIDATED", "Validated profiled scalar whitelist with read-back; ICAO and weather assets remain separate."),
            new Capability("world.new-map", true, "RUNTIME_VALIDATED"),
            new Capability("world.presented-entrypoint", true, "RUNTIME_VALIDATED"),
            new Capability("world.entrypoint-identity", false, "RUNTIME_PARTIAL", "The native presented-list matcher works, but the raw map identity to presented-list correlation is not closed; raw labels can be duplicated."),
            new Capability("world.saved-situation", true, "RUNTIME_VALIDATED", "Profiled Start-form selection and Button1Click sequence loaded Berlin-Spandau, reached gameplay and completed normal restore."),
            new Capability("world.last-map-state", false, "UNSUPPORTED_FOR_CURRENT_PROFILE", "Exact OMSI native semantics are not closed."),
            new Capability("world.date.explicit", false, "STATICALLY_PARTIAL"), new Capability("world.date.system", false, "STATICALLY_PARTIAL"),
            new Capability("world.time.explicit", false, "STATICALLY_PARTIAL"), new Capability("world.time.system", false, "STATICALLY_PARTIAL"),
            new Capability("weather.preset", false, "STATICALLY_PARTIAL"), new Capability("weather.icao", false, "STATICALLY_PARTIAL"), new Capability("weather.real-current", false, "STATICALLY_PARTIAL"),
            new Capability("player-vehicle.model", false, "STATICALLY_PARTIAL"), new Capability("player-vehicle.repaint", false, "STATICALLY_PARTIAL"),
            new Capability("player-vehicle.hof", false, "STATICALLY_PARTIAL"), new Capability("player-vehicle.fleet-number", false, "STATICALLY_PARTIAL"), new Capability("player-vehicle.registration", false, "STATICALLY_PARTIAL"),
            new Capability("configuration.options.semantic", true, "STATICALLY_VALIDATED"), new Capability("input.keyboard.patch", true, "STATICALLY_VALIDATED"),
            new Capability("input.controller.active-ffscale", true, "STATICALLY_VALIDATED"), new Capability("input.controller.axis-buttons", false, "STATICALLY_PARTIAL"),
            new Capability("content.maps", true, "STATICALLY_VALIDATED"), new Capability("content.situations", true, "STATICALLY_VALIDATED"),
            new Capability("content.vehicles", true, "STATICALLY_VALIDATED"), new Capability("content.repaints", true, "STATICALLY_VALIDATED"),
            new Capability("content.hofs", true, "STATICALLY_VALIDATED"), new Capability("content.fleet-registration-sources", true, "STATICALLY_VALIDATED")
        };
        return Task.FromResult(capabilities);
    }
    public Task<IReadOnlyList<ContentIdentity>> DiscoverAsync(InstallationSpec installation, ContentQueryKind query, OptionalValue<string> scope = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var catalog = new FileSystemContentCatalog(installation.RootPath);
        IEnumerable<ContentIdentity> results = query switch
        {
            ContentQueryKind.Maps => catalog.EnumerateMaps().Select(x => new ContentIdentity(x.Identity, "map", x.DisplayName)),
            ContentQueryKind.Situations => catalog.EnumerateSituations().Select(x => new ContentIdentity(x.Identity, "situation", x.MapIdentity)),
            ContentQueryKind.Vehicles => catalog.EnumerateVehicles().Select(x => new ContentIdentity(x.Identity, "vehicle", x.DisplayName)),
            ContentQueryKind.Repaints when scope.IsSet => catalog.EnumerateRepaints(scope.Value!).Select(x => new ContentIdentity(x.Identity, "repaint", x.DisplayName)),
            ContentQueryKind.Hofs => catalog.EnumerateHofs().Select(x => new ContentIdentity(x.Identity, "hof")),
            ContentQueryKind.Entrypoints => scope.IsSet
                ? catalog.EnumerateEntrypoints(scope.Value!).Select(x => new ContentIdentity(x.Identity, "entrypoint", x.DisplayName))
                : throw new ArgumentException("Entrypoint discovery requires a map identity scope.", nameof(scope)),
            ContentQueryKind.FleetNumbers when scope.IsSet => catalog.EnumerateFleetNumbers(scope.Value!).Select(x => new ContentIdentity(x.SourceIdentity, "fleet-number")),
            ContentQueryKind.Registrations when scope.IsSet => catalog.EnumerateRegistrations(scope.Value!).Select(x => new ContentIdentity(x.Mode, "registration", x.Value)),
            ContentQueryKind.Addons => catalog.EnumerateAddons().Select(x => new ContentIdentity(x.Identity, "addon", x.DiscoveryConfidence)),
            _ => Array.Empty<ContentIdentity>()
        };
        return Task.FromResult<IReadOnlyList<ContentIdentity>>(results.ToArray());
    }

    private async Task StartAsync(LiveSession session, SessionPlan plan, CancellationToken cancellationToken)
    {
        var trace = new HostTrace(plan.Spec.Installation.RootPath, plan.SessionId); trace.Write("STARTSESSION_ENTER");
        if (plan.Spec.Behavior.StartupTimeoutSeconds is < 1 or > 600) throw new ArgumentOutOfRangeException(nameof(plan), "Startup timeout must be between 1 and 600 seconds.");
        InstallationLease? lease = null; FileConfigurationTransaction? transaction = null; CurrentStartupHandoffStore? handoff = null; CurrentTelemetryStore? telemetry = null; CurrentRuntimeCommandStore? runtime = null;
        try
        {
            trace.Write("PLAN_ACCEPTED"); session.Move(SessionState.AcquiringInstallationLock); lease = InstallationLease.Acquire(plan.Spec.Installation.RootPath); trace.Write("INSTALLATION_LEASE_ACQUIRED");
            session.Move(SessionState.RecoveringPreviousTransaction);
            var artifacts = RuntimeArtifactSet.Load(runtimePaths.PluginBuildDirectory, runtimePaths.NativeBridgePath);
            if (plan.Spec.EffectivePresentation.Splash == SplashMode.Managed) SessionVisualAssets.EnsureInstallationAssets(plan.Spec.Installation.RootPath);
            var visualPlan = SessionVisualAssets.Build(plan.Spec);
            artifacts.ValidateInstalled(plan.Spec.Installation.RootPath); trace.Write("PERMANENT_PLUGIN_INSTALLATION_VALIDATED");
            transaction = new FileConfigurationTransaction(plan.Spec.Installation.RootPath, BuildTransactionalOverlays(plan.Spec, visualPlan), visualPlan.Deletions, plan.SessionId); trace.Write("TRANSACTION_STARTED");
            if (await transaction.HasPendingRecoveryAsync(cancellationToken).ConfigureAwait(false)) await transaction.RestorePendingAsync(cancellationToken).ConfigureAwait(false);
            // A journal is authoritative for normal crash recovery. With no
            // journal left, remove only exact byte-identical artifacts from the
            // active product manifest; differing same-name files remain a
            // user-owned collision and are snapshotted/restored normally.
            if (plan.Spec.Behavior.SuppressStaleClosecheckWarning) RemoveStaleClosecheck(plan.Spec.Installation.RootPath, session);
            session.Move(SessionState.Snapshotting); session.Move(SessionState.ApplyingConfiguration); await transaction.ApplyAsync(cancellationToken).ConfigureAwait(false);
            session.Move(SessionState.DeployingRuntime); await transaction.MarkStateAsync(TransactionState.RuntimeDeployed, cancellationToken).ConfigureAwait(false); trace.Write("PERMANENT_RUNTIME_READY");
            handoff = CurrentStartupHandoffStore.Create(new StartupHandoff(plan.SessionId, plan.BuildProfileId, plan.Spec.World.Mode, plan.Spec.World.MapIdentity.Value ?? string.Empty, plan.Spec.World.PresentedEntrypointIndex.IsSet ? plan.Spec.World.PresentedEntrypointIndex.Value : -1, true, plan.Spec.PlayerVehicle.IsSet, plan.Spec.Date.Mode, plan.Spec.Time.Mode, plan.Spec.World.EntrypointIdentity.IsSet ? plan.Spec.World.EntrypointIdentity.Value! : string.Empty, plan.Spec.World.SituationIdentity.IsSet ? plan.Spec.World.SituationIdentity.Value! : string.Empty));
            telemetry = CurrentTelemetryStore.Create(plan.SessionId);
            runtime = CurrentRuntimeCommandStore.Create(plan.SessionId);
            session.Move(SessionState.CreatingStartupHandoff); await transaction.MarkStateAsync(TransactionState.HandoffCreated, cancellationToken).ConfigureAwait(false); trace.Write("HANDOFF_CREATED");
            var environment = new Dictionary<string, string> { ["OMSILAUNCH_SESSION_ID"] = plan.SessionId.ToString("D"), ["OMSILAUNCH_HANDOFF_NAME"] = handoff.Name, ["OMSILAUNCH_TELEMETRY_NAME"] = telemetry.Name, ["OMSILAUNCH_RUNTIME_CHANNEL"] = runtime.Name, ["OMSILAUNCH_INTERNET_TEXTURES_MODE"] = plan.Spec.EffectiveInternetTextures.Mode.ToString() };
            session.Move(SessionState.StartingProcess); trace.Write("PROCESS_CREATE_ENTER");
            var process = await platform.StartAsync(new StartupProcessRequest(Path.Combine(plan.Spec.Installation.RootPath, "Omsi.exe"), plan.Spec.Installation.RootPath, environment), plan.BuildProfileId, cancellationToken).ConfigureAwait(false);
            trace.Write("PROCESS_CREATE_RETURN", process.ProcessId.ToString()); session.Attach(process); trace.Write("PROCESS_OWNERSHIP_REGISTERED"); await transaction.RecordProcessAsync(process.Identity.ProcessId, process.Identity.CreationTimeUtc, process.Identity.ExecutablePath, cancellationToken).ConfigureAwait(false); trace.Write("JOURNAL_PROCESS_STARTED"); session.Move(SessionState.WaitingForPlugin);
            session.AttachRuntime(runtime); runtime = null;
            session.LifecycleTask = Task.Run(() => SuperviseAsync(session, plan, process, lease, transaction, handoff, telemetry, trace)); trace.Write("SUPERVISOR_RUN_TASK_STARTED");
        }
        catch (Exception exception)
        {
            trace.Write("STARTSESSION_CALL_EXCEPTION", exception.ToString());
            runtime?.Dispose(); telemetry?.Dispose(); handoff?.Dispose();
            if (transaction is not null) { try { await transaction.RestoreAsync(CancellationToken.None).ConfigureAwait(false); } catch { } }
            lease?.Dispose(); throw;
        }
    }

    private async Task SuperviseAsync(LiveSession session, SessionPlan plan, LaunchedProcess process, InstallationLease lease, FileConfigurationTransaction transaction, CurrentStartupHandoffStore handoff, CurrentTelemetryStore telemetry, HostTrace trace)
    {
        try
        {
            trace.Write("SUPERVISOR_ENTER");
            var deadline = DateTimeOffset.UtcNow.AddSeconds(plan.Spec.Behavior.StartupTimeoutSeconds);
            string? lastTelemetry = null;
            while (!platform.HasExited(process) && !session.StopRequested)
            {
                var value = telemetry.ReadLatest();
                if (!string.IsNullOrWhiteSpace(value) && !string.Equals(value, lastTelemetry, StringComparison.Ordinal))
                {
                    lastTelemetry = value;
                    trace.Write("TELEMETRY_RECEIVED", value);
                    ApplyTelemetry(session, value);
                }
                if (session.State == SessionState.Failed) break;
                if (session.State != SessionState.Running && DateTimeOffset.UtcNow >= deadline)
                {
                    session.Fail(session.PluginStarted ? "OL_E_STARTUP_TIMEOUT" : "OL_E_PLUGIN_NOT_LOADED", "The requested semantic startup state was not reached before the timeout.");
                    break;
                }
                await Task.Delay(100).ConfigureAwait(false);
            }
            if (platform.HasExited(process) && session.State != SessionState.Running && session.State != SessionState.Failed) session.Fail("OL_E_PROCESS_EXITED_EARLY", "OMSI exited before gameplay was entered.");
            if (!platform.HasExited(process)) platform.Terminate(process);
            await platform.WaitForExitAsync(process, CancellationToken.None).ConfigureAwait(false); await transaction.MarkStateAsync(TransactionState.ProcessExited).ConfigureAwait(false); session.Move(SessionState.ProcessExited);
        }
        catch (Exception exception) { trace.Write("SUPERVISOR_FAULT", exception.ToString()); session.Fail("OL_E_PROCESS_SUPERVISION", exception.Message); }
        finally
        {
            try { session.Move(SessionState.Restoring); await transaction.RestoreAsync().ConfigureAwait(false); session.Move(SessionState.CleaningRuntime); }
            catch (Exception exception) { session.Fail("OL_E_RESTORE_FAILED", exception.Message); }
            finally { trace.Write("PROCESS_HANDLE_DISPOSE"); process.Dispose(); handoff.Dispose(); telemetry.Dispose(); session.DisposeRuntime(); lease.Dispose(); if (session.State != SessionState.Failed) session.Move(SessionState.Completed); trace.Write("SUPERVISOR_COMPLETE", session.State.ToString()); }
        }
    }

    private static void ApplyTelemetry(LiveSession session, string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var name = root.GetProperty("name").GetString();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var data = root.TryGetProperty("data", out var rawData) && rawData.ValueKind == JsonValueKind.Object
                    ? rawData.EnumerateObject().ToDictionary(property => property.Name, property => property.Value.ToString(), StringComparer.Ordinal)
                    : new Dictionary<string, string>(StringComparer.Ordinal);
                session.AddRuntimeEvent(name, data);
            }
            switch (name)
            {
                case "plugin.started": session.Move(SessionState.PluginBootstrap); break;
                case "world.starting": session.Move(SessionState.StartingWorld); break;
                case "gameplay.entered": session.Move(SessionState.Running); break;
                case "plugin.handoff.invalid": session.Fail("OL_E_PLUGIN_PROTOCOL_MISMATCH", json); break;
                case "plugin.build.invalid": session.Fail("OL_E_BUILD_VALIDATION_FAILED", json); break;
                case "headless.arm.failed": session.Fail("OL_E_HEADLESS_ARM_FAILED", json); break;
                case "plugin.request.unsupported": session.Fail("OL_E_CAPABILITY_UNAVAILABLE", json); break;
                case "world.failed": session.Fail("OL_E_WORLD_START_FAILED", json); break;
                case "world.situation.failed": session.Fail("OL_E_SITUATION_LOAD_FAILED", json); break;
            }
        }
        catch (JsonException) { session.Fail("OL_E_PLUGIN_PROTOCOL_MISMATCH", "Invalid plugin telemetry."); }
    }
    private static IReadOnlyDictionary<string, byte[]> BuildTransactionalOverlays(LaunchSpec spec, SessionVisualAssets.Plan? visualPlan = null)
    {
        var overlays = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        foreach (var visual in (visualPlan ?? SessionVisualAssets.Build(spec)).Overlays) overlays.Add(visual.Key, visual.Value);
        var settings = spec.Environment.General.Concat(spec.Environment.Advanced).Concat(spec.Environment.Graphics)
            .Concat(spec.Environment.AdvancedGraphics).Concat(spec.Environment.Sound).Concat(spec.Environment.AiPassengers)
            .Concat(spec.Environment.Keyboard).Concat(spec.Environment.Controllers).Where(x => x.Value.IsSet).ToArray();
        foreach (var group in settings.GroupBy(x => ConfigurationCatalog.TryGet(x.Key, out var setting) ? setting.FileName : throw new InvalidOperationException("OL_E_UNKNOWN_SETTING: " + x.Key), StringComparer.OrdinalIgnoreCase))
        {
            var path = Path.Combine(spec.Installation.RootPath, group.Key);
            var bytes = overlays.TryGetValue(group.Key, out var staged) ? staged : File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>();
            foreach (var item in group)
            {
                var setting = ConfigurationCatalog.TryGet(item.Key, out var known) ? known : throw new InvalidOperationException("OL_E_UNKNOWN_SETTING: " + item.Key);
                if (!setting.Writable) throw new InvalidOperationException("OL_E_SETTING_NOT_WRITABLE: " + item.Key);
                bytes = ConfigurationCatalog.CreatePatch(item.Key, item.Value.Value!).Apply(bytes);
            }
            overlays[group.Key] = bytes;
        }
        return overlays;
    }
    private LiveSession Get(SessionHandle handle) => sessions.TryGetValue(handle.SessionId, out var value) ? value : throw new KeyNotFoundException("Unknown OmsiLaunch session.");
    private static void RemoveStaleClosecheck(string root, LiveSession session)
    {
        var path = Path.Combine(root, "closecheck");
        if (!File.Exists(path)) return;
        var bytes = File.ReadAllBytes(path);
        session.AddDiagnostic("closecheck.stale-removed", Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)));
        File.Delete(path);
        if (File.Exists(path)) throw new IOException("OL_E_CLOSECHECK_REMOVE_FAILED");
    }

    private sealed class LiveSession
    {
        private readonly object gate = new(); private readonly List<LaunchDiagnostic> diagnostics = new(); private readonly List<RuntimeEvent> runtimeEvents = new(); private long eventSequence; private CurrentRuntimeCommandStore? runtime; public Guid Id { get; } public SessionState State { get; private set; } = SessionState.Created; public bool StopRequested { get; private set; } public bool PluginStarted { get; private set; }
        public bool IsTerminal { get { lock (gate) return State is SessionState.Completed or SessionState.Failed; } } public Task? LifecycleTask { get; set; }
        public LiveSession(Guid id) => Id = id;
        public void Attach(LaunchedProcess process) { lock (gate) diagnostics.Add(new("process.started", process.ProcessId.ToString(), new Dictionary<string, string> { ["thread_id"] = process.ThreadId.ToString(), ["creation_utc"] = process.Identity.CreationTimeUtc.ToString("O") })); }
        public void AddDiagnostic(string code, string message) { lock (gate) diagnostics.Add(new(code, message)); }
        public void AddRuntimeEvent(string type, IReadOnlyDictionary<string, string> data) { lock (gate) { runtimeEvents.Add(new(type, DateTimeOffset.UtcNow, ++eventSequence, data)); if (runtimeEvents.Count > 256) runtimeEvents.RemoveAt(0); } }
        public void Move(SessionState state) { lock (gate) { if (State != SessionState.Failed && State != SessionState.Completed) { State = state; if (state == SessionState.PluginBootstrap) PluginStarted = true; } } }
        public void Fail(string code, string message) { lock (gate) { diagnostics.Add(new(code, message)); State = SessionState.Failed; } }
        public void RequestStop() { lock (gate) StopRequested = true; }
        public void AttachRuntime(CurrentRuntimeCommandStore value) { lock (gate) runtime = value; }
        public Task<RuntimeCommandResult> RequestRuntimeAsync(RuntimeCommand command, TimeSpan timeout, CancellationToken cancellationToken) { lock (gate) return runtime?.RequestAsync(command, timeout, cancellationToken) ?? Task.FromException<RuntimeCommandResult>(new InvalidOperationException("OL_E_RUNTIME_CHANNEL_CLOSED")); }
        public void DisposeRuntime() { lock (gate) { runtime?.Dispose(); runtime = null; } }
        public SessionStatus Status() { lock (gate) return new SessionStatus(Id, State, diagnostics.ToArray(), runtimeEvents.ToArray()); }
    }
}
