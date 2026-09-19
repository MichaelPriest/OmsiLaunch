> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# `.omsilaunch` Installationsstatusverzeichnis

„.omsilaunch“ ist ein Verzeichnis unterhalb des OMSI-Installationsstammverzeichnisses. Es ist nie ein
LaunchSpec-Datei und wird nicht in „Plugins“ bereitgestellt.

Die Release-ZIP-Datei wird direkt in das OMSI-Stammverzeichnis extrahiert. „OmsiLaunch.exe“ und
Seine Controller-Assemblys bleiben in diesem Stammverzeichnis, während das permanente OMSI-Plugin
Der Verschluss wird direkt unter „plugins\\OmsiLaunch.*“ installiert. Diese Dateien gehören
zur Produktinstallation: Sitzungen validieren ihre Hashes, stellen sie jedoch niemals bereit.
Snapshots erstellen, wiederherstellen, entfernen oder als temporäre Bereitstellungsquelle verwenden.

## Schöpfung und Eigentum

Das Verzeichnis wird träge erstellt. Es wird eine Managed-Splash-Sitzung erstellt
`<OMSI>/.omsilaunch/assets/splash` nach dem Erwerb der Installationsmiete und
bevor die Transaktion angewendet wird. Fehlende OmsiLaunch-Assets werden kopiert
aus dem gepackten Verzeichnis „.omsilaunch/assets/splash“. Vorhandene Vermögenswerte sind niemals vorhanden
überschrieben, sodass ein explizit verwalteter Projekt-Asset-Satz bestehen bleibt.

Das Verzeichnis ist Eigentum von OmsiLaunch. Sie unterscheidet sich von der OMSI-Konfiguration
und aus der permanenten Plugin-Installation.

## Layout

| Pfad | Lebenszeit | Zweck |
|---|---|---|
| `assets/splash/{PTB,ENG,DEU,FRA}.bmp` | Persistente OmsiLaunch-Ressource | Verwaltete Standard-Splash-Assets mit 640 x 480 und 24 Bit. Es ist keine Release-Nummer eingebettet. |
| `diagnostics/<session>-host.log` | Anhaltende Diagnose | Semantische Übergangsverfolgung des Hosts. |
| `diagnostics/*-runtime-*.json` | Anhaltende Diagnose | Explizite Laufzeitbefehle/Batch-Beweise. |
| `journal.json` | Temporärer dauerhafter Transaktionsstatus | Genauer Snapshot, Prozessidentität und Wiederherstellungsfortschritt. Nach normaler Wiederherstellung entfernt; nur zur Wiederherstellung aufbewahrt. |
| `*.omsilaunch.tmp` neben einer berührten OMSI-Datei | Vergänglich | Atomic-Write-Staging; entfernt oder atomar ersetzt. |

Es werden nur temporäre OMSI-Änderungen wie „GUI/NewSplashscreen_*.bmp“ angefordert
Geben Sie für Konfigurationsüberlagerungen die Snapshot-/Journal-/Wiederherstellungstransaktion ein.
Plugins von Drittanbietern werden niemals als Eigentümer aufgeführt, kopiert, entfernt oder
restauriert.

Startübergabe-, Telemetrie- und Laufzeitbefehlszuordnungen werden als „gemeinsam“ bezeichnet
Erinnerungsobjekte. Sie sind sitzungs-/prozessgebunden und **keine** Dateien darunter
Verzeichnis.

## Splash-Auswahl

„SessionPresentationSpec.Splash“ verfügt über drei semantische Modi:

- „Verwaltet“ (Standard): OmsiLaunch installiert „ENG“ plus die aufgelöste Sprache
  Ziel in „GUI/NewSplashscreen_*.bmp“ über die vorhandene Transaktion. Die
  Original-OMSI-Dateien werden nach Beendigung, Fehler oder Byte für Byte wiederhergestellt
  Erholung.
- „Nativ“ oder „Unset“: OMSI-Splash-Dateien bleiben erhalten; Es gibt kein GUI-Splash-Overlay
  erstellt.

Die Sprachauflösung akzeptiert „PTB“, „ENG“, „DEU“ und „FRA“. wenn es nicht vorhanden ist, wird es verwendet
„options.cfg“ und greift dann auf „ENG“ zurück. `/splash-assets:<Verzeichnis>` oder
„Presentation.CustomAssetDirectory“ wählt ein explizites benutzerdefiniertes Verzeichnis aus. A
Das relative benutzerdefinierte Verzeichnis wird unterhalb des OMSI-Installationsstammverzeichnisses aufgelöst. Es muss
ein gültiges „ENG.bmp“ und, für eine nicht-englische ausgewählte Sprache, dessen Übereinstimmung enthalten
lokalisiertes BMP. Assets müssen BMP-Dateien im Format 640 x 480 und 24 Bit sein.

Fehlende Paket-/Standard-Assets, ungültiges Asset-Format oder eine ungültige benutzerdefinierte Datei
Planung/Start des Verzeichnisses schlägt mit einem semantischen Splash-Fehler fehl; OMSI ist es nicht
gestartet. Ein fehlendes „.omsilaunch“-Verzeichnis selbst ist normal und wird als erstellt
oben beschrieben.

## Freigabevalidierung

„tools/Test-ReleasePresentation.ps1“ validiert die gepackte ausführbare Release-Datei
und kann die drei Präsentationsfälle gegen einen autorisierten OMSI durchführen
Installation. Sein Standarddurchgang ist nicht mutierend und prüft Paket-Assets plus
die drei „PlanSession“-Ergebnisse. „-RunOmsi“ beweist zusätzlich das Tempo
Overlay während der Ausführung von OMSI, exakte GUI-Wiederherstellung und Journal-/Prozessbereinigung.
Mit „-RunOmsi -InstallPackage“ kopiert es nur das produkteigene Paket
Root-Dateien und „plugins/OmsiLaunch.*“-Dateien dann in das autorisierte Test-Root
Verifiziert außerdem, dass die Hashes von Drittanbieter-Plugins unverändert bleiben. Das Ergebnis ist
blieb bestehen als „Diagnose/Release-Präsentation-Validierung“..json`.

## Konfigurationsdateien

LaunchSpec JSON wird über „/spec:<file.json>“ bereitgestellt. Ein Verteilungsbeispiel ist
`examples/.omsilaunch/canonical-session.json`; Das verschachtelte Verzeichnis ist ein Beispiel
Nur Layout. Es kann in das Verzeichnis „.omsilaunch“ einer Installation kopiert werden.
Die Konfigurationsdatei ist jedoch nicht erforderlich, damit das Verzeichnis funktioniert.

