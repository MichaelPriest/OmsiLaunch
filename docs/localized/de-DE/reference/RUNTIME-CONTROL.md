> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Laufzeitsteuerung

Status: NORMATIV

Die OmsiLaunch-Laufzeitsteuerung ist sitzungsbezogen. Es ist kein OmsiHook RPC
Kompatibilitätsmodus und es handelt sich nicht um eine Architektur zum Anhängen externer Prozesse.

„
OmsiLaunch API/CLI -> Live-Sitzung -> Laufzeitpostfach -> PluginRuntime
    -> OMSI UI-Thread-Gateway -> Profiliertes Interop / Native.x86 -> OMSI
„

Die Startübergabe erfolgt weiterhin nur beim Start. Laufzeitbefehle verwenden eine separate
versioniertes Postfach, das an die Sitzungs-GUID und den gestarteten OMSI-Prozess gebunden ist.
Jede Anfrage verfügt über eine Anfrage-ID mit fester Breite, UTF-8-Nutzlast und SHA-256-Integrität
Überprüfung, Timeout und Antwortidentitätsvalidierung. Zuvor wird ein Mapping erstellt
„CreateProcessW“ wird nur über die untergeordnete Umgebung bereitgestellt und ist es auch
Wird bei PluginFinalize, Prozessbeendigung, Sitzungsbereinigung oder fehlgeschlagenem Start entsorgt.

„RuntimeCommand“ besteht aus semantischen Daten: Operationsname plus UTF-8-Schlüssel/Wert
Argumente. Es enthält keine OMSI-Zeiger, Win32-Handles, DNNE-Objekte oder CLR
Objektserialisierung. Das Plugin sendet Befehle von seinem vorhandenen OMSI
UI-Thread-Timer; IPC-Mitarbeiter rufen Delphi/Borland-Methoden niemals direkt auf.

Die Laufzeitsteuerung ist sitzungsbezogen. Die CLI ist ein dünnes Referenz-Frontend
`IOmsiLaunch.ExecuteRuntimeAsync`; es wird niemals unabhängig befestigt oder eingebettet
OMSI-spezifische Logik.

Nachdem eine deklarative Startanforderung „RUNNING“ erreicht hat, ein einzelner semantischer Befehl
kann ausgestellt werden mit:

„Text
OmsiLaunch.Cli <Installation> /new /map:maps\Grundorf\global.cfg \
  /entrypoint-index:1 /no-vehicle /runtime:time.read
OmsiLaunch.Cli <Installation> /new /map:maps\Grundorf\global.cfg \
  /entrypoint-index:1 /no-vehicle /runtime:time.set /runtime-arg:minute=18
„

„/runtime-arg“ wird für zusätzliche semantische Felder wiederholt. Jeder Befehl wird weitergeleitet
Das sitzungsgebundene Anforderungs-/Antwortpostfach wird von „PluginRuntime“ ausgeführt
der OMSI UI-Timer und wird abgelehnt, wenn die Sitzungsbindung oder Anforderungsidentität erfolgt
stimmt nicht überein. Der Befehl ist kein persistenter Konfigurationseditor.

Validierte Befehle für „Omsi23004_692EBFBF“ sind „time.read“, „time.set“,
`map.read`, `weather.read`, `weather.set`, `weather.actual.read`, `camera.read`,
`road-vehicles.read`, `road-vehicles.list`, `road-vehicle.read`,
`player-vehicle.read`, `vehicle.variables.list`, `vehicle.variable.get`,
`vehicle.string-variables.list`, `vehicle.string-variable.get`, `humans.read`,
`humans.list`, `human.read`, `timetable.read`, `timetable.tracks.list`,
`timetable.trips.list`, `timetable.lines.list`, `timetable.bus-stops.list`,
„timetable.station-links.list“, „timetable.tours.list“ und
`timetable.profiles.list`, `timetable.tour-entries.list`, `d3d.status`,
„d3d.texture.create“, „d3d.texture.describe“, „d3d.texture.update“ und
`d3d.textur.release`. Alle Fahrplanoperationen sind begrenzt und schreibgeschützt.
Argumente für Mutationen werden vom Profiladapter bewusst auf die Whitelist gesetzt.

