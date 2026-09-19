> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Öffentliche API

Status: NORMATIV

„OmsiLaunch.Api“ ist die Produktgrenze. Verbraucher konstruieren eine Semantik
„LaunchSpec“, rufen Sie „PlanSessionAsync“ auf und rufen Sie dann „StartSessionAsync“ nur für a auf
lauffähiger Plan. Die CLI ist ein Referenzkonsument dieser API und enthält keine
separate OMSI-Implementierung.

Öffentliche Verträge enthalten niemals OMSI-Zeiger, Win32-Handles, DNNE-Typen oder CLR
Objekte, Details zum gemeinsam genutzten Speicher oder von der Zeigerbreite abhängige Werte. Inhalt
Identitäten sind kanonische OMSI-relative Pfade. „GetCapabilitiesAsync“ und a
„SessionPlan“ macht die Verfügbarkeit von Build-Profil-spezifischen Vorgängen verfügbar.

„StartSessionAsync“ kehrt nach Sitzungsbesitz, Transaktions-Staging usw. zurück
Prozessüberwachung etabliert. Es verspricht kein Gameplay. Verbraucher
warte auf „SessionState.Running“; Für Headless-Startanfragen bedeutet dies:
Der Host hat „gameplay.entered“ aus der In-Process-Laufzeit beobachtet.

„World.EntrypointIdentity“ bleibt bis zu seinem strukturierten Rohzustand funktionsabhängig
Die Zuordnung von Einstiegspunkt zu präsentierter Liste ist für das aktive Profil geschlossen. Die
Der native Adapter lehnt bereits fehlende oder mehrdeutig dargestellte Bezeichnungen ab, aber Anrufer
Verwenden Sie in der Zwischenzeit „PresentedEntrypointIndex“ als unterstützten Low-Level-Selektor.

„DiscoverAsync“ ist schreibgeschützt. Add-on-Erkennung ist nur Inventar: OmsiLaunch
Aktiviert, deaktiviert, ändert die Berechtigung nicht und ändert auch keine Steam-Add-ons.

Die gesamte Konfiguration in einer „LaunchSpec“ ist ein temporärer Sitzungsstatus. Die öffentliche API
stellt keine permanente Konfigurationsbearbeitung bereit: jede berührte Konfiguration
Die Datei wird per Transaktions-Snapshot erstellt, protokolliert, überprüft und Byte für Byte wiederhergestellt.

## Sitzungspräsentation

„SessionPresentationSpec.Splash“ ist standardmäßig „verwaltet“. Es wählt die aus
OmsiLaunch-Standard-Splash von „<installation>/.omsilaunch/assets/splash“ und
Überlagert vorübergehend OMSI-GUI-Splash-Dateien. „CustomAssetDirectory“ wählt a aus
Sitzungs-/Projekt-Asset-Verzeichnis; Relative Pfade werden unterhalb der Installation aufgelöst.
„Unset“ (und der Kompatibilitätsalias „Native“) behält explizit das Native bei
OMSI Splash und fügt keine GUI-Splash-Mutation hinzu. Diese Präsentationspolitik bleibt bestehen
sitzungsbezogen; „.omsilaunch“ ist der Produktstatus, keine dauerhafte Bearbeitung von OMSI
Konfiguration.

## Erweiterte D3D9-API

„D3DRuntimeApi“ bietet typisierte Sitzungserweiterungen für Gerätestatus und Textur
erstellen, beschreiben, aktualisieren und freigeben. Öffentliche Modelle sind „D3DDeviceStatus“,
„D3DTextureHandle“, „D3DTextureDescription“, „D3DTextureUpdate“,
„D3DDeviceState“, „D3DTextureFormat“ und „D3DTextureResourceState“.

Handles sind undurchsichtige, sitzungsbezogene Werte. Verbraucher können keine COM erhalten
Zeiger, OMSI-Adresse oder Win32-Handle. Freigegeben, veraltete Generation und
Sitzungsübergreifende Werte schlagen mit strukturierten „OmsiRuntimeException“-Codes fehl. Öffentlich
Der Sitzungsstatus macht auch die begrenzte, geordnete „RuntimeEvents“-Sammlung verfügbar.
das D3D-Lebenszyklusereignisse mit Zeitstempel, Sequenz und semantischen Daten überträgt.

Die typisierten Methoden sind Wrapper für denselben öffentlichen Laufzeitbefehlskanal:

„scharf
var status = waiting launch.GetD3DStatusAsync(session, TimeSpan.FromSeconds(5));
var Textur = Warten auf Start.CreateD3DTextureAsync(
    Sitzung, 64, 64, D3DTextureFormat.A8R8G8B8);
Warten Sie auf den Start.UpdateD3DTextureAsync(session, Texture.Handle,
    neues D3DTextureUpdate(0, 0, 0, 64, 16, Pixel));
Warten Sie auf den Start.ReleaseD3DTextureAsync(session, texture.Handle);
„

Aufrufe sind nur gültig, solange die besitzende Sitzung „LÄUFT“ ist und der Abgleich erfolgt
BuildProfile ist aktiv. Beim Zurücksetzen wird ein Handle nicht neu erstellt oder neu ausgerichtet.

