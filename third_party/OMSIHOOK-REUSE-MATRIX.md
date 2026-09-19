# OmsiHook Reuse Matrix

Status: NORMATIVE PROVENANCE

Upstream: `space928/Omsi-Extensions` at `7687b6623f5f74b4419695257bd2a4eef54dd93e`, LGPL-3.0-only.

| Upstream area | Decision | OmsiLaunch destination | Notes |
| --- | --- | --- | --- |
| `Memory.cs` | ADAPT | `OmsiLaunch.Interop/OmsiMemoryPrimitives.cs`, `OmsiDelphiValues.cs` | Explicit x86 pointer and Delphi data semantics; no attach/RPC copied. |
| `OmsiObject.cs` | ADAPT | `OmsiMemoryPrimitives.cs` | `OmsiRemoteObject` is an address wrapper only. |
| `CustomAttributes.cs` | REFERENCE_ONLY | future explicit marshalling descriptors | Reflection attributes are not yet needed for a profile-gated surface. |
| `ReflectionCache.cs` | REFERENCE_ONLY | future descriptor codec | Do not import reflection-driven writes before field layout is profile-confirmed. |
| `OmsiStructs.cs` | REFERENCE_ONLY | profile-scoped native records | Candidate structs require size/packing crosscheck per profile. |
| `MemArray/*` | ADAPT | `OmsiDelphiValues.cs`, `OmsiObjectCollection.cs`, `OmsiPointerLists.cs`, `OmsiStringList.cs` | Read-only Delphi array, `TList`, and `OList` snapshots; no generic pointer replacement. |
| `OmsiGlobals.cs` | SELECTIVE_ADAPT | `OmsiLaunch.Builds.Omsi23004/Profile.cs` | Time, map, weather, camera, vehicle/human and timetable globals are profile metadata only. |
| `WrappedOmsiClasses/OmsiTime.cs` | ADAPT | `OmsiTimeAdapter.cs` | Read adapter uses exact-profile scalar locations; semantic mutation still requires native method validation. |
| `WrappedOmsiClasses/OmsiWeather.cs` | ADAPT | `OmsiRuntimeReaders.cs` | Read-only weather layout adapted under the exact profile; no generic object writes. |
| `WrappedOmsiClasses/OmsiMap.cs` | ADAPT | `OmsiRuntimeReaders.cs` | Read-only map layout; map loading remains the proven native bridge. |
| `WrappedOmsiClasses/OmsiActuWeather.cs`, `OmsiCamera.cs` | ADAPT | `OmsiRuntimeReaders.cs` | Read-only actual-weather and camera surfaces; camera-family evidence is profile scoped. |
| `WrappedOmsiClasses/OmsiProgMan.cs` | SELECTIVE_ADAPT | native operation service | Methods are profile symbols, not public API. |
| `WrappedOmsiClasses/OmsiRoadVehicle*.cs` | SELECTIVE_ADAPT | `OmsiRuntimeReaders` | Profiled collection and per-vehicle telemetry use opaque session handles; spatial writes remain excluded. `OmsiComplMapObjInst` variable/string-variable reads are runtime-validated with ANSI name tables and Unicode values. Numeric `SetVariable` is statically adapted through the profiled public-value slot with finite input and read-back; live validation awaits a harmless reversible target. |
| `WrappedOmsiClasses/OmsiHumanBeingInst.cs` | SELECTIVE_ADAPT | `OmsiRuntimeReaders` | Profiled read-only human snapshots cover target, seat, ticket, vehicle/station association and AI state; no generic mutation. |
| `WrappedOmsiClasses/OmsiTimeTableMan.cs` | SELECTIVE_ADAPT | `OmsiRuntimeReaders` | Read-only profiled Tracks, Trips, Lines, BusStops, StationLinks, nested Tours and TrackEntries snapshots are runtime-validated. Deeper records remain incremental profile work. |
| remaining `WrappedOmsiClasses/*` | REFERENCE_ONLY | capability inventory | Promote only with profile evidence and a consumer. |
| `OmsiRemoteMethods.cs` | SELECTIVE_ADAPT | Native.x86 operation contracts | Borland ABI semantics inform native exports; no upstream transport. |
| `OmsiHookRPCMethods.cs` | REFERENCE_ONLY | none | Upstream RPC is not OmsiLaunch startup architecture. |
| `OmsiHookInvoker/FunctionHooks.*` | SELECTIVE_ADAPT | `OmsiLaunch.Native.x86` | ABI/hook concepts only, profile-address routed. |
| `OmsiHookRPCPlugin/*` | REFERENCE_ONLY | none | DNNE/RPC queue architecture is not adopted. |
| `OmsiHookPlugin/*` | ADAPT_BUILD_CONCEPTS | `OmsiLaunch.Plugin` | Current DNNE adapter remains thin. |
| `DXHook.*`, `D3DTexture.cs` | SELECTIVE_ADAPT | `OmsiLaunch.Native.x86`, `D3DRuntimeApi` | Adapted slot/QI, dynamic default-pool texture and pitch-aware update semantics. Upstream extra AddRef, `IsTexture` leak, unsafe destructor and incomplete reset model were not copied. No upstream binary or RPC dependency. |
| UI/CLI/examples | EXCLUDE | none | No duplicate launcher architecture. |

Every `ADAPT` item remains LGPL-3.0-only and must retain this provenance when
expanded. `REFERENCE_ONLY` means it provides semantic evidence, not code or
addresses that may be copied without validation.
