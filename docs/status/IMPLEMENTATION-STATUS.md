# Implementation Status

Status: RUNTIME_NEW_MAP_PASS

| Component | State | Notes |
| --- | --- | --- |
| RuntimePlatform abstraction | STATICALLY_VALIDATED | Current Windows x64 provider is implemented. |
| Current Windows x64 provider | STATICALLY_VALIDATED | Validates platform before mutable runtime operations. |
| BuildProfile | STATICALLY_VALIDATED | `Omsi23004_692EBFBF` is fingerprint-scoped. |
| Native.x86 | RUNTIME_VALIDATED | v145 Win32 build; profile/original-byte guards exercised on the canonical path. |
| OmsiHook-derived interop foundation | STATICALLY_VALIDATED | x86 Delphi strings, dynamic arrays, object snapshots and explicit remote-allocation boundary; no upstream RPC protocol. |
| In-process plugin interop | STATICALLY_VALIDATED | `PluginRuntime` uses a profile-scoped in-process memory provider; external attach is not the normal session path. |
| Runtime command channel | STATICALLY_VALIDATED | Session-bound fixed-width/SHA-256 request-response mailbox dispatched by the OMSI UI-thread timer. |
| Runtime map/weather/camera readers | RUNTIME_VALIDATED | Canonical session validated time, map, full profiled weather scalar reads, actual weather and camera through the session-bound plugin channel. |
| Advanced map/tile graph | BLOCKED_PROFILE_LAYOUT | The upstream `OmsiMap.Kacheln` label at `Map+0x118` did not resolve to a valid Delphi dynamic-array header in three guarded Current sessions. The trial never writes and all sessions restored normally. |
| Runtime clock/scalar weather writes | RUNTIME_VALIDATED | `time.set` applied profiled `SetTime` with read-back; `weather.set` validated a profile-backed scalar whitelist and restoration. |
| Runtime camera FOV write | RUNTIME_VALIDATED | `camera.set` changed and restored FOV during the canonical session; camera family remains range-guarded. |
| Runtime vehicle/human/timetable summary | RUNTIME_VALIDATED | Canonical Grundorf batch returned road vehicle state, human count, and timetable manager array counts without exposing raw pointers. |
| Runtime detailed timetable telemetry | RUNTIME_VALIDATED | Profiled immutable snapshots returned tracks, trips and lines with their semantic names, paths, counts and assignment metadata through the session channel. |
| Runtime timetable stops and links | RUNTIME_VALIDATED | Profiled immutable snapshots returned bus-stop names, IDs, links and station-link endpoint metadata. `Nordspitze` was read live from the UTF-16 Delphi field. |
| Runtime timetable tours | RUNTIME_VALIDATED | Profiled nested Tour snapshots returned line affiliation, AI group/type, vehicle reservation metadata and TourEntry counts without exposing Delphi pointers. |
| Runtime timetable profiles | RUNTIME_VALIDATED | Profiled nested Profile snapshots returned trip affiliation, name, total time, stop-time and track-entry-time counts. |
| Runtime timetable tour entries | RUNTIME_VALIDATED | Profiled nested TourEntry snapshots returned semantic trip identity, trip/profile indices, timing and smooth-transition state. |
| Runtime timetable track entries | RUNTIME_VALIDATED | Bounded TrackEntry snapshots returned 91 live records with track affiliation, ID, tile/path, distances, validity, ordering and chrono metadata through the session channel. |
| Runtime timetable RVFiles | RUNTIME_VALIDATED | Profiled RVFile array returned a valid empty collection on Grundorf through the session channel. |
| Runtime timetable NoRVNumbers | UNRESOLVED | Upstream `raw:true` decoding conflicts with the observed `Omsi23004_692EBFBF` field layout. Removed from the automatic batch; it does not affect other timetable telemetry. |
| Runtime vehicle/human detailed telemetry | RUNTIME_VALIDATED | Session-scoped opaque handles and read-only snapshots returned coherent vehicle movement/controls/lighting/AI and human target/ticket/seat/station/AI state on Grundorf. No spatial or arbitrary entity writes are enabled. |
| Runtime script variable read | RUNTIME_VALIDATED | ANSI name tables and the instance `PublicVars` pointer array resolved 1,025 names and `Refresh_Strings=0` through an opaque vehicle handle. |
| Runtime script variable mutation | RUNTIME_VALIDATED | `vehicle.variable.set` resolves a live opaque handle and the profiled `PublicVars` slot by open string name, rejects non-finite values, and returns immediate read-back. `Refresh_Strings 0 -> 1 -> 0` passed with normal cleanup. |
| Runtime string-variable read | RUNTIME_VALIDATED | ANSI name tables plus Unicode string values resolved 27 names and `ident=GRN-V 30` through an opaque vehicle handle. Delphi replacement/refcount writes remain separate. |
| Wave A runtime metadata readers | RUNTIME_VALIDATED | HOF metadata, drivers, ticket packs and timetable logs passed in session `e33aae7e-450a-4c00-8c7a-5f550ac4263f`; constants and curves passed in `46260d64-a8e9-4ee9-a907-00e46b07a1ae`, all with normal cleanup. |
| Permanent plugin installation | STATICALLY_VALIDATED | The Release ZIP places the OmsiLaunch plugin closure directly in `plugins\\`. A session validates exact product hashes before launch; it never stages, snapshots, restores, removes, or claims third-party plugins. |
| PlaceRandomBus | RUNTIME_VALIDATED | Session `4e2321d4-4c09-428b-8760-d348205b7392` returned `2`, increased RoadVehicles from 2 to 4, then completed exact cleanup. It is distinct from make-basic and makes no PlayerVehicle claim. |
| Named triggers and string mutation | BLOCKED_STRING_OWNERSHIP | Production dispatch is intentionally withheld until Delphi-string allocation, assignment and lifetime can be closed without inheriting upstream leak/corruption risk. |
| OmsiHook domain operation catalog | STATICALLY_VALIDATED | Program, map, time, weather, vehicles, humans, timetable, camera, player and sound domains are semantic/profile-gated. |
| Wave D D3D device and textures | RUNTIME_VALIDATED | Exact-profile device acquisition, one owned QI reference, opaque session handles, create/describe/full and rect update/release, repeated release rejection, multiple resources, forced exit and relaunch cleanup passed. |
| Wave D D3D lifecycle/reset | IMPLEMENTED_NOT_RUNTIME_VALIDATED | Reset interception invalidates default-pool resources before the native call, advances generation and publishes ordered lifecycle transitions. Safe device-loss/Reset induction was unavailable, so loss/reset/repeated-cycle/race scenarios remain blocked rather than claimed. |
| DNNE adapter | STATICALLY_VALIDATED | Current adapter is separated from PluginRuntime. |
| PluginRuntime | RUNTIME_VALIDATED | Handoff consumption, synchronous headless arm and generic NEW_MAP execution reached gameplay. |
| Portable handoff | STATICALLY_VALIDATED | Fixed-width UTF-8/SHA-256 protocol v4 carries semantic entrypoint and saved-situation identities; v3 decode remains supported and round-trip/corruption tests pass. |
| Content discovery | STATICALLY_VALIDATED | Offline map/situation/vehicle/repaint/HOF discovery plus opaque hashed `[entrypoints]` records; entrypoint launch correlation remains separately gated. |
| Configuration | STATICALLY_VALIDATED | Session-scoped only; lossless no-op, negative flags, ranges, compound blocks, and vector preservation tests pass. |
| `.omsilaunch` installation directory | RELEASE_VALIDATED | Directory state is created lazily after lease acquisition for managed presentation. Persistent resources live under `assets`; diagnostics persist; the durable journal is removed after restore. It is not a file and never a plugin deployment destination. |
| Managed splash presentation | RELEASE_RUNTIME_VALIDATED | `Managed` is the default. Packaged PTB/ENG/DEU/FRA 640x480 24-bit assets live under `.omsilaunch/assets/splash` and transactionally overlay `GUI/NewSplashscreen_ENG.bmp` plus the resolved locale. Release sessions `388cf5d2-7310-4301-9313-e62c4c08a346` (default) and `794cae34-a2ac-4fcf-8981-f142ac14c308` (custom directory) reached gameplay and completed normal cleanup. `Unset` session `12a74b24-6c78-4863-8ffe-947651212eeb` reached gameplay with no planned GUI overlay. All three restored the original GUI state and removed the journal; permanent product plugins were not transaction participants. |
| Release package | RELEASE_VALIDATED | `tools/New-ReleasePackage.ps1 -Configuration Release` produces `OmsiLaunch-current.zip` with SHA-256 manifest, Release runtime closure, splash assets, `.omsilaunch` example, and no Debug paths in its manifest. |
| Transaction/recovery | RUNTIME_RECOVERY_VALIDATED | A stale `HANDOFF_CREATED` journal restored only session-owned artifacts and removed its journal. Owner validation requires a non-exited process, creation time, and executable path before recovery is withheld. |
| PlanSession | STATICALLY_VALIDATED | Canonical Grundorf plan resolves runtime artifacts read-only. |
| Headless Start | IMPLEMENTED / RUNTIME_VALIDATED | Native startup was armed synchronously and gameplay was the observed target state. |
| NEW_MAP | IMPLEMENTED / RUNTIME_VALIDATED | Grundorf, presented index 1 and Nordspitze Bauernhof completed through gameplay. |
| NEW_MAP entrypoint identity | RUNTIME_PARTIAL | Offline raw records have stable hashes. Runtime confirms Grundorf presented index `1` selects raw index `0` (`Nordspitze Bauernhof`), but native presented text/mapping remains unresolved. Public identity requests remain gated. |
| SAVED_SITUATION | RUNTIME_VALIDATED | `situations\\Baustelle Falkenseer Ch..osn` loaded Berlin-Spandau through the full profiled Start-form radio, selector, synchronization, and `Button1Click` sequence, reached gameplay, remained RUNNING for 8 seconds, then completed requested stop and normal restore. PlanSession still rejects `.osn` files whose declared map is absent before staging. |
| LAST_MAP_STATE | UNSUPPORTED_FOR_CURRENT_PROFILE | Exact native last-map restoration branch is not closed; no file-order fallback exists. |
| Explicit/system date-time | STATICALLY_PARTIAL | Not requested by canonical regression. |
| Player vehicle | STATICALLY_PARTIAL | Not requested by canonical regression. |
| Legacy NT6 implementation | FUTURE / N/A | Architectural boundary only. |
| Legacy XP implementation | FUTURE / N/A | Architectural boundary only. |

