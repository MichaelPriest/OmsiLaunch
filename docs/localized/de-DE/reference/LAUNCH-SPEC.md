> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# LaunchSpec

Status: NORMATIV

„LaunchSpec“ sind portable semantische Daten. „UNSET“ bedeutet, das Bestehende zu bewahren
OMSI-Einstellung und darf niemals auf „falsch“, null oder erfunden reduziert werden
Standard. Die JSON-Darstellung ist das gleiche Modell, das von „/spec“ verwendet wird; explizit
CLI-Schalter überschreiben die Dateiwerte.

Unterstützte Strukturgruppen sind Installation, Welt, Einstiegspunktidentität oder
präsentierter Index, Datum, Uhrzeit, Jahr, Wetter, Spielerfahrzeug, Konfiguration,
Eingabe, Diagnose und Laufzeitverhalten. „NEW_MAP“, „SAVED_SITUATION“ und
„LAST_MAP_STATE“ sind semantische Modi. „LAST_MAP_STATE“ bedeutet OMSIs nativ
automatischer Zweig zur Wiederherstellung der letzten Karte; es wird niemals aus der Datei „.osn“ abgeleitet
Zeitstempel oder Verzeichnisreihenfolge. Bei numerischen nativen Listenindizes ist dies nicht der Fall
persistente Inhaltsidentitäten.

Für „NEW_MAP“ ist „World.EntrypointIdentity“ für eine zukünftige Struktur reserviert
kanonische Identität. Ein rohes „global.cfg“-Label reicht nicht aus, da dies möglich ist
kann dupliziert werden und vom nativ dargestellten Etikett abweichen. Bis zum
Profil schließt diese Korrelation, „PresentedEntrypointIndex“ wird unterstützt
Diagnose-/Niedrigfüllstand-Selektor. Startup Handoff v3 enthält ein Identitätsfeld
ohne zeigergroße Werte, aber die öffentliche Nutzung bleibt funktionsabhängig.

Datum und Uhrzeit verwenden explizite Datensätze „Jahr/Monat/Tag“ und „Stunde/Minute/Sekunde“.
Das Schema umfasst keine Betriebssystemversionen, Win32-Handles, CLR-Laufzeitobjekte,
DNNE-Details, Speicherzuordnungen oder native Adressen.

PlanSession ist der Compiler für diese Daten. Ein Plan ist nur dann nicht ausführbar, wenn a
Die angeforderte erforderliche Fähigkeit ist nicht verfügbar. Unaufgeforderte optionale Funktion
Tore dienen der Information.

