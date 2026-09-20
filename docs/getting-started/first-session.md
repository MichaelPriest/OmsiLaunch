# First Session

## Graphical launcher

For normal interactive use, open `OmsiLaunch.Launcher.exe`.

The launcher detects `Omsi.exe` beside itself when the package is installed in
the OMSI root. For a standalone/manual installation elsewhere, click
**Browse...** once; the selected executable path is remembered in the current
Windows user profile.

Choose **New map** or **Saved situation**, select the content to load, and use
**Validate** before **Start OMSI**. The diagnostic panel shows unsupported build
profiles, missing plugin files, plugin startup failures, and session lifecycle
events.

Keep the launcher open while the managed session is running. Closing it with an
active session prompts for a safe stop so OmsiLaunch can restore temporary files.

## Command line

From the OMSI root, inspect a portable Release example before starting OMSI:

```powershell
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
```

`RootPath: "."` in the packaged example resolves to the directory containing
`OmsiLaunch.exe`. Start the same session by omitting `/plan`.

Managed splash presentation is the default. OmsiLaunch temporarily uses its
PTB, ENG, DEU, or FRA splash asset and restores the original OMSI files on
normal stop, failed startup, or durable recovery. `/splash:Native` and
`/splash:Unset` preserve the original OMSI splash.


For a standalone/manual installation, invoke the installed controller with the
OMSI executable explicitly when you do not want to rely on a Steam-library
location:

```powershell
.\OmsiLaunch.exe /exe:"D:\Games\OMSI 2\Omsi.exe" /new /map:"maps\Grundorf\global.cfg" /entrypoint-index:1 /plan /json
```

The normal OmsiLaunch plugin closure must still be installed under that OMSI
root. Remove `/plan` after the plan reports `READY`. An unsupported executable
fingerprint is rejected before the OMSI process is created.


## Session lifetime

A normal launch now remains active until OMSI itself is closed. The CLI no
longer stops a successful interactive session after the old eight-second
observation window.

For automated validation, test, or one-shot runtime operations, the bounded
lifecycle is retained. You can also request it explicitly with
`/observe-seconds:<n>`.
