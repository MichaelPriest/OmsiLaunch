> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Erste Sitzung

Überprüfen Sie im OMSI-Stammverzeichnis ein portables Release-Beispiel, bevor Sie OMSI starten:

„Powershell
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
„

„RootPath: „.““ im gepackten Beispiel wird in das Verzeichnis aufgelöst, das Folgendes enthält
„OmsiLaunch.exe“. Starten Sie dieselbe Sitzung, indem Sie „/plan“ weglassen.

Die verwaltete Splash-Präsentation ist die Standardeinstellung. OmsiLaunch verwendet vorübergehend seine
PTB-, ENG-, DEU- oder FRA-Splash-Asset und stellt die ursprünglichen OMSI-Dateien wieder her
normaler Stopp, fehlgeschlagener Start oder dauerhafte Wiederherstellung. `/splash:Native` und
`/splash:Unset` behält den ursprünglichen OMSI-Splash bei.

