# Beta 0.1 Documentation Manifest

This manifest is the source map for the Beta 0.1 documentation phase. It does not create a promise that every listed page already exists.

| Document | Purpose | Audience | Source of truth | Status |
|---|---|---|---|---|
| `README.md` | Product scope and supported build summary | All users | Root README, `IMPLEMENTATION-STATUS.md` | READY_TO_WRITE |
| `getting-started/installation.md` | Prerequisites, staging, and clean removal | Users | `RuntimeDeployment.cs`, transaction tests | READY_TO_WRITE |
| `getting-started/first-session.md` | Canonical Start/Running/Stop flow | Developers | `LaunchApi.cs`, CLI, runtime validation artifacts | READY_TO_WRITE |
| `getting-started/compatibility.md` | Exact-build policy | Users | `Profile.cs`, `BUILD-PROFILES.md` | READY_TO_WRITE |
| `concepts/architecture.md` | Host, plugin, Native.x86, profile boundaries | Developers | `PUBLIC-API.md`, architecture sources | READY_TO_WRITE |
| `concepts/omsilaunch-directory.md` | Installation-state directory, splash assets, lifecycle, and recovery | Users, developers | `SessionVisualAssets.cs`, transaction sources, Release validation | READY |
| `concepts/sessions.md` | Lifecycle, transaction, lease, recovery | Developers | `OmsiLaunchService.cs`, `RUNTIME-CONTROL.md` | READY_TO_WRITE |
| `concepts/build-profiles.md` | Profile and executable validation | Developers | `Profile.cs`, `BUILD-PROFILES.md` | READY_TO_WRITE |
| `concepts/handles.md` | Session-scoped opaque handles | Developers | `LaunchApi.cs`, D3D and runtime readers | READY_TO_WRITE |
| `concepts/errors.md` | Beta error taxonomy | Developers | runtime command results, `D3DRuntimeApi.cs` | READY_TO_WRITE |
| `concepts/semantic-vs-advanced-api.md` | Stable semantic versus profile-gated advanced access | Developers | `OmsiRuntimeSurface.cs`, capability catalog | READY_TO_WRITE |
| `api/overview.md` | Public API entry points | Developers | `IOmsiLaunch`, `LaunchApi.cs` | READY_TO_WRITE |
| `api/session.md` | Start, wait, stop, close, status | Developers | `IOmsiLaunch`, `LaunchApi.cs` | READY_TO_WRITE |
| `api/time.md`, `api/weather.md`, `api/map.md`, `api/camera.md` | Core runtime operations | Developers | `CurrentRuntimeControl.cs`, validation artifacts | READY_TO_WRITE |
| `api/vehicles.md`, `api/player-vehicle.md`, `api/humans.md`, `api/timetable.md` | Runtime entities and snapshots | Developers | `OmsiRuntimeReaders.cs`, runtime artifacts | READY_TO_WRITE |
| `api/scripts.md`, `api/hof.md`, `api/drivers.md`, `api/tickets.md` | Profile-gated runtime data | Developers | `CurrentRuntimeControl.cs`, Wave A-C reports | READY_TO_WRITE |
| `api/d3d.md`, `api/events.md`, `api/advanced.md` | Experimental advanced surfaces | Developers | `D3DRuntimeApi.cs`, Wave D report, runtime telemetry | READY_TO_WRITE |
| `guides/create-vehicle.md` | PlaceRandomBus and internal basic creation boundaries | Developers | runtime artifacts, MakeVehicle reports | NEEDS_CODE_CLOSURE |
| `guides/variables-and-triggers.md` | Numeric variables and unsupported string/trigger writes | Developers | Wave A closure report | READY_TO_WRITE |
| `guides/d3d-textures.md` | Texture lifecycle and reset limits | Developers | `D3DRuntimeApi.cs`, Wave D report | READY_TO_WRITE |
| `guides/diagnostics.md`, `guides/recovery.md` | Evidence collection and safe recovery | Users, developers | host trace, journal/recovery implementation | READY_TO_WRITE |
| `reference/beta-0.1-capabilities.md` | Definitive Beta capability catalog | Users, developers | current code and regression artifacts | READY |
| `localized/<locale>/` | Complete localized public-documentation mirror | Users, developers | Canonical English public documentation | LOCALIZED_PUBLIC_SURFACE |
| `reference/known-limitations.md` | Explicit Beta boundaries | Users, developers | readiness report and parity reports | READY |
| `reference/compatibility-matrix.md`, `reference/errors.md` | Build and error references | Developers | profile/capability/error sources | READY_TO_WRITE |
| `reference/omsihook-parity.md` | Engineering parity tracking | Engineers | `third_party/OMSIHOOK-REUSE-MATRIX.md`, Wave reports | INTERNAL_ONLY |
| `internals/native-interop.md`, `internals/build-profile-current.md` | Native ABI and Current-profile implementation | Maintainers | Interop/Native sources and research | INTERNAL_ONLY |
| `internals/transaction-model.md`, `internals/d3d-lifecycle.md` | Recovery and resource lifetime internals | Maintainers | Core/process/native sources and reports | INTERNAL_ONLY |

The full documentation-writing phase begins only after this manifest and the Beta readiness report are accepted.
