> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Laufzeitfunktionen

Status: UMSETZUNGSINVENTAR

| Fähigkeit | API/Kanal | Interop | Statischer Beweis | Laufzeitbeweise | Release-Status |
| --- | --- | --- | --- | --- | --- |
| Sitzungsanforderung/-antwort | `ExecuteRuntimeAsync` | Profilunabhängiges Postfach | Protokoll- und Plugintests | kanonischer Batch PASS | RUNTIME_PASS |
| Zeit gelesen | `time.read` | `OmsiTimeAdapter` | gepinnt OmsiHook plus genaues Profil | PASS | RUNTIME_PASS |
| Karte lesen | `map.read` | `OmsiRuntimeReaders` | angeheftetes „OmsiMap“-Layout | PASSIEREN; geladener Zustand und zurückgegebene Kacheln | RUNTIME_PASS |
| Karten-/Kacheldiagramm | keine | Profilversuch intern gespeichert | Upstream bezeichnet „OmsiMap+0x118“ als „Kacheln“, aber der aktuelle Live-Speicher hat keinen gültigen Delphi-Array-Header verfügbar gemacht | Die Testversion von „map.tiles.list“ schlug in den Sitzungen „ec85b51a-f69a-4f05-8fb3-638c63f9f20d“, „129482a4-b3a2-448a-830e-4043d11de246“ und sicher fehl `90011081-f5e9-4b31-b907-941bb3a9fd1a`; alles normal gereinigt | BLOCKED_PROFILE_LAYOUT |
| Wetter lesen | `weather.read` | `OmsiRuntimeReaders` | „OmsiWeather“ und aktive Wetteraufzeichnungslayouts angeheftet | PASS: Basisskalare plus Temperatur, Taupunkt, Druck, Niederschlag und Rate | RUNTIME_PASS |
| Aktuelle/ICAO-Wetterlage | `weather.actual.read` | `OmsiRuntimeReaders` | angeheftetes „OmsiActuWeather“-Layout | PASS | RUNTIME_PASS |
| Kamera lesen | `camera.read` | `OmsiRuntimeReaders` | angehefteter Wrapper und Beweise für die Kamerafamilie | PASS | RUNTIME_PASS |
| Kamera-FOV-Mutation | `camera.set` | `OmsiCameraWriter` | angeheftetes Kamera-Skalar-Layout | PASS: `45 -> 46 -> 45` | RUNTIME_PASS |
| Uhrenmutation | `time.set` | `OmsiTimeAdapter` + profiliertes `SetTime` | Upstream-Taktfelder und nativer Aufruf | PASS: Minute `17 -> 18 -> 17` | RUNTIME_PASS |
| Skalare Wettermutation | `weather.set` | „OmsiWeatherWriter“-Whitelist | Upstream-Skalarfelder und Profillayouts | PASS: Wind `0 -> 1 -> 0` | RUNTIME_PASS |
| Semantisch präsentierter Einstiegspunkt | eingezäunt | native „Tform_setpos“ exakte eindeutige Listenübereinstimmung | profilierter „FormShow“, Listenelementtext und „Button1Click“-Fluss | Teilweise: kanonischer Lauf bewiesen „präsentierter Index 1 -> Rohindex 0 -> Nordspitze Bauernhof“; Die dargestellte Textdarstellung bleibt ungelöst und Rohbeschriftungen werden dupliziert | RUNTIME_PARTIAL |
| Gespeicherte Situation Start | `WorldMode.SavedSituation` | profilierter Startformmodus/Selektor/Button1Click-Sequenz | „Tform_start+0x3E4/+0x3F8/+0x434/+0x438/+0x43C“, natives „SetChecked“, VMT-Elementindex-Setter und „Button1Click“ | PASS: `situations\\Baustelle Falkenseer Ch..osn` hat Berlin-Spandau geladen, hat `gameplay.entered` erreicht, blieb 8 s lang RUNNING, hat dann den angeforderten Stopp und die normale Wiederherstellung abgeschlossen | RUNTIME_PASS |
| Kalender / aktuelle ICAO-Mutation | keine | Delphi-String/ABI-Grenze unvollständig | Methoden-/Profilreferenzen | nicht ausgeführt | RELEASE_IF_CLOSED |
| Staat der Straßenfahrzeugsammlung | `road-vehicles.read` | profilierter „OmsiMyOmsiList“-Status | angepinnter OmsiHook-Container | PASS: Anzahl/KI/Spielerindex | RUNTIME_PASS |
| Detaillierte Telemetrie von Straßenfahrzeugen | `road-vehicles.list`, `road-vehicle.read`, `player-vehicle.read` | Profilierte „MemArrayList“- und „OmsiRoadVehicleInst“-Snapshots | Upstream-Layouts abgeglichen mit „Omsi23004_692EBFBF“ | PASS: undurchsichtiger Griff, Position, Drehung, Bedienelemente, Beleuchtung und KI-Felder; Keine-Fahrzeug-Anfrage gibt „present=false“ zurück | RUNTIME_PASS |
| Fahrzeugskriptvariable gelesen | `vehicle.variables.list`, `vehicle.variable.get` | profilierte ANSI-Namenstabelle und öffentliches Zeiger-Array | angeheftete `OmsiComplMapObjInst.GetVariable`-Kette; Profil verwendet Instanzwerte „+0x23C“ | PASS: 1.025 Namen aufgezählt; `Refresh_Strings=0` | RUNTIME_PASS |
| Mutation der Fahrzeugskriptvariablen | `vehicle.variable.set` | profilierte ANSI-Namenstabelle und öffentliches Zeiger-Array | angeheftete „OmsiComplMapObjInst.SetVariable“-Kette; endliche Float-Eingabe, Live-Validierung undurchsichtiger Handles und sofortiges Zurücklesen | PASS: `Refresh_Strings 0 -> 1 -> 0`; normale Bereinigung PASS | RUNTIME_PASS |
| Fahrzeug-String-Variable gelesen | `vehicle.string-variables.list`, `vehicle.string-variable.get` | profilierte ANSI-Namenstabelle und Unicode-Werte-Array | „OmsiComplMapObjInst.GetStringVariable“-Kette angeheftet | PASS: 27 Namen aufgezählt; `ident=GRN-V 30` | RUNTIME_PASS |
| Fahrzeugkonstanten | `vehicle.constants.list`, `vehicle.constant.get` | profilierter „ScriptConstants“-Block | Angeheftete „OmsiConstBlock“-Konstanten-/Namensarrays | PASS: `AI_lights_blinkgeberintervall = 0.3`; normale Bereinigung PASS | RUNTIME_PASS |
| Fahrzeugkurven | `vehicle.curves.list`, `vehicle.curve.evaluate` | Profilierte Funktion „OmsiConstBlock“ und Punktarrays | Pinned Curve Clamp/Linear Interpolation Semantik | PASS: `AI_Wandler_last(0) = 1300`; normale Bereinigung PASS | RUNTIME_PASS |
| Fahrzeug-HOF-Metadaten | `vehicle.hofs.read` | profiliertes HOF-Array mit Fahrzeugdefinition | angehefteter „OmsiHOF“ UTF-16-Name plus ANSI-Servicefahrt | PASS: 11 Einträge, darunter „Grundorf“ und „Betriebsfahrt“; normale Bereinigung PASS | RUNTIME_PASS |
| Treiber | `drivers.read` | Profiliertes Treiberarray | angeheftete „OmsiDriver“-Felder | PASS: ein „OMSI-Fan“-Schnappschuss; normale Bereinigung PASS | RUNTIME_PASS |
| Ticketpaket | `tickets.read` | profilierte Ticketpakete/Ticketdatensätze | angeheftete Felder „OmsiTicketPack“/„OmsiTicket“ | PASS: fünf „Berlin_1“-Ticketdatensätze; normale Bereinigung PASS | RUNTIME_PASS |
| Fahrplanprotokolle | `timetable.logs.read` | Profiliertes dynamisches Protokollarray | „OmsiTimeTableLog“-Datensatz angeheftet | PASS: gültige leere Grundorf-Sammlung; normale Bereinigung PASS | RUNTIME_PASS |
| Zufälligen Bus platzieren | `road-vehicles.place-random` | profilierte native Brücke | Upstream „TProgMan.PlaceRandomBus“ ABI | PASS: Rohrückgabe „2“, RoadVehicles „2 -> 4“; normale Bereinigung PASS | RUNTIME_PASS |
| Benannte Fahrzeug-/Objektauslöser | keine | keine | Upstream-Methoden erfordern einen nicht aufgelösten verwalteten Delphi-String-Besitznachweis | nicht ausgeführt | BLOCKED_STRING_OWNERSHIP |
| Tonauslöser | keine | keine | Die Upstream-Methode erfordert einen nicht aufgelösten verwalteten Delphi-String-Besitznachweis | nicht ausgeführt | BLOCKED_STRING_OWNERSHIP |
| Anzahl menschlicher Sammlungen | `humans.read` | Delphi-Zeiger-Array | gepinnt OmsiHook global | PASS: 408 | RUNTIME_PASS |
| Detaillierte menschliche Telemetrie | `humans.list`, `human.read` | profilierte „OmsiHumanBeingInst“-Schnappschüsse | Upstream-Layouts abgeglichen mit „Omsi23004_692EBFBF“ | PASS: undurchsichtige Griff-, Bewegungs-, Ziel-, Ticket-, Sitzplatz-, Stations- und KI-Felder | RUNTIME_PASS |
| Fahrplanmanager zählt | `timetable.read` | profilierte Arrays | angeheftet „OmsiTimeTableMan“ | PASS: Gleise/Fahrten/Haltestellen/Linien | RUNTIME_PASS |
| Fahrplan Gleise / Fahrten / Linien | `timetable.tracks.list`, `timetable.trips.list`, `timetable.lines.list` | Profilierte „OmsiTT*Internal“-Datensätze mit fester Breite | angeheftete Stundenplandatensätze abgeglichen mit „Omsi23004_692EBFBF“ | PASS: 3 Gleise, 3 Fahrten und 2 Linien; `76_BH-Kk` / `76` Identitäten | RUNTIME_PASS |
| Fahrplan Bushaltestellen / StationLinks | `timetable.bus-stops.list`, `timetable.station-links.list` | Profilierte Datensätze mit fester Breite „OmsiTTBusstopListEntryInternal“ und „OmsiTTStnLinkInternal“ | angeheftete Stundenplandatensätze abgeglichen mit „Omsi23004_692EBFBF“ | PASS: 13 Bushaltestellen inklusive „Nordspitze“; 14 Bahnhofsverbindungen | RUNTIME_PASS |
| Fahrplanführungen | `fahrplan.touren.list` | profilierte verschachtelte „OmsiTTTourInternal“-Arrays unter „Zeilen |“. angeheftete Stundenplandatensätze abgeglichen mit „Omsi23004_692EBFBF“ | PASS: 3 Touren; Linie „0“, Tour „1“, AI-Gruppe „Busse“, 72 TourEntries | RUNTIME_PASS |
| Fahrplanprofile | `timetable.profiles.list` | profilierte verschachtelte „OmsiTTProfileInternal“-Arrays unter Trips | angeheftete Stundenplandatensätze abgeglichen mit „Omsi23004_692EBFBF“ | PASS: 3 Profile; „Standard“, Gesamtzeit 420, 8 Stoppzeiten | RUNTIME_PASS |
| Fahrplan-TourEinträge | `fahrplan.tour-einträge.liste` | Profilierte verschachtelte „OmsiTTTourEntryInternal“-Arrays unter Touren | angeheftete Stundenplandatensätze abgeglichen mit „Omsi23004_692EBFBF“ | PASS: 130 Einträge; „76_BH-Kk“, Reise/Profil 0, 14820 bis 15240 | RUNTIME_PASS |
| Fahrplaneinträge | `fahrplan.track-entries.list` | Profilierte verschachtelte „OmsiTTTrackEntryInternal“-Arrays unter „Tracks |“. fixierte Tr. mit fester BreiteackEntry-Datensatz mit „Omsi23004_692EBFBF“ abgeglichen; begrenzt auf 512 Einträge | PASS: 91 Einträge; erste ID „99“, Kachel „4“, Abstand „0“ in 93 ms | RUNTIME_PASS |
| Fahrplan RVFiles | `timetable.rv-files.list` | profilierte „OmsiRVFileInternal“-Datensätze | Angeheftetes Datums-/Zeilen-/Listen-/Wahrscheinlichkeitslayout mit „Omsi23004_692EBFBF“ abgeglichen | PASS: gültiges leeres Array auf Grundorf in 62 ms | RUNTIME_PASS |
| Fahrplan NoRVNumbers | keine | keine | Upstream-Array-Zugriff „raw:true“ kollidiert mit dem beobachteten „+0x24“-Layout dieses Profils | FAIL: Ungültiger Header des dynamischen Arrays, dann ungültige Rohadresse; kanonische Bereinigung PASS | UNGELÖST |
| Mutation der Fahrzeug-String-Variablen | nicht angekündigt | Delphi-String-Zuweisung/Refcount-Grenze steht aus | Upstream-native Methoden | nicht ausgeführt | BLOCKED_STRING_OWNERSHIP |
| D3D-Gerätestatus | eingegeben `D3DRuntimeApi` / `d3d.status` | Profilslot, geschütztes QI und Beobachter auf kooperativer Ebene | angehefteter DXHook plus Exact-Build-Slot-Validierung | PASS: „S_OK“, BEREIT, eine eigene Referenz, Reset-Hook installiert | RUNTIME_PASS |
| D3D-Textur erstellen/beschreiben | typisiertes undurchsichtiges „D3DTextureHandle“ | Begrenzte native Registrierung, dynamische Standard-Pool-Textur | angeheftete Textursemantik mit „Aktuell |“ abgeglichen PASS: zwei Formate, Level-Metadaten, mehrere Handles | RUNTIME_PASS |
| D3D-Textur-Update | tippte „D3DTextureUpdate“ | ein `GetLevelDesc`, Grenzen, `LockRect`, pitch-aware copy, `UnlockRect` | angeheftetes DXHook-Verhalten ohne Eigentumsmängel | PASS: Vollständige und rechteckige Aktualisierungen haben „S_OK“ zurückgegeben | RUNTIME_PASS |
| D3D-Texturfreigabe | typisierter Freigabevorgang | Atomischer Ruhestand und genau einmaliges COM-Release | Eigenes Referenzdiagramm und Generierungsmodell | PASS: Freigabe, Ablehnung bei wiederholter Freigabe und drei Wiederverwendungszyklen | RUNTIME_PASS |
| D3D-Lebenszyklusereignisse | öffentliche `RuntimeEvents` (`d3d.ready/lost/resetting/restored`) | Vtable-Beobachter plus geordnete feste Übergangswarteschlange zurücksetzen | Aktueller Build-Reset-Slot ABI und D3D9-Reset-Vertrag | `d3d.ready` PASS; Verlust/Zurücksetzen/Wiederherstellen nicht sicher herbeigeführt | TEILWEISE_UNTERSTÜTZT |
| D3D-Ablehnung veralteter Handles | Sitzungsmarkiertes undurchsichtiges Handle plus Gerätegenerierung | Überprüfung der nativen Generierung und des Ressourcenstatus | Sitzungskanal mit fester Breite und Richtlinie zum Zurücksetzen des Standardpools | PASS für freigegebene Handles und Handles aus früheren Sitzungen; Ungültigkeitspfad zurücksetzen, Laufzeit nicht ausgeführt | TEILWEISE_UNTERSTÜTZT |

