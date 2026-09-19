> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Profile erstellen

Status: NORMATIV

„OmsiBuildProfile“ ist unabhängig von „RuntimePlatform“. Es besitzt die ausführbare OMSI-Datei
Fingerabdruck, Globals, Formulare, Felder, Methoden, Callsites und native Byte-Guards.
Es kodiert niemals die Kompatibilität des Hosts mit der Windows-Generation.

„Omsi23004_692EBFBF“ akzeptiert nur diese bekannten LAA SHA-256-Werte:

- `692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243`
  („ALTERNATE_LAA“, laufzeitvalidiert);
- „7DAB063D1F62E73B3A2C7A6AC1921D7EDF5E5DB0FBC731481D117EEC8DE7D759“.
  („STEAM_LAA“, statisch abgeglichen; Validierung des Beta-Laufzeitfelds steht aus).

Es gibt keine Versionszeichenfolge, keinen Namen der ausführbaren Datei, keine Dateigröße oder keinen generischen LAA-Marker
Akzeptanzkriterium. Jeder andere Hash wird vor dem nativen Laufzeit-Staging abgelehnt.
Das Profil zeichnet „SetActualDateTime“, den Versand gespeicherter Situationen und die auf
Fahrzeugprimitive, die in statischen/stromaufwärts gelegenen Beweisen beobachtet wurden. Das Aufzeichnen eines Symbols ist
keine Berechtigung zum Aufrufen: Jeder native ABI-Wrapper benötigt seinen eigenen
Fingerabdruckschutz und bewährter Anrufvertrag.

Das Profil zeichnet auch den aktuellen D3D-Kandidatensteckplatz „0x008627D0“ auf
„D3DDevice“. Die Laufzeitsitzung „3cbd7bb6-d73f-4853-adc8-1213200527d2“ hat sich bewährt
dass der geliehene Slot-Kandidat „QueryInterface<IDirect3DDevice9>“ unterstützt,
„TestCooperativeLevel“ gibt „S_OK“ zurück und alle D3D-Vorgänge werden auf dem ausgeführt
OMSI primärer UI/Main/Render-Owner-Thread. OmsiLaunch behält eine QI-Referenz;
Der geliehene Slot selbst wird niemals freigegeben. Dieser Eintrag ist nicht verallgemeinert
ein weiterer ausführbarer Fingerabdruck.

Der passende statische Aufrufer für „TProgMan.MakeVehicle“ verweist auf den Kandidaten
Globals bei „0x00859DEC“, „0x008591DC“ und „0x00858D28“. Ihre genaue Liste/Index
Rollen bleiben ungelöst, daher werden sie bewusst nicht BuildProfile genannt
Symbole. Sie sind keine öffentlichen Hinweise und stellen keine vollständige Information dar
Spieler-Fahrzeug-Implementierung.

