> Lokalisierte Übersetzung. Bei technischen Abweichungen gilt die kanonische englische Dokumentation.

# OMSI Interop Surface

Status: NORMATIV

„OmsiLaunch.Interop“ ist die nur für x86 gültige OMSI-ABI-Grenze. Es rekonstruiert die
wiederverwendbare Teile von OmsiHook, ohne den externen Anhang oder RPC von OmsiHook zu importieren
Architektur in das OmsiLaunch-Produkt integrieren.

## Regeln

– Alle OMSI-Zeiger sind explizite vorzeichenlose 32-Bit-Adressen innerhalb von Interop.
– Öffentliche API-Verträge legen niemals Zeiger, Handles, DNNE-Typen oder verwaltete Daten offen
  OMSI-Objekt-Wrapper.
- Das Lesen von Delphi-Strings und -Arrays ist generisch; Schreiben oder Zuordnung erfordert
  ein expliziter profilgesteuerter nativer Allokator.
- Ein Domain-Wrapper wird nur dann ausführbar, wenn das aktive „OmsiBuildProfile“ aktiviert ist
  Bietet die genaue Adresse/den genauen Offset sowie native Byte-Schutzfunktionen.
- Der generische Austausch einer Delphi-Sammlung ist verboten. Nativer Lebenszyklus
  Operationen besitzen Mutationen zu OMSI-Sammlungen.

## Rekonstruiertes Fundament

| Fähigkeit | Staat | Umsetzung |
| --- | --- | --- |
| x86-Remoteadressen mit fester Breite | UMGESETZT | `OmsiRemoteAddress` |
| Lesen/Schreiben des Skalarspeichers | UMGESETZT | `OmsiMemoryPrimitives` |
| Delphi UnicodeString lesen | UMGESETZT | `ReadStringAsync` |
| Delphi ANSI-String gelesen | UMGESETZT | `ReadStringAsync` |
| Delphi-String-/Zeiger-/Struktur-Arrays | UMGESETZT | `OmsiDelphiValues` |
| Remote-String-/Array-Zuordnung | IMPLEMENTIERT, profilnativer Allokator erforderlich | `IOmsiRemoteAllocator` |
| Snapshots der schreibgeschützten Objektsammlung | UMGESETZT | `OmsiObjectCollection<T>` |
| Snapshots der Zeigersammlung „TList“/„OList“ | UMGESETZT | „OmsiPointerList<T>“ mit vom Profil bereitgestelltem Layout |
| `TList`/`OList` Delphi-String-Snapshots | UMGESETZT | „OmsiStringList“ mit vom Profil bereitgestelltem Layout und Kodierung |
| Profilvalidierter Objektfeldzugriff | UMGESETZT | „OmsiProfiledObject“ validiert VMT vor jedem Feldzugriff |
| Strukturreflexions-Marshalling | VERSCHOBEN | Erfordert bestätigte Verpackung/Layout und einen konkreten Verbraucher |
| Prozessanbindung / externer RPC | AUSGESCHLOSSEN | OmsiLaunch ist Eigentümer des Prozesslebenszyklus und der Startup-Übergabe |

„OmsiRuntimeSurface“ listet die von OmsiHook geerbten semantischen Operationen auf
nützliche Domänen. Ein Katalogeintrag ist kein Anspruch, den das aktuelle Profil zulässt
die Operation. Die Verfügbarkeit wird durch „OmsiBuildProfile“ und native Wächter gelöst;
„OmsiRuntimeSurface.Resolve“ macht diese Entscheidung explizit für einen Profiladapter.

