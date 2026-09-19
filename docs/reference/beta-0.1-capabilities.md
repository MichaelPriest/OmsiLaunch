# OmsiLaunch Beta 0.1 Capability Catalog

Product version: `0.1.0`  
Control protocol: `0.1`  
Supported executable profile: `Omsi23004_692EBFBF` (`Omsi.exe` SHA-256 `692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243`). The profiled Steam LAA SHA-256 is accepted but remains pending Beta runtime field validation.

Status meanings:

- **Supported**: runtime-validated on the supported profile and appropriate for the Beta semantic API.
- **Experimental**: implemented and validated where stated, but profile-specific details or the API shape may evolve during Beta.
- **Partial**: useful subset is available; listed restrictions are material.
- **Not yet available**: intentionally absent from the public Beta contract.
- **Internal**: implementation/test primitive, not a public Beta commitment.

| Capability | Status | API stability | Runtime validation | Known limitation |
|---|---|---|---|---|
| Session planning, launch, status, wait, stop, close | Supported | Stable Beta | Pass | One exact OMSI profile only. |
| Transactional runtime/config staging and restoration | Supported | Stable Beta | Pass | All launch configuration changes are session-scoped. |
| NEW_MAP launch by presented entrypoint index | Supported | Stable Beta | Pass | Semantic entrypoint identity remains partial. |
| SAVED_SITUATION native dispatch | Experimental | Experimental | Pass | Tested saved content must resolve to installed map/content. |
| LAST_MAP_STATE | Not yet available | N/A | Not run | Never inferred from newest `.osn`. |
| Content discovery and PlanSession | Supported | Stable Beta | Offline pass | Discovery is profile/content dependent. |
| Time read and native time set | Supported | Stable Beta | Pass | Calendar/`SetActualDateTime` mutation is unavailable. |
| Weather read and whitelisted scalar write | Experimental | Experimental | Pass | Actual/ICAO configure/apply lifecycle is unavailable. |
| Actual-weather controller read | Experimental | Experimental | Pass | No public ICAO activation/refresh operation. |
| Basic map state read | Supported | Stable Beta | Pass | Advanced map metadata/tile graph fields are partial. |
| Camera read and bounded FOV write | Experimental | Experimental | Pass | Camera lifecycle/family semantics remain profile-gated. |
| RoadVehicle collection and detail snapshots | Experimental | Experimental | Pass | No arbitrary spatial relocation or ODE rebind. |
| PlayerVehicle read/index | Supported | Stable Beta | Pass | Null player vehicle is valid; deterministic headless assignment is unavailable. |
| Humans and timetable snapshots | Experimental | Experimental | Pass | Some advanced Current-build layouts remain partial. |
| Numeric vehicle script variables | Experimental | Experimental | Read/write pass | String-variable write is unavailable. |
| String-variable, constants, curves, HOF, drivers, tickets, logs read | Experimental | Experimental | Pass | Managed-string mutation and named triggers are unavailable. |
| `road-vehicles.place-random` | Experimental | Experimental | Pass | Native world mutation; return value is diagnostic, not identity. |
| Basic MakeVehicle primitive | Internal | Internal only | Pass | Does not create/assign PlayerVehicle. |
| D3D status and texture lifecycle | Experimental | Experimental | Pass | Device-lost/Reset lifecycle is only partially proven. |
| Runtime/session events | Partial | Experimental | Partial | D3D lost/reset/restored events lack real runtime proof. |
| Keyboard/controller/config overlays | Supported | Stable Beta | Offline/integration pass | Applied only for the session and restored byte-for-byte. |

All runtime handles are opaque and session-scoped. They are not OMSI pointers, cannot be reused after session termination, and can be rejected as stale or released.

OmsiLaunch does not require OmsiHook binaries, its RPC plugin, or its runtime files. OmsiHook remains an engineering oracle with documented LGPL provenance; it is not a runtime dependency.
