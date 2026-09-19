> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Inhaltserkennung

Status: NORMATIV

Discovery ist offline und schreibgeschützt. Karten verwenden „maps\...\global.cfg“; Situationen
use `situations\...\.osn`; Fahrzeuge verwenden OMSI-relative „.bus“-Dateien; Neulackierungen sind
fahrzeugbezogene CTI-Identitäten. HOF, Flottennummer, Registrierung und Zusatz
Die Ergebnisse melden nur Beweise, die in installierten Inhalten verfügbar sind. Entdeckungsauftrag bzw
Dateizeitstempel definieren niemals die Semantik des gespeicherten Laufzeitzustands.
## Einstiegspunkte

„EnumerateEntrypoints(mapIdentity)“ analysiert die vom Profil beobachteten „[Einstiegspunkte]“.
Datensätze in „global.cfg“. Seine kanonische Identität ist die Kartenidentität plus an
SHA-256 des gesamten normalisierten Rohdatensatzes. Beschriftungen sind Anzeigemetadaten
Nur, weil OMSI-Karten doppelte Beschriftungen enthalten können. Discovery erhebt keinen Anspruch
dass dieser Rohdatensatz mit der Laufzeit „Tform_setpos“ korreliert wurde
vorgestellte Liste.

