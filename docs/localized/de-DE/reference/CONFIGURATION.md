> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Konfiguration

Status: NORMATIV

Alle LaunchSpec-Konfigurationsüberschreibungen sind sitzungsbezogen. OmsiLaunch-Snapshots
Jeder berührte „options.cfg“, „Inputs\keyboard.cfg“ oder „Inputs\gamectrler.cfg“.
Datei vor dem Anwenden einer Überlagerung, zeichnet sie im dauerhaften Transaktionsjournal auf,
und stellt die ursprünglichen Bytes und SHA-256 nach dem normalen Beenden „StopSession“ wieder her.
startup failure, or stale-journal recovery. There is no permanent-edit API.

`UNSET` is preserve: it produces no mutation. Unknown tokens, ordering,
Codierung, Zeilenumbrüche, Vektorenden und nicht verwandte Werte überleben verlustfreie Patches.

## Implemented Options Surface

„ConfigurationCatalog“ unterstützt derzeit bewährte Sitzungs-Overlays für allgemeine,
Ansicht, Steuerung, Kollision, Ticket, automatische Speicherung, Grafikentfernung/-komplexität,
Schablonen-/Regenreflexion, Verkehr, Ton und die vierwertigen „Rauchsysteme“.
blockieren. Die öffentliche negative Semantik wird an der Codec-Grenze invertiert; für
Beispiel „simulation.collisionTerrain=false“ schreibt „no_collision_terrain“.

„advanced.reducedMultithreading“ ist eine semantische Einstellung und synchronisiert beide
native reduced-multithreading flags. `AIMaxCountRandom` patches only the road
Verkehr oder menschliche Komponente und behält die anderen sieben Vektorwerte bei.

„graphics.realTimeReflections“ akzeptiert derzeit nur „Economy“ und „Full“.
Die native Darstellung der Deaktiviert-Auswahl der Benutzeroberfläche bleibt bewusst bestehen
non-writable until static evidence closes it. `graphics.texture` und
„graphics.textureFilter“ sind bekannt, aber aus demselben Grund nicht beschreibbar.

`graphics.particles` is a compound value:
`enabled,maxPerEmitter,playerVehicleOnly,inReflections`. Es übersetzt die
natives viertes Feld („disableInReflections“), ohne diesen negativen Namen offenzulegen.