## Wave-D-Zustand

OmsiLaunch ist Eigentümer der D3D9-Implementierung. Es liest das profilspezifische Gerät
Steckplatz, behält genau eine „QueryInterface<IDirect3DDevice9>“-Referenz und wird ausgeführt
Befehle über den vorhandenen OMSI UI/Main/Render-Owner-Timer und macht keine verfügbar
COM-Zeiger. Standard-Pool-Texturen werden vor dem beobachteten Zurücksetzen ungültig gemacht und
Ältere Generationen zielen niemals auf eine Ersatzressource ab. Normale Texturarbeit,
StopSession, erzwungenes Beenden, Neustart und Ablehnung veralteter Sitzungen sind Laufzeitfunktionen
bewiesen. Geräteverlust, erfolgreicher Reset, wiederholter Verlust/Reset und Request-vs-Reset
Rennen bleiben implementiert, aber nicht laufzeitbewährt, da keine sicheren, wahrheitsgemäßen Angaben vorliegen
Verlust-/Resetproduzent war verfügbar; Es wurde kein synthetischer Reset aufgerufen.

Unangekündigte Operationen werden abgelehnt; Fähigkeitserkennung darf nicht bedeuten, dass a
Die Rohadresse kann sicher geschrieben oder aufgerufen werden.

