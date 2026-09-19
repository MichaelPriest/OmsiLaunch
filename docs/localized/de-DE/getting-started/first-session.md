# Erste Sitzung

Prüfen Sie im OMSI-Stammverzeichnis zuerst das portable Release-Beispiel:

```powershell
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
```

`RootPath: "."` bezeichnet den Ordner mit `OmsiLaunch.exe`. Entfernen Sie
`/plan`, um die Sitzung zu starten. Die verwaltete Splash-Anzeige ist Standard;
`Native` und `Unset` erhalten die OMSI-Originaldatei.
