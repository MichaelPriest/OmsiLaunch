# OmsiLaunch Beta 0.1 Known Limitations

## Compatibility

Beta 0.1 supports only `Omsi23004_692EBFBF`, identified by the exact `Omsi.exe` SHA-256 documented in the capability catalog. Unknown or unprofiled executables are rejected; Beta 0.1 does not claim generic OMSI 2 support.

## World loading and vehicles

- `LAST_MAP_STATE` is not implemented. OmsiLaunch never substitutes the newest saved situation for it.
- Presented entrypoint index is supported; semantic entrypoint identity remains partial.
- Basic native `MakeVehicle` and `PlaceRandomBus` are validated, but deterministic headless PlayerVehicle placement/assignment is not a supported public feature.
- Position fields are inspectable. Arbitrary vehicle relocation, cross-tile spatial rebinding, and ODE-safe transform authority are not supported.

## Runtime mutation

- Numeric script-variable writes are supported only through the profiled semantic path.
- String-variable writes, vehicle named triggers, and object sound triggers are unavailable pending a safe Delphi managed-string lifetime boundary.
- `SetActualDateTime` and explicit calendar mutation are unavailable pending ABI/postcondition closure.
- Actual/ICAO weather can be read, but configure/activate/refresh is unavailable.

## Advanced state

- Some Current-build advanced map/tile/path/spline/object graph fields are partial because the `Kacheln` representation is not fully reconciled.
- `NoRVNumbers` and selected detailed timetable representations remain partial.
- Advanced APIs are profile-gated typed snapshots, not a contract for arbitrary memory access or native pointer use.

## D3D

Texture create, describe, update, release, stale/released-handle rejection, forced-exit cleanup, and relaunch are runtime-proven. Real `DEVICELOST`, OMSI `Reset`, repeated loss/reset, request-versus-Reset races, and lost/reset/restored public-event proof remain experimental/partial.

## Session and configuration safety

All configuration and temporary OMSI-file changes are session-scoped. `options.cfg`, `keyboard.cfg`, `gamectrler.cfg`, splash overlays, and other temporary touched artifacts are snapshot/journal/restored. The permanent OmsiLaunch plugin closure under `plugins\\OmsiLaunch.*` is installed with the product and is not a transaction participant. Permanent configuration editing is outside Beta 0.1 scope.

## Presentation assets

`.omsilaunch` is an OmsiLaunch-owned installation directory, not a file or a
permanent OMSI configuration override. Its default splash resources persist as
product assets; GUI `NewSplashscreen_*.bmp` replacements are session-only and
restored. An interrupted host without a durable journal cannot safely infer
whether an arbitrary remaining GUI file is user-owned, so recovery deliberately
does not blindly delete it.
