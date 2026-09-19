> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Tests und Validierung

Sitzungskonfigurationstests erfordern nach Erfolg eine byteidentische Wiederherstellung.
Fehler, Stopp und Wiederherstellung nach veraltetem Journal. Nicht-Laufzeittests decken verlustfrei ab
Optionen, negative Flags, Vektoren, Bereiche, zusammengesetzte Blöcke, benutzerdefinierte Tastatur
Ereignisse, Controller-Erhaltung, Übergabeintegrität, Transaktionswiederherstellung und
die Cross-Thread-Installationsmiete.

„OmsiLaunch.Native.x86.vcxproj“ ist ein eigenes v145 „Debug|Win32“/„Release|Win32“.
Projekt und wird nicht durch den verwalteten „.sln“-Aufruf erstellt. Jeder Laufzeitlauf oder
Der Paketbuild, der „NativeBoundary.cpp“ ändert, muss dieses Projekt erstellen
explizit vor der Inszenierung; Das Bereitstellungsmanifest verbraucht
`artifacts\x86\<Konfiguration>\OmsiLaunch.Native.x86.dll`.

Die Laufzeitvalidierung wird durch die Fähigkeitsmatrix konsolidiert. Der Geschützte zuerst
Grundlinie bleibt „NEW_MAP“ Grundorf, Einstiegspunkt Index 1, Nordspitze Bauernhof,
acht Sekunden LAUFEN, angeforderter Stopp und normale exakte Wiederherstellung.

## Validierung der Release-Präsentation

„tools/Test-ReleaseIdentity.ps1“ extrahiert „OmsiLaunch-current.zip“ und prüft
jedes verteilte OmsiLaunch PE. Es überprüft die allgemeinen Produkt-/Versionsfelder,
Der interne/ursprüngliche Dateiname „OmsiLaunch.exe“ des Controllers und seine eingebettete Datei
Symbol. „nethost.dll“ wurde absichtlich ausgeschlossen, da es sich um eine unveränderte Datei handelt
Microsoft-Laufzeitabhängigkeit, keine OmsiLaunch-Binärdatei.

„tools/Test-ReleasePresentation.ps1“ validiert das extrahierte Release-Paket,
nicht „Artefakte/bin“. Es überprüft, ob sein Manifest die Konfiguration „Release“ hat.
enthält den permanenten „plugins/OmsiLaunch.*“-Abschluss und vier verpackte Splash
Assets unter „.omsilaunch/assets/splash“ und hat kein Debug oder ist veraltet
„runtime/plugin“-Pfad im Manifest. Ohne
„-RunOmsi“ validiert die standardmäßig verwalteten, benutzerdefinierten verwalteten und „Unset“-Pläne.
Mit „-RunOmsi -InstallPackage“ wird nur das produkteigene Paket installiert
root und „plugins/OmsiLaunch.*“-Dateien in das autorisierte Root-Verzeichnis, wird jeweils ausgeführt
Über die installierte Release-CLI wird das temporäre GUI-Overlay überprüft, während die
Sitzung ist aktiv, bytegenaue GUI-Wiederherstellung, kein verbleibender OMSI-Prozess, nein
Journal und unveränderte Plugin-Hashes von Drittanbietern. Es schreibt seine Beweise unten
das Installationsverzeichnis „.omsilaunch/diagnostics“.

Wave D D3D-Beweise verwenden denselben öffentlichen „StartSessionAsync“-Pfad. Sitzung
„3cbd7bb6-d73f-4853-adc8-1213200527d2“ validierter Geräteerwerb, einer im Besitz
QI-Referenz, Reset-Hook-Installation, erstellen/beschreiben, vollständig und rechteckig
Updates, mehrere Ressourcen, deterministische Ablehnung wiederholter Veröffentlichungen und
drei Erstellungs-/Freigabezyklen im OMSI-Thread „16676“. Sitzung
„fe1e10fd-cc79-484c-aa04-41fc40fef8da“ deckte den erzwungenen Prozessabbruch mit einem Live ab
Textur; „a95d9fb4-66c6-4f7a-8a64-a18d8e3f9225“ erwies sich als veraltet in der vorherigen Sitzung
Behandeln Sie die Ablehnung nach dem Neustart. Jeder Lauf endete ohne OMSI-Prozess, Journal,
Leasing; „plugins\OmsiLaunch.*“ bleibt als permanente Produktinstallation bestehen.
Geräteverlust-/Reset-Tests sind blockiert
bis ein sicherer nativer Lebenszyklusproduzent verfügbar ist; Rufen Sie Reset direkt auf
aus dem Validierungskabelbaum ist kein akzeptabler Ersatz.

