namespace OmsiLaunch.Api;

// Canonical Beta control-surface inventory. Internal implementation details and
// raw native operations are intentionally not published here.
public enum PublicCapabilityClassification : byte
{
    PublicStableBeta,
    PublicExperimental,
    InternalOnly,
    Unsupported
}

public enum PublicCapabilityKind : byte { Read, Write, Action, Event }

public sealed record PublicCapabilityDescriptor(
    string Id,
    string Family,
    PublicCapabilityClassification Classification,
    PublicCapabilityKind Kind,
    bool RequiresSession,
    bool RequiresExactProfile,
    string ApiRoute,
    string CliRoute,
    string RuntimeValidation,
    string Description,
    IReadOnlyList<string>? HandleTypes = null);

public static class PublicCapabilityRegistry
{
    public const string ProtocolVersion = "0.1";

    public static readonly IReadOnlyList<PublicCapabilityDescriptor> All = new PublicCapabilityDescriptor[]
    {
        new("session.plan", "session", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Action, false, true, "session.plan", "session plan", "RUNTIME_PASS", "Compile a LaunchSpec without starting OMSI."),
        new("session.start", "session", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Action, false, true, "session.start", "session start", "RUNTIME_PASS", "Start a transactional managed OMSI session."),
        new("session.status", "session", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, false, "session.status", "session status", "RUNTIME_PASS", "Read the semantic lifecycle state."),
        new("session.stop", "session", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Action, true, false, "session.stop", "session stop", "RUNTIME_PASS", "Request normal OMSI shutdown and exact restore."),
        new("session.recover", "session", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Action, false, true, "session.recover", "recover", "RUNTIME_PASS", "Recover a stale durable transaction."),
        new("time.read", "time", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.time.read", "time get", "RUNTIME_PASS", "Read OMSI clock and calendar fields."),
        new("time.set", "time", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Write, true, true, "runtime.time.set", "time set", "RUNTIME_PASS", "Set the OMSI clock through the profiled native operation."),
        new("weather.read", "weather", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.weather.read", "weather get", "RUNTIME_PASS", "Read profiled weather state."),
        new("weather.set", "weather", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Write, true, true, "runtime.weather.set", "weather set", "RUNTIME_PASS", "Set individually supported weather scalars."),
        new("weather.actual.read", "weather", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Read, true, true, "runtime.weather.actual.read", "weather actual get", "RUNTIME_PASS", "Read actual/ICAO weather-controller state."),
        new("map.read", "map", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.map.read", "map get", "RUNTIME_PASS", "Read stable map identity and load state."),
        new("camera.read", "camera", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.camera.read", "camera get", "RUNTIME_PASS", "Read camera family and supported fields."),
        new("camera.set", "camera", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Write, true, true, "runtime.camera.set", "camera set", "RUNTIME_PASS", "Set supported camera scalars such as FOV."),
        new("vehicles.list", "vehicles", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.road-vehicles.list", "vehicles list", "RUNTIME_PASS", "List opaque RoadVehicle handles.", new[] { "RoadVehicle" }),
        new("vehicles.get", "vehicles", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.road-vehicle.read", "vehicles get", "RUNTIME_PASS", "Read an opaque RoadVehicle snapshot.", new[] { "RoadVehicle" }),
        new("vehicles.place-random", "vehicles", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Action, true, true, "runtime.road-vehicles.place-random", "vehicles place-random", "RUNTIME_PASS", "Invoke profiled PlaceRandomBus."),
        new("player.read", "player", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.player-vehicle.read", "player get", "RUNTIME_PASS", "Read the active PlayerVehicle or semantic null."),
        new("humans.list", "humans", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Read, true, true, "runtime.humans.list", "humans list", "RUNTIME_PASS", "List opaque human handles.", new[] { "Human" }),
        new("humans.get", "humans", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Read, true, true, "runtime.human.read", "humans get", "RUNTIME_PASS", "Read a human snapshot.", new[] { "Human" }),
        new("timetable.read", "timetable", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.timetable.read", "timetable get", "RUNTIME_PASS", "Read timetable manager state."),
        new("scripts.numeric", "scripts", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Write, true, true, "runtime.vehicle.variable.*", "scripts variable", "RUNTIME_PASS", "List, read and write numeric script variables.", new[] { "RoadVehicle" }),
        new("scripts.string.read", "scripts", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Read, true, true, "runtime.vehicle.string-variable.*", "scripts string", "RUNTIME_PASS", "List and read string script variables.", new[] { "RoadVehicle" }),
        new("constants", "constants", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.vehicle.constant.*", "constants", "RUNTIME_PASS", "List and read vehicle constants.", new[] { "RoadVehicle" }),
        new("curves", "curves", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.vehicle.curve.*", "curves", "RUNTIME_PASS", "List and evaluate vehicle curves.", new[] { "RoadVehicle" }),
        new("hof.read", "hof", PublicCapabilityClassification.PublicStableBeta, PublicCapabilityKind.Read, true, true, "runtime.vehicle.hofs.read", "hof get", "RUNTIME_PASS", "Read vehicle HOF metadata.", new[] { "RoadVehicle" }),
        new("drivers.read", "drivers", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Read, true, true, "runtime.drivers.read", "drivers list", "RUNTIME_PASS", "Read driver records."),
        new("tickets.read", "tickets", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Read, true, true, "runtime.tickets.read", "tickets get", "RUNTIME_PASS", "Read ticket-pack records."),
        new("d3d.texture", "d3d", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Action, true, true, "runtime.d3d.texture.*", "d3d texture", "RUNTIME_PASS", "Create, update, describe and release opaque D3D texture resources.", new[] { "D3DTexture" }),
        new("events.read", "events", PublicCapabilityClassification.PublicExperimental, PublicCapabilityKind.Event, true, false, "session.events", "events read|watch", "RUNTIME_PASS", "Read or observe bounded session runtime events."),
        new("internal.make-basic", "vehicles", PublicCapabilityClassification.InternalOnly, PublicCapabilityKind.Action, true, true, "internal.road-vehicles.make-basic", "", "RUNTIME_PASS", "Profiled research primitive without PlayerVehicle assignment semantics."),
        new("calendar.set-actual-date-time", "time", PublicCapabilityClassification.Unsupported, PublicCapabilityKind.Write, true, true, "", "", "UNSUPPORTED", "Native ABI and postconditions are not closed for Beta."),
        new("player.assign-headless", "player", PublicCapabilityClassification.Unsupported, PublicCapabilityKind.Action, true, true, "", "", "UNSUPPORTED", "Deterministic headless PlayerVehicle assignment is a future extension.")
    };

    // The control plane accepts concrete runtime operation IDs, while the
    // catalog intentionally groups some related operations behind a wildcard.
    // This explicit set prevents an inactive-session error from masking an
    // invalid public command name.
    private static readonly HashSet<string> PublicRuntimeOperations = new(StringComparer.Ordinal)
    {
        "time.read", "time.set", "weather.read", "weather.set", "weather.actual.read",
        "map.read", "camera.read", "camera.set", "road-vehicles.list", "road-vehicle.read",
        "road-vehicles.place-random", "player-vehicle.read", "humans.list", "human.read",
        "timetable.read", "timetable.tracks.list", "timetable.trips.list", "timetable.lines.list",
        "timetable.rv-files.list", "timetable.track-entries.list", "timetable.bus-stops.list",
        "timetable.station-links.list", "timetable.tours.list", "timetable.profiles.list",
        "timetable.tour-entries.list", "timetable.logs.read", "drivers.read", "tickets.read",
        "vehicle.hofs.read", "vehicle.variables.list", "vehicle.variable.get", "vehicle.variable.set",
        "vehicle.string-variables.list", "vehicle.string-variable.get", "vehicle.constants.list",
        "vehicle.constant.get", "vehicle.curves.list", "vehicle.curve.evaluate", "d3d.status",
        "d3d.texture.create", "d3d.texture.describe", "d3d.texture.update", "d3d.texture.release"
    };

    public static bool IsPublicRuntimeOperation(string operation) => PublicRuntimeOperations.Contains(operation);
}
