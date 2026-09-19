> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# OmsiLaunch Beta 0.1 Funktionskatalog

Produktversion: „0.1.0“.  
Steuerprotokoll: „0.1“.  
Unterstütztes ausführbares Profil: „Omsi23004_692EBFBF“ („Omsi.exe“ SHA-256 „692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243“). Der profilierte Steam LAA SHA-256 wird akzeptiert, wartet jedoch noch auf die Validierung des Beta-Laufzeitfelds.

Statusbedeutungen:

- **Unterstützt**: Laufzeitvalidierung für das unterstützte Profil und geeignet für die semantische Beta-API.
- **Experimentell**: implementiert und validiert, wo angegeben, aber profilspezifische Details oder die API-Form können sich während der Betaversion weiterentwickeln.
- **Teilweise**: nützliche Teilmenge ist verfügbar; Die aufgeführten Einschränkungen sind wesentlich.
- **Noch nicht verfügbar**: im öffentlichen Beta-Vertrag absichtlich nicht enthalten.
- **Intern**: Implementierungs-/Testprimitiv, keine öffentliche Beta-Verpflichtung.

| Fähigkeit | Status | API-Stabilität | Laufzeitvalidierung | Bekannte Einschränkung |
|---|---|---|---|---|
| Sitzungsplanung, Start, Status, Warten, Stoppen, Schließen | Unterstützt | Stabile Beta | Pass | Nur ein exaktes OMSI-Profil. |
| Transaktionales Laufzeit-/Konfigurations-Staging und -Wiederherstellung | Unterstützt | Stabile Beta | Pass | Alle Änderungen der Startkonfiguration sind sitzungsbezogen. |
| NEW_MAP-Start durch präsentierten Einstiegspunktindex | Unterstützt | Stabile Beta | Pass | Die Identität des semantischen Einstiegspunkts bleibt unvollständig. |
| SAVED_SITUATION nativer Versand | Experimentell | Experimentell | Pass | Getestete gespeicherte Inhalte müssen in die installierte Karte/den installierten Inhalt aufgelöst werden. |
| LAST_MAP_STATE | Noch nicht verfügbar | N/A | Nicht ausgeführt | Niemals aus der neuesten „.osn“-Datei abgeleitet. |
| Inhaltserkennung und PlanSession | Unterstützt | Stabile Beta | Offline-Pass | Die Erkennung ist profil-/inhaltsabhängig. |
| Zeit gelesen und native Zeit eingestellt | Unterstützt | Stabile Beta | Pass | Die Mutation „Kalender/SetActualDateTime“ ist nicht verfügbar. |
| Wetter-Lesen und Whitelist-Skalar-Schreiben | Experimentell | Experimentell | Pass | Der tatsächliche/ICAO-Lebenszyklus zum Konfigurieren/Anwenden ist nicht verfügbar. |
| Ist-Wetterregler gelesen | Experimentell | Experimentell | Pass | Kein öffentlicher ICAO-Aktivierungs-/Aktualisierungsvorgang. |
| Grundlegender Kartenstatus gelesen | Unterstützt | Stabile Beta | Pass | Erweiterte Kartenmetadaten/Kacheldiagrammfelder sind teilweise. |
| Kamera lesen und begrenztes FOV schreiben | Experimentell | Experimentell | Pass | Die Kameralebenszyklus-/Familiensemantik bleibt profilabhängig. |
| RoadVehicle-Sammlung und Detail-Schnappschüsse | Experimentell | Experimentell | Pass | Keine willkürliche räumliche Verschiebung oder ODE-Neubindung. |
| PlayerVehicle lesen/indexieren | Unterstützt | Stabile Beta | Pass | Das Fahrzeug eines Nullspielers ist gültig; Die deterministische Headless-Zuweisung ist nicht verfügbar. |
| Mensch und Fahrplan-Schnappschüsse | Experimentell | Experimentell | Pass | Einige fortgeschrittene aktuelle Build-Layouts bleiben unvollständig. |
| Numerische Fahrzeugskriptvariablen | Experimentell | Experimentell | Lese-/Schreibdurchlauf | Das Schreiben von String-Variablen ist nicht verfügbar. |
| String-Variable, Konstanten, Kurven, HOF, Treiber, Tickets, Protokolle gelesen | Experimentell | Experimentell | Pass | Mutation verwalteter Zeichenfolgen und benannte Trigger sind nicht verfügbar. |
| `road-vehicles.place-random` | Experimentell | Experimentell | Pass | Mutation in der heimischen Welt; Der Rückgabewert ist eine Diagnose, keine Identität. |
| Grundlegendes MakeVehicle-Grundelement | Intern | Nur intern | Pass | Erstellt/weist kein PlayerVehicle zu. |
| D3D-Status und Texturlebenszyklus | Experimentell | Experimentell | Pass | Der Lebenszyklus von Geräteverlust/-zurücksetzung ist nur teilweise nachgewiesen. |
| Laufzeit-/Sitzungsereignisse | Teilweise | Experimentell | Teilweise | Für D3D-Ereignisse „Verloren/Zurücksetzen/Wiederhergestellt“ fehlt ein echter Laufzeitnachweis. |
| Tastatur-/Controller-/Konfigurations-Overlays | Unterstützt | Stabile Beta | Offline-/Integrationspass | Wird nur für die Sitzung angewendet und Byte für Byte wiederhergestellt. |

Alle Laufzeithandles sind undurchsichtig und sitzungsbezogen. Sie sind keine OMSI-Zeiger, können nach Beendigung der Sitzung nicht wiederverwendet werden und können als veraltet oder freigegeben abgelehnt werden.

Für OmsiLaunch sind keine OmsiHook-Binärdateien, das RPC-Plugin oder die Laufzeitdateien erforderlich. OmsiHook bleibt ein technisches Orakel mit dokumentierter LGPL-Herkunft; Es handelt sich nicht um eine Laufzeitabhängigkeit.

