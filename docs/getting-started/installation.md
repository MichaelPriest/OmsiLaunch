# Installation

`OmsiLaunch-0.1.0-beta1.zip` is extracted directly into the supported OMSI
installation root. It installs `OmsiLaunch.exe` and controller dependencies at
the root, and the permanent product-owned plugin closure under `plugins\`.

The `.omsilaunch\` directory is private OmsiLaunch state. It contains product
assets, diagnostics, session journals while recovery is pending, and the small
offline documentation/example set. Do not move plugin binaries into it.

Only `OmsiLaunch.*` files are product-owned. Third-party plugins remain outside
OmsiLaunch transaction ownership. The current supported executable profile is
`Omsi23004_692EBFBF`; an unknown OMSI executable is rejected before launch.


## Standalone or manually installed OMSI

Steam discovery is not required. If OMSI is installed outside a Steam library,
the CLI can be pointed directly at the executable:

```powershell
.\OmsiLaunch.exe /exe:"D:\Games\OMSI 2\Omsi.exe" /new /map:"maps\Grundorf\global.cfg" /entrypoint-index:1
```

The installation root is derived from the directory containing `Omsi.exe`.
The executable must still match an exact supported BuildProfile. This is
intentional: OmsiLaunch's plugin/runtime uses build-specific native addresses,
so accepting an unknown executable could make the runtime unsafe.

A positional installation root and `/exe` may be supplied together only when
both resolve to the same directory. OmsiLaunch starts the selected executable
directly through Win32 `CreateProcessW`; it does not need to start
`steam.exe` or use a `steam://` launch URI.