„timetable.track-entries.list“ ist eine validierte, begrenzte, unveränderliche Momentaufnahme von
die semantische Pfadidentität und zeitrelevante Metadaten jedes Titeleintrags. Die
Der kanonische Grundorf-Batch gab 91 Datensätze über den Sitzungskanal zurück.

„timetable.rv-files.list“ ist ein validierter begrenzter Snapshot. Grundorf ist aktiv
Die Laufzeit hat eine gültige leere Sammlung zurückgegeben. „NoRVNumbers“ ist absichtlich nicht
veröffentlicht: die angeheftete Upstream-Interpretation „raw:true“ des Kandidaten
Das Feld steht in Konflikt mit dem beobachteten „Omsi23004_692EBFBF“-Layout und bleibt daher bestehen
Profil-ungelöst, anstatt einen unsicheren Lesevorgang offenzulegen.

„vehicle.variable.set“ wird zur Laufzeit über denselben profilbezogenen Kanal validiert.
Es akzeptiert ein undurchsichtiges „Handle“, einen „Namen“ mit offener Zeichenfolge und einen endlichen Gleitkomma-„Wert“.
Löst den aktuellen Steckplatz für öffentliche Variablen auf und gibt dann einen sofortigen Rücklesevorgang zurück.

Profilierte Entitäts-Snapshots verwenden undurchsichtige Handles, die von „road-vehicles.list“ zurückgegeben werden
und „humans.list“, dann von „road-vehicle.read“ und „human.read“ mit verbraucht
`/runtime-arg:handle=<handle>`. Ein Handle ist nur für seine besitzende Sitzung gültig
und wird abgelehnt, wenn sein natives Objekt nicht mehr im aktuellen vorhanden ist
Sammlung. Es ist kein Hinweis, List-Index oder persistente Identität.

## D3D9 Erweiterte Steuerung

Die öffentliche typisierte Oberfläche in „D3DRuntimeApi“ umschließt die semantischen D3D-Operationen.
„D3DTextureHandle“ enthält ein Sitzungs-Tag und eine interne Gerätegenerierung
Token; Es ist weder ein COM-Zeiger noch nach der Freigabe oder dem Zurücksetzen des Geräts gültig
Ersetzen, Beenden des Prozesses oder Starten einer anderen Sitzung.

Das Plugin erhält den exakten Profilkandidaten von „D3DDevice“ und behält einen
QI-Referenz und leitet alle Texturaufrufe vom vorhandenen OMSI ab
UI/Main/Render-Owner-Timer. Texturen sind dynamisch „D3DPOOL_DEFAULT“ begrenzt
Ressourcen. Aktualisierungen validieren Ebene, Rechteck und Nutzlast und verwenden sie dann
„LockRect“/„UnlockRect“ mit pitchbewussten Zeilenkopien. Die Postfachnutzlast ist
begrenzt auf 48 KiB; Größere Textur-Uploads werden als mehrere Rechtecke ausgedrückt
Aktualisierungen.

Der Lebenszyklusbeobachter veröffentlicht „d3d.ready“, „d3d.lost“, „d3d.resetting“ und
„d3d.restored“ in „SessionStatus.RuntimeEvents“. Beim Zurücksetzen werden alle Live-Vorgänge ungültig
Standard-Pool-Ressourcen vor dem Aufrufen der ursprünglichen nativen Methode und Fortschritte
der Gerätegeneration. Geräteverlust- und Reset-Pfade sind implementiert, jedoch nicht
laufzeitbewährt, da Wave D keinen sicheren Produzenten für diese nativen Versionen hatte
Übergänge. `d3d.ready`, normale Texturoperationen, StopSession, erzwungenes Beenden,
Relaunch-Bereinigung, freigegebene Handles und sitzungsübergreifende veraltete Handles sind bewährt.

