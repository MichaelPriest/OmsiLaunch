> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Legacy-Portabilität

Status: NORMATIV
Umsetzungsstatus: ZUKUNFT

Legacy-Portabilität ist eine Designbeschränkung und kein aktuelles Implementierungsziel.

## Ziel

Nachdem Current stabil ist und bevor Current seine ursprüngliche .NET-Basislinie verlässt,
Frieren Sie eine funktionierende „Legacy-Port-Basis“ für zwei explizite Backports ein: Legacy NT6
und Legacy XP.

## Prinzip

Current wird nicht alt gemacht, um altes Windows zu unterstützen. LaunchSpec-Semantik, öffentlich
Ergebnisse/Fehler, BuildProfile-Daten, Inhaltsidentitäten, Konfigurationssemantik,
Startup-Handoff-Drahtprotokoll und native Betriebsidentitäten bleiben portierbar;
Die Plattformimplementierungen variieren darunter.

„OmsiBuildProfile“ ist unabhängig von „RuntimePlatform“. DNNE ist ein Strom
Plugin-Host-Adapter, keine Protokollanforderung.

## Legacy-Ziele

Legacy NT6 zielt auf Vista SP2 x64, Windows 7 SP1 x64, Windows 8 x64 und
Windows 8.1 x64. Legacy XP zielt nur auf Windows XP SP3 x86 ab. Das eigentliche Erbe
Toolchains werden in ihren jeweiligen Port-Meilensteinen ausgewählt.

## Kabelkompatibilität

Startup-/Sitzungsdrahtstrukturen sind versioniert; Verwenden Sie Felder mit fester Breite, UTF-8
Zeichenfolgen mit expliziten Längen und dokumentierten Pack-/Ausrichtungs-/Endian-Regeln.
Sie verwenden niemals die CLR-Objektserialisierung oder ein von der Zeigergröße abhängiges Layout.

## Reproduzierbarkeit

Wo OMSI-Build/Inhalt dies unterstützen, sollte die gleiche semantische LaunchSpec sein
Wiederholbar auf aktuellen und älteren Plattformimplementierungen für Benchmark und
Optimierungsforschung.

## Entwicklungssequenz

1. Vervollständigen und stabilisieren Sie den Strom.
2. „Legacy-Port-Base“ einfrieren/taggen.
3. Erstellen Sie „legacy/nt6“ und „legacy/xp“.
4. Führen Sie explizite Backports durch und behalten Sie diese Zweige bei.
5. Modernisieren Sie Current erst, nachdem diese Baseline vorhanden ist.

Die Legacy-Implementierung beginnt nicht während des ersten aktuellen Builds.

