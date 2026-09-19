# Runtime Control

Status: NORMATIVE

OmsiLaunch runtime control is session-scoped. It is not OmsiHook RPC
compatibility mode and it is not an external-process attach architecture.

```
OmsiLaunch API/CLI -> live session -> runtime mailbox -> PluginRuntime
    -> OMSI UI-thread gateway -> profiled Interop / Native.x86 -> OMSI
```

The startup handoff remains startup-only. Runtime commands use a separate
versioned mailbox bound to the session GUID and its launched OMSI process.
Every request has a fixed-width request ID, UTF-8 payload, SHA-256 integrity
check, timeout and response identity validation. A mapping is created before
`CreateProcessW`, is supplied only through the child environment, and is
disposed on PluginFinalize, process exit, session cleanup, or failed startup.

`RuntimeCommand` is semantic data: operation name plus UTF-8 key/value
arguments. It contains no OMSI pointers, Win32 handles, DNNE objects, or CLR
object serialization. The plugin dispatches commands from its existing OMSI
UI-thread timer; IPC workers never call Delphi/Borland methods directly.

Runtime control is session-scoped. The CLI is a thin reference frontend over
`IOmsiLaunch.ExecuteRuntimeAsync`; it never attaches independently or embeds
OMSI-specific logic.

After a declarative launch request reaches `RUNNING`, a single semantic command
may be issued with:

```text
OmsiLaunch.Cli <installation> /new /map:maps\Grundorf\global.cfg \
  /entrypoint-index:1 /no-vehicle /runtime:time.read
OmsiLaunch.Cli <installation> /new /map:maps\Grundorf\global.cfg \
  /entrypoint-index:1 /no-vehicle /runtime:time.set /runtime-arg:minute=18
```

`/runtime-arg` repeats for additional semantic fields. Each command travels via
the session-bound request/response mailbox, is executed by `PluginRuntime` on
the OMSI UI timer, and is rejected when the session binding or request identity
does not match. The command is not a persistent configuration editor.

Validated commands for `Omsi23004_692EBFBF` are `time.read`, `time.set`,
`map.read`, `weather.read`, `weather.set`, `weather.actual.read`, `camera.read`,
`road-vehicles.read`, `road-vehicles.list`, `road-vehicle.read`,
`player-vehicle.read`, `vehicle.variables.list`, `vehicle.variable.get`,
`vehicle.string-variables.list`, `vehicle.string-variable.get`, `humans.read`,
`humans.list`, `human.read`, `timetable.read`, `timetable.tracks.list`,
`timetable.trips.list`, `timetable.lines.list`, `timetable.bus-stops.list`,
`timetable.station-links.list`, `timetable.tours.list`, and
`timetable.profiles.list`, `timetable.tour-entries.list`, `d3d.status`,
`d3d.texture.create`, `d3d.texture.describe`, `d3d.texture.update`, and
`d3d.texture.release`. All timetable operations are bounded and read-only.
Arguments for mutations are deliberately whitelisted by the profile adapter.

`timetable.track-entries.list` is a validated, bounded immutable snapshot of
each track entry's semantic path identity and timing-relevant metadata. The
canonical Grundorf batch returned 91 records through the session channel.

`timetable.rv-files.list` is a validated bounded snapshot. Grundorf's active
runtime returned a valid empty collection. `NoRVNumbers` is intentionally not
published: the pinned upstream `raw:true` interpretation of the candidate
field conflicts with the observed `Omsi23004_692EBFBF` layout, so it remains
profile-unresolved rather than exposing an unsafe read.

`vehicle.variable.set` is runtime-validated through the same profile-scoped channel.
It accepts an opaque `handle`, open-string `name`, and finite float `value`,
resolves the current public-variable slot, then returns an immediate read-back.

Profiled entity snapshots use opaque handles returned by `road-vehicles.list`
and `humans.list`, then consumed by `road-vehicle.read` and `human.read` with
`/runtime-arg:handle=<handle>`. A handle is valid only for its owning session
and is rejected if its native object is no longer present in the current
collection. It is not a pointer, list index, or persistent identity.

## D3D9 Advanced Control

The public typed surface in `D3DRuntimeApi` wraps the semantic D3D operations.
`D3DTextureHandle` contains a session tag and an internal device-generation
token; it is neither a COM pointer nor valid after release, Reset, device
replacement, process exit, or another session launch.

The plugin acquires the exact-profile candidate from `D3DDevice`, retains one
QI reference, and dispatches all texture calls from the existing OMSI
UI/main/render-owner timer. Textures are bounded dynamic `D3DPOOL_DEFAULT`
resources. Updates validate level, rectangle and payload, then use
`LockRect`/`UnlockRect` with pitch-aware row copies. The mailbox payload is
bounded to 48 KiB; larger texture uploads are expressed as multiple rectangle
updates.

The lifecycle observer publishes `d3d.ready`, `d3d.lost`, `d3d.resetting` and
`d3d.restored` into `SessionStatus.RuntimeEvents`. Reset invalidates all live
default-pool resources before invoking the original native method and advances
the device generation. Device-loss and Reset paths are implemented but are not
runtime-proven because Wave D did not have a safe producer for those native
transitions. `d3d.ready`, normal texture operations, StopSession, forced exit,
relaunch cleanup, released handles and cross-session stale handles are proven.
