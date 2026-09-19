> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# OmsiLaunch Beta 0.1 Bekannte Einschränkungen

## Kompatibilität

Beta 0.1 unterstützt nur „Omsi23004_692EBFBF“, identifiziert durch den genauen „Omsi.exe“ SHA-256, der im Funktionskatalog dokumentiert ist. Unbekannte oder nicht profilierte ausführbare Dateien werden abgelehnt. Beta 0.1 erhebt keinen Anspruch auf generische OMSI 2-Unterstützung.

## Weltladung und Fahrzeuge

- „LAST_MAP_STATE“ ist nicht implementiert. OmsiLaunch ersetzt es niemals durch die neueste gespeicherte Situation.
- Präsentierter Einstiegspunktindex wird unterstützt; Die Identität des semantischen Einstiegspunkts bleibt unvollständig.
– Grundlegende native „MakeVehicle“ und „PlaceRandomBus“ sind validiert, aber die deterministische Headless-PlayerVehicle-Platzierung/-Zuweisung ist keine unterstützte öffentliche Funktion.
- Positionsfelder sind einsehbar. Beliebige Fahrzeugverschiebung, kachelübergreifende räumliche Neubindung und ODE-sichere Transformationsberechtigung werden nicht unterstützt.

## Laufzeitmutation

– Numerische Schreibvorgänge für Skriptvariablen werden nur über den profilierten semantischen Pfad unterstützt.
– Das Schreiben von String-Variablen, Trigger mit Fahrzeugnamen und Objekt-Sound-Trigger sind nicht verfügbar, bis eine sichere, von Delphi verwaltete String-Lebensdauer erreicht ist.
– „SetActualDateTime“ und die explizite Kalendermutation sind bis zum Abschluss von ABI/Nachbedingung nicht verfügbar.
- Aktuelles/ICAO-Wetter kann gelesen werden, aber Konfigurieren/Aktivieren/Aktualisieren ist nicht verfügbar.

## Erweiterter Status

- Einige erweiterte Karten-/Kachel-/Pfad-/Spline-/Objektdiagrammfelder des aktuellen Builds sind teilweise, da die „Kacheln“-Darstellung nicht vollständig abgeglichen ist.
- „NoRVNumbers“ und ausgewählte detaillierte Fahrplandarstellungen bleiben teilweise bestehen.
- Erweiterte APIs sind profilgesteuerte typisierte Snapshots, kein Vertrag für willkürlichen Speicherzugriff oder die Verwendung nativer Zeiger.

## D3D

Texturerstellung, -beschreibung, -aktualisierung, -freigabe, Ablehnung veralteter/freigegebener Handles, Bereinigung bei erzwungenem Beenden und Neustart sind laufzeiterprobt. Echtes „DEVICELOST“, OMSI „Reset“, wiederholtes Loss/Reset, Request-versus-Reset-Wettbewerbe und der Beweis für verlorene/zurückgesetzte/wiederhergestellte öffentliche Ereignisse bleiben experimentell/teilweise.

## Sitzungs- und Konfigurationssicherheit

Alle Konfigurations- und temporären OMSI-Dateiänderungen sind sitzungsbezogen. „options.cfg“, „keyboard.cfg“, „gamectrler.cfg“, Splash-Overlays und andere vorübergehend berührte Artefakte werden als Snapshot/Journal/wiederhergestellt. Der permanente OmsiLaunch-Plugin-Abschluss unter „plugins\\OmsiLaunch.*“ wird mit dem Produkt installiert und ist kein Transaktionsteilnehmer. Die dauerhafte Bearbeitung der Konfiguration liegt außerhalb des Beta 0.1-Bereichs.

## Präsentationsressourcen

„.omsilaunch“ ist ein OmsiLaunch-eigenes Installationsverzeichnis, keine Datei oder Datei
Permanente OMSI-Konfigurationsüberschreibung. Seine Standard-Splash-Ressourcen bleiben bestehen als
Produktvermögenswerte; GUI-Ersetzungen „NewSplashscreen_*.bmp“ erfolgen nur sitzungsbezogen und
restauriert. Ein unterbrochener Host ohne dauerhaftes Journal kann keine sichere Schlussfolgerung ziehen
ob eine beliebige verbleibende GUI-Datei im Besitz des Benutzers ist, also bewusste Wiederherstellung
löscht es nicht blind.

