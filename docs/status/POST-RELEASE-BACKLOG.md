# Post-Release Backlog

Status: NORMATIVE

The first Current release must not expose unsafe generic pointer/list mutation.
The following work is intentionally post-release unless an exact profile-bound
semantic operation closes before the release gate:

- Safe runtime induction and validation of D3D9 device-loss/Reset cycles and a
  request racing Reset. Device status and texture create/update/query/release
  are already runtime-proven with opaque OmsiLaunch handles.
- Cross-tile/spatial/ODE vehicle rebind. Local transforms are not a correct
  teleport or an authority-replication primitive.
- High-frequency authoritative vehicle puppet control.
- Generic pointer/list writes and low-level ODE APIs.
- OmsiHook RPC compatibility mode.
- Legacy NT6 and XP ports, after `legacy-port-base` is frozen.
