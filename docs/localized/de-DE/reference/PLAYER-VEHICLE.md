> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Spielerfahrzeug

Status: UMSETZUNG IN PROGRESS

„PlayerVehicleSpec“ verwendet kanonisches Fahrzeug „.bus“, Repaint, HOF, Flottennummer,
und Registrierungsidentitäten. Die Offline-Erkennung löst diese Identitäten auf
ohne die Zuweisung persistenter OMSI-Listenindizes.

Das aktuelle Profil zeichnet die statischen Grundelemente auf, die für die spätere native Ausführung benötigt werden
Erstellung („TProgMan.MakeVehicle“, temporäre Fahrzeuglistenerstellung/-kopie, zufällig
Busplatzierung und Buspositionierung). Ihr ABI-to-Session-Wrapper ist noch nicht verfügbar
implementiert, so dass jede angeforderte Spieler-Fahrzeug-Unterfähigkeit geschlossen bleibt
unabhängig. Eine gespeicherte Situation darf nicht mit einem neuen Spielerfahrzeug überlagert werden
Erstellungsfluss, es sei denn, seine native Semantik erfordert dies ausdrücklich.

Die angeheftete OmsiHook-Aufrufsignatur ist nur bis zu jedem Zeitpunkt ein Beweis
Die Eingabe global/list/critical-section wird mit „Omsi23004_692EBFBF“ abgeglichen.
Beim ersten gezielten Abgleich wurde eine konkrete Profilinkongruenz festgestellt: Upstream
„MakeVehicle“ liest seinen „ProgMan“ global bei „0x00862F28“, während dieses Profil
Die vorhandene globale Semantik „ProgMan“ ist „0x00858BDC“. Folglich kein Upstream
Die Adresse wird in die aktuelle Bridge kopiert. Die verbleibenden Schließungsarbeiten sind bis
Identifizieren Sie den genauen aktuellen Eigentümer/Global, der vom nativen Erstellungspfad verwendet wird, und
Validieren Sie die Lebensdauer der temporären Liste und das Delta der Sammlung nach der Erstellung.

