> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

<p align="center">
  <img src="assets/branding/omsilaunch-logo.png" alt="OmsiLaunch" width="620">
</p>

<p align="center"><strong>Sitzungssteuerung für OMSI 2.</strong></p>
<p align="center">Open Source · Programmierbar · Community-gesteuert</p>

---

# OmsiLaunch

**OmsiLaunch** ist ein programmierbarer Open-Source-Start, Sitzungsverwaltung,
und Laufzeitsteuerungsschicht für OMSI 2. Es bietet eine öffentliche API, eine funktionale
CLI, ein In-Process-OMSI-Plugin/eine In-Process-OMSI-Laufzeit und ein exakt erstelltes „BuildProfile“.
Grenze. Es handelt sich um eine Infrastruktur für Launcher, Tools, Automatisierung und Community
Integrationen statt eines grafischen Launchers.

> **Definieren Sie die Sitzung, nicht die Klicks.**

## Beta 0.1

Die erste öffentliche Beta ist **0.1.0-beta1**. Es unterstützt das exakte OMSI-Profil
„Omsi23004_692EBFBF“, validiert den ausführbaren Fingerabdruck vor dem Start und
erhebt keinen Anspruch auf Kompatibilität mit unbekannten OMSI-Builds. Das Paket und detailliert
Kompatibilitätsstatus sind in [`docs/`](../README.md) dokumentiert.

OmsiLaunch unterstützt die semantische Sitzungsplanung mit „LaunchSpec“ und
„PlanSession“, Hochfahren durch „StartSession“, beobachtete Laufzeitsteuerung und
normale „StopSession“/„CloseSession“-Bereinigung. Unterstützte Laufzeitfunktionen sind
explizit katalogisiert; experimentelle und nicht verfügbare Funktionen werden nicht ausgeblendet.

## Sicherer Sitzungsbesitz

Alle Überschreibungen der Startkonfiguration sind sitzungsbezogen. OmsiLaunch-Snapshots,
Protokolliert, wendet, überprüft und stellt jede temporäre Konfiguration oder GUI wieder her
Datei, die es ändert. In dieser Betaversion ist keine permanente Konfigurationsbearbeitung möglich.

Produkt-Plugin-Dateien werden dauerhaft unter „plugins\\OmsiLaunch.*“ installiert.
Sie werden nicht für jede Sitzung kopiert und entfernt, ebenso wie Plugins von Drittanbietern
sind niemals Eigentum einer Transaktion. `.omsilaunch\\` ist ein OmsiLaunch-privat
Verzeichnis für Assets, Diagnosen, Journale und Benutzerbeispiele.

Die verwaltete Splash-Präsentation ist die Standardeinstellung. PTB-, ENG-, DEU- und FRA-Produkt
Vermögenswerte werden temporär überlagert und exakt wiederhergestellt. Nativer/„Unset“-Splash
Der Modus behält OMSI-Dateien bei.

## Herunterladen

Laden Sie **`OmsiLaunch-0.1.0-beta1.zip`** von der Projekt-Release-Seite herunter und
Extrahieren Sie es direkt in das unterstützte OMSI-Stammverzeichnis. Das Paket beinhaltet die
Controller, seine erforderlichen Abhängigkeiten, der permanente Plugin-Abschluss, Splash
Assets, ein Beispiel einer Release-Sitzung und ein kleines Offline-Benutzerhandbuch.

Siehe [Installation](../README.md) und
[Erste Sitzung](../README.md). Benutzen
„OmsiLaunch.exe /version“, um den installierten Controller zu überprüfen.

## Für Entwickler

Die bevorzugte Produktoberfläche ist die semantische öffentliche API. Die CLI ist eine
Referenz-Frontend über dieselbe API; es enthält keine separate OMSI-Logik.
Die Laufzeitsteuerung ist sitzungsbezogen, profilvalidiert und verwendet undurchsichtige Semantik
Handles statt öffentlicher nativer Zeiger.

- [Öffentliche API](../README.md)
- [Laufzeitsteuerung](../README.md)
- [Fähigkeitskatalog](../README.md)
- [Bekannte Einschränkungen](../README.md)
- [Profilrichtlinie erstellen](../README.md)

## Community und Lizenz

OmsiLaunch ist ein Community-orientiertes Open-Source-Projekt. Es ist unabhängig von
andere OMSI-Launcher und können von kompatiblen Community-Tools genutzt werden.

OmsiLaunch ist unter [nur LGPL-3.0] (LIZENZ) lizenziert. Siehe die
[Hinweise Dritter](../README.md) für „incorporated-source“.
Herkunft und entsprechende Hinweise.

