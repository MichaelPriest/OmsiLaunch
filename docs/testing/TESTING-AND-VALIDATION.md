# Testing And Validation

Session configuration tests require byte-identical restoration after success,
failure, stop, and stale-journal recovery. Non-runtime tests cover lossless
options, negative flags, vectors, ranges, compound blocks, keyboard custom
events, controller preservation, handoff integrity, transaction restore, and
the cross-thread installation lease.

`OmsiLaunch.Native.x86.vcxproj` is an owned v145 `Debug|Win32`/`Release|Win32`
project and is not built by the managed `.sln` invocation. Any runtime run or
package build that changes `NativeBoundary.cpp` must build that project
explicitly before staging; the deployment manifest consumes
`artifacts\x86\<Configuration>\OmsiLaunch.Native.x86.dll`.

Runtime validation is consolidated by capability matrix. The protected first
baseline remains `NEW_MAP` Grundorf, entrypoint index 1, Nordspitze Bauernhof,
eight seconds RUNNING, requested stop, and normal exact restore.

## Release Presentation Validation

`tools/Test-ReleaseIdentity.ps1` extracts `OmsiLaunch-current.zip` and audits
every distributed OmsiLaunch PE. It verifies the common product/version fields,
the controller's `OmsiLaunch.exe` internal/original filename, and its embedded
icon. `nethost.dll` is intentionally excluded because it is an unmodified
Microsoft runtime dependency, not an OmsiLaunch binary.

`tools/Test-ReleasePresentation.ps1` validates the extracted Release package,
not `artifacts/bin`. It verifies its manifest has configuration `Release`,
contains the permanent `plugins/OmsiLaunch.*` closure and four packaged splash
assets under `.omsilaunch/assets/splash`, and has no Debug or obsolete
`runtime/plugin` path in the manifest. Without
`-RunOmsi` it validates the default-managed, custom-managed, and `Unset` plans.
With `-RunOmsi -InstallPackage`, it installs only the package's product-owned
root and `plugins/OmsiLaunch.*` files into the authorized root, runs each case
through the installed Release CLI, verifies the temporary GUI overlay while the
session is alive, byte-exact GUI restoration, no remaining OMSI process, no
journal, and unchanged third-party plugin hashes. It writes its evidence under
the installation `.omsilaunch/diagnostics` directory.

Wave D D3D evidence uses the same public `StartSessionAsync` path. Session
`3cbd7bb6-d73f-4853-adc8-1213200527d2` validated device acquisition, one owned
QI reference, Reset-hook installation, create/describe, full and rectangular
updates, multiple resources, deterministic repeated-release rejection and
three create/release cycles on OMSI thread `16676`. Session
`fe1e10fd-cc79-484c-aa04-41fc40fef8da` covered forced process exit with a live
texture; `a95d9fb4-66c6-4f7a-8a64-a18d8e3f9225` proved prior-session stale
handle rejection after relaunch. Every run ended with no OMSI process, journal,
lease; `plugins\OmsiLaunch.*` remains as the permanent product installation.
Device loss/Reset tests are blocked
until a safe native lifecycle producer is available; calling Reset directly
from the validation harness is not an acceptable substitute.
