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
