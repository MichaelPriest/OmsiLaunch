# First Session

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


For a standalone/manual installation, the controller can live elsewhere and
receive the OMSI executable explicitly:

```powershell
.\OmsiLaunch.exe /exe:"D:\Games\OMSI 2\Omsi.exe" /new /map:"maps\Grundorf\global.cfg" /entrypoint-index:1 /plan /json
```

Remove `/plan` after the plan reports `READY`. An unsupported executable
fingerprint is rejected before the OMSI process is created.
