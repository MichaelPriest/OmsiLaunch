# `.omsilaunch` Installation State Directory

`.omsilaunch` is a directory below the OMSI installation root. It is never a
LaunchSpec file and is not deployed into `plugins`.

The Release ZIP is extracted directly into the OMSI root. `OmsiLaunch.exe` and
its controller assemblies remain at that root, while the permanent OMSI plugin
closure is installed directly under `plugins\\OmsiLaunch.*`. Those files belong
to the product installation: sessions validate their hashes but never stage,
snapshot, restore, remove, or use them as a temporary deployment source.

## Creation and ownership

The directory is created lazily. A managed-splash session creates
`<OMSI>/.omsilaunch/assets/splash` after acquiring the installation lease and
before the transaction is applied. Missing stock OmsiLaunch assets are copied
from the packaged `.omsilaunch/assets/splash` directory. Existing assets are never
overwritten, allowing an explicitly managed project asset set to persist.

The directory is OmsiLaunch-owned state. It is distinct from OMSI configuration
and from the permanent plugin installation.

## Layout

| Path | Lifetime | Purpose |
|---|---|---|
| `assets/splash/{PTB,ENG,DEU,FRA}.bmp` | Persistent OmsiLaunch resource | Default 640x480 24-bit managed splash assets. No release number is embedded. |
| `diagnostics/<session>-host.log` | Persistent diagnostic | Host semantic transition trace. |
| `diagnostics/*-runtime-*.json` | Persistent diagnostic | Explicit runtime command/batch evidence. |
| `journal.json` | Temporary durable transaction state | Exact snapshot, process identity and restoration progress. Removed after normal restore; retained only for recovery. |
| `*.omsilaunch.tmp` beside a touched OMSI file | Ephemeral | Atomic-write staging; removed or atomically replaced. |

Only temporary OMSI changes, such as `GUI/NewSplashscreen_*.bmp` and requested
configuration overlays, enter the snapshot/journal/restore transaction.
Third-party plugins are never enumerated for ownership, copied, removed, or
restored.

Startup handoff, telemetry, and runtime command mappings are named shared
memory objects. They are session/process-bound and are **not** files below this
directory.

## Splash selection

`SessionPresentationSpec.Splash` has three semantic modes:

- `Managed` (default): OmsiLaunch installs `ENG` plus the resolved language
  target in `GUI/NewSplashscreen_*.bmp` through the existing transaction. The
  original OMSI files are restored byte-for-byte after exit, failure, or
  recovery.
- `Native` or `Unset`: preserve OMSI splash files; no GUI splash overlay is
  created.

Language resolution accepts `PTB`, `ENG`, `DEU`, and `FRA`; when absent it uses
`options.cfg` and then falls back to `ENG`. `/splash-assets:<directory>` or
`Presentation.CustomAssetDirectory` selects an explicit custom directory. A
relative custom directory is resolved below the OMSI installation root. It must
contain a valid `ENG.bmp` and, for a non-English selected language, its matching
localized BMP. Assets must be 640x480, 24-bit BMP files.

Absent packaged/default assets, invalid asset format, or an invalid custom
directory fail planning/startup with a semantic splash error; OMSI is not
launched. A missing `.omsilaunch` directory itself is normal and is created as
described above.

## Release validation

`tools/Test-ReleasePresentation.ps1` validates the packaged Release executable
and can execute the three presentation cases against an authorized OMSI
installation. Its default pass is non-mutating and checks package assets plus
the three `PlanSession` outcomes. `-RunOmsi` additionally proves the temporary
overlay while OMSI runs, exact GUI restoration, and journal/process cleanup.
With `-RunOmsi -InstallPackage`, it copies only the package's product-owned
root files and `plugins/OmsiLaunch.*` files into the authorized test root, then
also verifies third-party plugin hashes remain unchanged. The result is
persisted as `diagnostics/release-presentation-validation.json`.

## Configuration files

LaunchSpec JSON is supplied via `/spec:<file.json>`. A distribution example is
`examples/.omsilaunch/canonical-session.json`; the nested directory is a sample
layout only. It may be copied under an installation's `.omsilaunch` directory,
but the configuration file is not required for the directory to function.
