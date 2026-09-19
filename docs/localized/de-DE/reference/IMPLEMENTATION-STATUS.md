> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Implementierungsstatus

Status: RUNTIME_NEW_MAP_PASS

| Komponente | Staat | Notizen |
| --- | --- | --- |
| RuntimePlatform-Abstraktion | STATISCH_VALIDIERT | Aktueller Windows x64-Anbieter ist implementiert. |
| Aktueller Windows x64-Anbieter | STATISCH_VALIDIERT | Validiert die Plattform vor veränderbaren Laufzeitvorgängen. |
| BuildProfile | STATISCH_VALIDIERT | „Omsi23004_692EBFBF“ ist fingerabdruckbezogen. |
| Native.x86 | RUNTIME_VALIDATED | v145 Win32-Build; Profil-/Originalbyte-Schutzmaßnahmen, die auf dem kanonischen Pfad ausgeführt werden. |
| Von OmsiHook abgeleitete Interop-Grundlage | STATISCH_VALIDIERT | x86-Delphi-Strings, dynamische Arrays, Objekt-Snapshots und explizite Remote-Zuweisungsgrenze; kein Upstream-RPC-Protokoll. |
| In-Process-Plugin-Interop | STATISCH_VALIDIERT | „PluginRuntime“ verwendet einen profilbezogenen In-Process-Speicheranbieter; Der externe Anhang ist nicht der normale Sitzungspfad. |
| Laufzeitbefehlskanal | STATISCH_VALIDIERT | Sitzungsgebundenes Anforderungs-/Antwortpostfach mit fester Breite/SHA-256, das vom OMSI-UI-Thread-Timer versendet wird. |
| Laufzeitkarten-/Wetter-/Kameraleser | RUNTIME_VALIDATED | Canonical sitzungsvalidierte Zeit, Karte, vollständige profilierte Wetterskalarablesungen, tatsächliches Wetter und Kamera über den sitzungsgebundenen Plugin-Kanal. |
| Erweitertes Karten-/Kacheldiagramm | BLOCKED_PROFILE_LAYOUT | Das Upstream-Label „OmsiMap.Kacheln“ bei „Map+0x118“ wurde in drei geschützten aktuellen Sitzungen nicht in einen gültigen Delphi-Dynamic-Array-Header aufgelöst. Die Testversion schreibt nie und alle Sitzungen werden normal wiederhergestellt. |
| Laufzeituhr/skalares Wetter schreibt | RUNTIME_VALIDATED | „time.set“ wendete profiliertes „SetTime“ mit Rücklesen an; „weather.set“ validierte eine profilgestützte skalare Whitelist und Wiederherstellung. |
| Laufzeitkamera FOV schreiben | RUNTIME_VALIDATED | „camera.set“ hat das Sichtfeld während der kanonischen Sitzung geändert und wiederhergestellt; Die Reichweite der Kamerafamilie bleibt unter Kontrolle. |
| Zusammenfassung der Laufzeit Fahrzeug/Mensch/Fahrplan | RUNTIME_VALIDATED | Der kanonische Grundorf-Batch gab den Zustand von Straßenfahrzeugen, die Anzahl der Personen und die Array-Anzahl des Fahrplanmanagers zurück, ohne Rohzeiger offenzulegen. |
| Detaillierte Fahrplantelemetrie zur Laufzeit | RUNTIME_VALIDATED | Profilierte unveränderliche Snapshots gaben Tracks, Fahrten und Linien mit ihren semantischen Namen, Pfaden, Zählungen und Zuweisungsmetadaten über den Sitzungskanal zurück. |
| Laufzeitfahrplan Haltestellen und Links | RUNTIME_VALIDATED | Profilierte unveränderliche Snapshots lieferten Bushaltestellennamen, IDs, Links und Metadaten von Stationsverbindungsendpunkten. „Nordspitze“ wurde live aus dem UTF-16 Delphi-Feld gelesen. |
| Laufzeitfahrplantouren | RUNTIME_VALIDATED | Profilierte verschachtelte Tour-Snapshots gaben Linienzugehörigkeit, AI-Gruppe/-Typ, Fahrzeugreservierungsmetadaten und TourEntry-Zähler zurück, ohne Delphi-Zeiger offenzulegen. |
| Laufzeitfahrplanprofile | RUNTIME_VALIDATED | Profilierte verschachtelte Profil-Snapshots lieferten Fahrtzugehörigkeit, Name, Gesamtzeit, Stoppzeit und Track-Eintrittszeit-Zähler. |
| Laufzeitfahrplan-Toureinträge | RUNTIME_VALIDATED | Profilierte verschachtelte TourEntry-Snapshots lieferten semantische Reiseidentität, Reise-/Profilindizes, Timing und reibungslosen Übergangsstatus. |
| Fahrplan-Gleiseinträge | RUNTIME_VALIDATED | Begrenzte TrackEntry-Snapshots lieferten 91 Live-Aufzeichnungen mit Track-Zugehörigkeit, ID, Kachel/Pfad, Entfernungen, Gültigkeit, Reihenfolge und Chrono-Metadaten über den Sitzungskanal. |
| Laufzeitplan RVFiles | RUNTIME_VALIDATED | Das profilierte RVFile-Array hat über den Sitzungskanal eine gültige leere Sammlung auf Grundorf zurückgegeben. |
| Laufzeitplan NoRVNumbers | UNGELÖST | Die Upstream-Decodierung „raw:true“ steht in Konflikt mit dem beobachteten Feldlayout „Omsi23004_692EBFBF“. Aus der automatischen Charge entfernt; Andere Fahrplantelemetriedaten sind davon nicht betroffen. |
| Detaillierte Laufzeit-Fahrzeug-/Mensch-Telemetrie | RUNTIME_VALIDATED | Sitzungsbezogene undurchsichtige Handles und schreibgeschützte Schnappschüsse lieferten kohärente Fahrzeugbewegungen/-steuerungen/Beleuchtung/KI und den Zustand menschlicher Ziele/Tickets/Sitzplätze/Stationen/KIs auf Grundorf. Es sind keine räumlichen oder willkürlichen Entitätsschreibvorgänge aktiviert. |
| Laufzeitskriptvariable lesen | RUNTIME_VALIDATED | ANSI-Namenstabellen und die inDas Zeiger-Array „PublicVars“ löste 1.025 Namen und „Refresh_Strings=0“ über ein undurchsichtiges Fahrzeughandle auf. |
| Mutation der Laufzeitskriptvariablen | RUNTIME_VALIDATED | „vehicle.variable.set“ löst ein undurchsichtiges Live-Handle und den profilierten „PublicVars“-Slot nach offenem String-Namen auf, lehnt nicht endliche Werte ab und gibt sofortiges Zurücklesen zurück. „Refresh_Strings 0 -> 1 -> 0“ wurde bei normaler Bereinigung übergeben. |
| Laufzeit-String-Variable lesen | RUNTIME_VALIDATED | ANSI-Namenstabellen plus Unicode-Zeichenfolgenwerte lösten 27 Namen und „ident=GRN-V 30“ über ein undurchsichtiges Fahrzeughandle auf. Delphi-Ersetzungs-/Refcount-Schreibvorgänge bleiben getrennt. |
| Wave A-Laufzeit-Metadatenleser | RUNTIME_VALIDATED | HOF-Metadaten, Fahrer, Ticketpakete und Fahrplanprotokolle wurden in der Sitzung „e33aae7e-450a-4c00-8c7a-5f550ac4263f“ übergeben; Konstanten und Kurven, die in „46260d64-a8e9-4ee9-a907-00e46b07a1ae“ übergeben wurden, alle mit normaler Bereinigung. |
| Permanente Plugin-Installation | STATISCH_VALIDIERT | Die Release-ZIP-Datei platziert den OmsiLaunch-Plugin-Abschluss direkt in „plugins\\“. Eine Sitzung validiert die genauen Produkt-Hashes vor der Markteinführung; Es werden niemals Drittanbieter-Plugins bereitgestellt, Snapshots erstellt, wiederhergestellt, entfernt oder beansprucht. |
| PlaceRandomBus | RUNTIME_VALIDATED | Die Sitzung „4e2321d4-4c09-428b-8760-d348205b7392“ gab „2“ zurück, erhöhte die Anzahl der Straßenfahrzeuge von 2 auf 4 und schloss dann die genaue Bereinigung ab. Es unterscheidet sich von make-basic und erhebt keinen PlayerVehicle-Anspruch. |
| Benannte Trigger und String-Mutation | BLOCKED_STRING_OWNERSHIP | Der Produktionsversand wird absichtlich zurückgehalten, bis die Delphi-String-Zuweisung, -Zuweisung und -Lebensdauer geschlossen werden kann, ohne dass das Upstream-Leck-/Korruptionsrisiko übernommen wird. |
| OmsiHook-Domänenbetriebskatalog | STATISCH_VALIDIERT | Die Domänen Programm, Karte, Zeit, Wetter, Fahrzeuge, Menschen, Zeitplan, Kamera, Player und Ton sind semantisch/profilgesteuert. |
| Wave D D3D-Gerät und Texturen | RUNTIME_VALIDATED | Geräteerfassung mit exaktem Profil, eine eigene QI-Referenz, undurchsichtige Sitzungshandles, Erstellung/Beschreibung/vollständige und korrekte Aktualisierung/Freigabe, wiederholte Freigabeablehnung, mehrere Ressourcen, erzwungenes Beenden und Neustart-Bereinigung bestanden. |
| Wave D D3D-Lebenszyklus/Zurücksetzen | IMPLEMENTED_NOT_RUNTIME_VALIDATED | Durch das Zurücksetzen des Abfangens werden Standardpoolressourcen vor dem nativen Aufruf ungültig gemacht, die Generierung vorangetrieben und geordnete Lebenszyklusübergänge veröffentlicht. Eine sichere Geräteverlust-/Reset-Induktion war nicht verfügbar, sodass Verlust-/Reset-/Wiederholungszyklus-/Race-Szenarien weiterhin blockiert und nicht beansprucht werden. |
| DNNE-Adapter | STATISCH_VALIDIERT | Der aktuelle Adapter ist von PluginRuntime getrennt. |
| PluginRuntime | RUNTIME_VALIDATED | Übergabeverbrauch, synchroner Headless-Arm und generische NEW_MAP-Ausführung erreichten das Gameplay. |
| Tragbare Übergabe | STATISCH_VALIDIERT | Das UTF-8/SHA-256-Protokoll v4 mit fester Breite trägt semantische Einstiegspunkt- und gespeicherte Situationsidentitäten; Die v3-Dekodierung wird weiterhin unterstützt und Round-Trip-/Korruptionstests werden bestanden. |
| Inhaltserkennung | STATISCH_VALIDIERT | Offline-Karten-/Situations-/Fahrzeug-/Neulackierungs-/HOF-Erkennung plus undurchsichtige gehashte „[Einstiegspunkte]“-Datensätze; Die Startkorrelation des Einstiegspunkts bleibt separat gesteuert. |
| Konfiguration | STATISCH_VALIDIERT | Nur sitzungsbezogen; Verlustfreie No-Op-, Negativ-Flags-, Range-, Compound-Blöcke- und Vektorerhaltungstests werden bestanden. |
| „.omsilaunch“-Installationsverzeichnis | RELEASE_VALIDATED | Der Verzeichnisstatus wird nach dem Lease-Erwerb für die verwaltete Präsentation verzögert erstellt. Persistente Ressourcen befinden sich unter „Assets“. Diagnose bleibt bestehen; Das dauerhafte Journal wird nach der Wiederherstellung entfernt. Es handelt sich nicht um eine Datei und niemals um ein Plugin-Bereitstellungsziel. |
| Verwaltete Splash-Präsentation | RELEASE_RUNTIME_VALIDATED | „Verwaltet“ ist die Standardeinstellung. Verpackte PTB/ENG/DEU/FRA 640x480 24-Bit-Assets befinden sich unter „.omsilaunch/assets/splash“ und überlagern transaktional „GUI/NewSplashscreen_ENG.bmp“ sowie das aufgelöste Gebietsschema. Die Release-Sitzungen „388cf5d2-7310-4301-9313-e62c4c08a346“ (Standard) und „794cae34-a2ac-4fcf-8981-f142ac14c308“ (benutzerdefiniertes Verzeichnis) erreichten das Gameplay und führten die normale Bereinigung durch. Sitzung „12a74b24-6c78-486“ deaktivieren3-8ffe-947651212eeb` erreichte das Gameplay ohne geplante GUI-Überlagerung. Alle drei stellten den ursprünglichen GUI-Status wieder her und entfernten das Journal; Permanente Produkt-Plugins waren keine Transaktionsteilnehmer. |
| Release-Paket | RELEASE_VALIDATED | „tools/New-ReleasePackage.ps1 -Configuration Release“ erzeugt „OmsiLaunch-current.zip“ mit SHA-256-Manifest, Release-Laufzeitabschluss, Splash-Assets, „.omsilaunch“-Beispiel und ohne Debug-Pfade im Manifest. |
| Transaktion/Rückgewinnung | RUNTIME_RECOVERY_VALIDATED | Ein veraltetes „HANDOFF_CREATED“-Journal stellte nur sitzungseigene Artefakte wieder her und entfernte sein Journal. Die Eigentümervalidierung erfordert einen nicht beendeten Prozess, eine Erstellungszeit und einen ausführbaren Pfad, bevor die Wiederherstellung zurückgehalten wird. |
| PlanSession | STATISCH_VALIDIERT | Der kanonische Grundorf-Plan löst Laufzeitartefakte schreibgeschützt auf. |
| Kopfloser Start | IMPLEMENTIERT / RUNTIME_VALIDATED | Der native Startvorgang wurde synchron aktiviert und das Gameplay war der beobachtete Zielzustand. |
| NEUE_KARTE | IMPLEMENTIERT / RUNTIME_VALIDATED | Grundorf, vorgestellter Index 1 und Nordspitze Bauernhof durch Gameplay abgeschlossen. |
| NEW_MAP-Einstiegspunktidentität | RUNTIME_PARTIAL | Offline-Rohdatensätze verfügen über stabile Hashes. Die Laufzeit bestätigt, dass der von Grundorf präsentierte Index „1“ den Rohindex „0“ („Nordspitze Bauernhof“) auswählt, der native präsentierte Text/Mapping bleibt jedoch ungelöst. Anträge auf öffentliche Identität bleiben weiterhin verschlossen. |
| GESPEICHERT_SITUATION | RUNTIME_VALIDATED | „situations\\Baustelle Falkenseer Ch..osn“ lud Berlin-Spandau über das vollständig profilierte Startformular, den Selektor, die Synchronisierung und die „Button1Click“-Sequenz, erreichte das Gameplay, blieb 8 Sekunden lang LAUFEND und schloss dann den angeforderten Stopp und die normale Wiederherstellung ab. PlanSession lehnt immer noch „.osn“-Dateien ab, deren deklarierte Karte vor dem Staging fehlt. |
| LAST_MAP_STATE | UNSUPPORTED_FOR_CURRENT_PROFILE | Der Zweig für die exakte native Wiederherstellung der letzten Karte ist nicht geschlossen. Es gibt keinen Fallback für die Dateireihenfolge. |
| Explizites/Systemdatum/Uhrzeit | STATICALLY_PARTIAL | Wird von der kanonischen Regression nicht angefordert. |
| Spielerfahrzeug | STATICALLY_PARTIAL | Wird von der kanonischen Regression nicht angefordert. |
| Legacy-NT6-Implementierung | ZUKUNFT / N/A | Nur architektonische Grenze. |
| Legacy-XP-Implementierung | ZUKUNFT / N/A | Nur architektonische Grenze. |

## Canonical Runtime Baseline

„RUNTIME_NEW_MAP_PASS“: öffentliches „StartSessionAsync“ validierte die permanente Laufzeitinstallation und
tragbare Übergabe, erreichte „gameplay.entered“ für „maps\Grundorf\global.cfg“.
mit angezeigtem Einstiegspunktindex „1“ („Nordspitze Bauernhof“), blieb RUNNING
acht Sekunden lang, dann wurde ein angeforderter Stopp und eine normale exakte Wiederherstellung durchgeführt.

Der aktuelle Installationsleasingvertrag verwendet ein benanntes Windows-Semaphor (maximale Anzahl).
eins), kein Mutex: Die Bereinigung kann in einem anderen verwalteten Thread erfolgen. Das Langlebige
Die Journal- und Prozessidentität bleibt die Autorität für die Wiederherstellung nach einem Absturz.

Das Standard-Startzeitlimit beträgt 180 Sekunden. Dadurch bleibt ein begrenzter Fehler erhalten
Pfad und ermöglicht gleichzeitig die Vervollständigung nativ gespeicherter Situationen auf großen Karten
legitime Ladearbeiten vor dem Spiel; Anrufer können ein kürzeres explizites Timeout festlegen.

## Release-Präsentation 001

Release-Build, Einheit/Integrationssuiten, gepackte CLI „PlanSession“, Paket
Manifest- und Ressourcenvalidierung bestanden. Die gepackten ausführbaren Berichte
Produkt „OmsiLaunch“, Produktversion „0.1.0“, Protokoll „0.1“ und verwendet das
offizielles Symbol mit mehreren Auflösungen. Ein verwalteter Release-Start wurde erstellt und mit einem Hash abgeglichen
alle vier dauerhaften Splash-Assets unter „.omsilaunch/assets/splash“. Die Veröffentlichung
Standard-, benutzerdefinierte und „Unset“-Sitzungen erreichten jeweils das Gameplay und wurden dann „Abgeschlossen“.
Standardmäßige/benutzerdefinierte Overlays und native Erhaltung wurden von der Öffentlichkeit ausgewählt
LaunchSpec und validiert durch die ausgegebenen Pläne. Alle drei restaurierten das Original
GUI-Status, das Journal wurde entfernt und das permanente Produkt-Plugin beibehalten
Schließung. Die
Das Transaktionsverhalten bei Abwesenheit eines Ziels wird separat abgedeckt
`transaction.absent-overlay-restore`.

