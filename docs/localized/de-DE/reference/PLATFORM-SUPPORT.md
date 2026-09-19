> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# Plattformunterstützung

Status: NORMATIV

## OmsiLaunch Current

Der offizielle aktuelle Support ist absichtlich eng gefasst: endgültig/letzter Service
Windows 10 x86-64 und aktuell unterstütztes Windows 11 x86-64. Das Host-Betriebssystem und
Der äußere aktuelle Host ist AMD64/x86-64; OMSI, sein In-Process-Plugin und nativ
OMSI-Interop bleibt x86.

Derzeit werden keine 32-Bit-Versionen von Windows, ARM64, Vista, Windows 7, Windows 8 unterstützt.
Windows 8.1, Windows XP, Wine, Proton, Linux oder macOS. Nicht unterstützte Plattformen
werden während der Validierung vor jeder Installationsmutation abgelehnt.

## Laufzeitabhängigkeiten

Die Endstromverteilung sollte soweit möglich in sich geschlossen sein. Benutzer tun es
Sie benötigen kein Visual Studio, das .NET SDK, Git, Python, Ghidra oder CMake. Normal
Für den Betrieb ist keine Erhöhung erforderlich, wenn die OMSI-Installation beschreibbar ist
der aktuelle Benutzer.

## Legacy-Familien

Bei Legacy-Familien handelt es sich um separate historische/Forschungs-Ports, nicht um aktuelle Unterstützung.

### Legacy NT6

Beabsichtigte Ziele sind endgültig gepatchtes Windows Vista SP2 x64, Windows 7 SP1 x64,
Windows 8 x64 und Windows 8.1 x64.

### Legacy XP

Das vorgesehene Ziel ist Windows XP SP3 x86. Windows XP x64 ist kein Ziel.

## Sicherheitspositionierung

Legacy-Releases sind für Offline-Systeme, historische Kompatibilität, kontrolliert
Benchmarks und Reproduzierbarkeitsforschung. Sie werden nicht für den Normalgebrauch empfohlen
Nutzung mit Internetverbindung.

