> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Lokales Kontrollprotokoll

„OmsiLaunch.exe /serve“ besitzt eine verwaltete Sitzung und stellt eine nur für den aktuellen Benutzer verfügbare Sitzung bereit
Named-Pipe-Steuerungsendpunkt. Sekundäre „OmsiLaunch.exe“-Aufrufe fungieren als
Kunden; Sie erstellen niemals einen konkurrierenden Host für diese Sitzung.

Protokollversion: „0.1“.

Unterstützte Client-Routen:

- „Sitzungsstatus --json“.
- `session stop --json`
- „Ereignisse lesen --json“.
- „events watch --json“ (bis „Strg+C“)
– Laufzeit-Aliase wie „time get --json“ und „time set --hour=18 --minute=10 --json“.

Jede Anfrage und Antwort ist JSON mit Längenpräfix und auf 64 KiB begrenzt. Die
Der Endpunkt ist nur lokal und verwendet „PipeOptions.CurrentUserOnly“. Es ist getrennt
aus dem Host-zu-Plugin-Postfach, das sitzungs-/prozessgebunden bleibt und dies nicht ist
eine öffentlich ausführbare IPC-Oberfläche.

Wenn kein aktiver Host vorhanden ist, geben Clients „OL_E_NO_ACTIVE_SESSION“ zurück und beenden den Vorgang
mit Code „4“. Eine Client-Anfrage startet OMSI niemals implizit.

Jedes Ergebnis ist ein Umschlag mit „ok“, „command“, „protocol_version“ und
entweder „Ergebnis“ oder ein strukturierter „Fehler“. Der anfängliche stabile Exit-Code-Vertrag
ist: „0“ Erfolg, „2“ ungültige Argumente, „3“ nicht unterstütztes Profil, „4“ nein
aktive Sitzung, „5“ nicht verfügbare Laufzeit, „6“ nicht gefunden, „7“ Vorgang
abgelehnt, „8“ Wiederherstellungsfehler und „10“ interner Fehler. Fehlerkategorien
sind semantisch („invalid_argument“, „unsupported_profile“, „session“,
„runtime“, „not_found“, „transaction“ oder „internal“); Anrufer dürfen nicht analysieren
Für Menschen lesbare Nachrichten.

Laufzeitbefehle erhalten innerhalb der besitzenden Sitzung eindeutige Anforderungs-IDs. Die
Die öffentliche Pipe transportiert keine OMSI-Zeiger und macht den Host-zu-Plugin nicht verfügbar
Postfachname. Alle von einem Laufzeitbefehl zurückgegebenen Handles bleiben sitzungsbezogen
und veralten, wenn der Besitzer die Sitzung stoppt oder schließt.

Unbekannte Laufzeitoperationsnamen werden lokal als abgelehnt
`OL_E_RUNTIME_OPERATION_UNKNOWN`; Die CLI erstellt keine Sitzung und leitet sie nicht weiter
eine nicht erkannte native Operation.

Die derzeit aktive öffentliche Befehlsregistrierung ist verfügbar über:

„Powershell
OmsiLaunch.exe-Funktionen --json
OmsiLaunch.exe-Hilfezeit --json
„

