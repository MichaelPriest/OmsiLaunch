# Public API

Status: NORMATIVE

`OmsiLaunch.Api` is the product boundary. Consumers construct a semantic
`LaunchSpec`, call `PlanSessionAsync`, then call `StartSessionAsync` only for a
runnable plan. The CLI is a reference consumer of this API and contains no
separate OMSI implementation.

Public contracts never contain OMSI pointers, Win32 handles, DNNE types, CLR
objects, shared-memory details, or pointer-width-dependent values. Content
identities are canonical OMSI-relative paths. `GetCapabilitiesAsync` and a
`SessionPlan` expose the availability of build-profile-specific operations.

`StartSessionAsync` returns after session ownership, transactional staging and
process supervision are established. It does not promise gameplay. Consumers
wait for `SessionState.Running`; for headless launch requests this means the
host observed `gameplay.entered` from the in-process runtime.

`World.EntrypointIdentity` remains capability-gated until its structured raw
entrypoint-to-presented-list mapping is closed for the active profile. The
native adapter already rejects absent or ambiguous presented labels, but callers
use `PresentedEntrypointIndex` as the supported low-level selector meanwhile.

`DiscoverAsync` is read-only. Add-on discovery is inventory only: OmsiLaunch
does not activate, deactivate, modify entitlement, or alter Steam add-ons.

All configuration in a `LaunchSpec` is temporary session state. The public API
does not expose permanent configuration editing: every touched configuration
file is transaction-snapshotted, journaled, verified, and restored byte-for-byte.

## Session Presentation

`SessionPresentationSpec.Splash` is `Managed` by default. It selects the
OmsiLaunch default splash from `<installation>/.omsilaunch/assets/splash` and
temporarily overlays OMSI GUI splash files. `CustomAssetDirectory` selects a
session/project asset directory; relative paths resolve below the installation.
`Unset` (and the compatibility alias `Native`) explicitly preserves the native
OMSI splash and adds no GUI splash mutation. This presentation policy remains
session-scoped; `.omsilaunch` is product state, not a persistent edit to OMSI
configuration.

## D3D9 Advanced API

`D3DRuntimeApi` provides typed session extensions for device status and texture
create, describe, update and release. Public models are `D3DDeviceStatus`,
`D3DTextureHandle`, `D3DTextureDescription`, `D3DTextureUpdate`,
`D3DDeviceState`, `D3DTextureFormat`, and `D3DTextureResourceState`.

Handles are opaque, session-scoped values. Consumers cannot obtain a COM
pointer, OMSI address or Win32 handle. Released, stale-generation and
cross-session values fail with structured `OmsiRuntimeException` codes. Public
session status also exposes the bounded ordered `RuntimeEvents` collection,
which carries D3D lifecycle events with timestamp, sequence and semantic data.

The typed methods are wrappers over the same public runtime command channel:

```csharp
var status = await launch.GetD3DStatusAsync(session, TimeSpan.FromSeconds(5));
var texture = await launch.CreateD3DTextureAsync(
    session, 64, 64, D3DTextureFormat.A8R8G8B8);
await launch.UpdateD3DTextureAsync(session, texture.Handle,
    new D3DTextureUpdate(0, 0, 0, 64, 16, pixels));
await launch.ReleaseD3DTextureAsync(session, texture.Handle);
```

Calls are valid only while the owning session is `RUNNING` and the matching
BuildProfile is active. Reset does not recreate or retarget a handle.