## Canonical Runtime Baseline

`RUNTIME_NEW_MAP_PASS`: public `StartSessionAsync` validated the permanent runtime installation and
portable handoff, reached `gameplay.entered` for `maps\Grundorf\global.cfg`
with presented entrypoint index `1` (`Nordspitze Bauernhof`), remained RUNNING
for eight seconds, then completed a requested stop and normal exact restore.

The Current installation lease uses a named Windows semaphore (maximum count
one), not a mutex: cleanup can occur on a different managed thread. The durable
journal and process identity remain the crash-recovery authority.

The default startup timeout is 180 seconds. This preserves a bounded failure
path while allowing native saved situations on large maps to complete their
legitimate pre-game loading work; callers can set a shorter explicit timeout.

## Release Presentation 001

Release build, unit/integration suites, packaged CLI `PlanSession`, package
manifest and resource validation passed. The packaged executable reports
product `OmsiLaunch`, product version `0.1.0`, protocol `0.1`, and uses the
official multi-resolution icon. A managed Release start created and hash-matched
all four persistent splash assets at `.omsilaunch/assets/splash`. The Release
default, custom and `Unset` sessions each reached gameplay then `Completed`.
Default/custom overlays and native preservation were selected by their public
LaunchSpec and validated by the emitted plans. All three restored the original
GUI state, removed the journal and retained the permanent product plugin
closure. The
absent-destination transaction behavior is separately covered by
`transaction.absent-overlay-restore`.
