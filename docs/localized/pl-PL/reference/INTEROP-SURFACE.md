> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Powierzchnia współdziałania OMSI

Status: NORMATYWNY

`OmsiLaunch.Interop` to granica OMSI ABI dostępna tylko dla x86. Rekonstruuje
części OmsiHook do wielokrotnego użytku bez importowania zewnętrznego przyłącza OmsiHook lub RPC
architekturę w produkcie OmsiLaunch.

## Zasady

- Wszystkie wskaźniki OMSI są jawnymi 32-bitowymi adresami bez znaku w Interop.
- Publiczne kontrakty API nigdy nie ujawniają wskaźników, uchwytów, typów DNNE ani zarządzanych
  Opakowania obiektów OMSI.
- Odczytywanie ciągów i tablic Delphi ma charakter ogólny; wymaga pisania lub przydziału
  jawny natywny alokator bramkowany profilem.
- Opakowanie domeny staje się wykonywalne tylko wtedy, gdy aktywny jest `OmsiBuildProfile`
  podaje dokładny adres/przesunięcie plus natywne zabezpieczenia bajtów.
- Zabroniona jest wymiana kolekcji Generic Delphi. Natywny cykl życia
  operacje posiadają własne mutacje w kolekcjach OMSI.

## Zrekonstruowany fundament

| Zdolność | stan | Wdrożenie |
| --- | --- | --- |
| Adresy zdalne x86 o stałej szerokości | ZREALIZOWANO | `OmsiRemoteAdres` |
| Odczyt/zapis pamięci skalarnej | ZREALIZOWANO | `Prymitywy OmsiMemory` |
| Delphi UnicodeString odczytany | ZREALIZOWANO | `ReadStringAsync` |
| Odczyt ciągu znaków Delphi ANSI | ZREALIZOWANO | `ReadStringAsync` |
| Tablice łańcuchów/wskaźników/struktur Delphi | ZREALIZOWANO | `Wartości OmsiDelphi` |
| Zdalna alokacja ciągu/tablicy | WDROŻONO, wymagany jest alokator natywny dla profilu | `IOmsiRemoteAllocator` |
| Migawki kolekcji obiektów tylko do odczytu | ZREALIZOWANO | `OmsiObjectCollection<T>` |
| Migawki kolekcji wskaźników `TList`/`OList` | ZREALIZOWANO | `OmsiPointerList<T>` z układem dostarczonym przez profil |
| `TList`/`OList` Migawki ciągów Delphi | ZREALIZOWANO | `OmsiStringList` z układem dostarczonym przez profil i kodowaniem |
| Dostęp do pola obiektu z walidacją profilu | ZREALIZOWANO | `OmsiProfiledObject` sprawdza VMT przed każdym dostępem do pola |
| Zestawianie odbić strukturalnych | ODROCZONE | Wymaga potwierdzonego opakowania/układu i konkretnego konsumenta |
| Dołączenie procesu / zewnętrzne RPC | WYKLUCZONE | OmsiLaunch jest właścicielem cyklu życia procesów i przekazywania uruchamiania |

`OmsiRuntimeSurface` zawiera listę operacji semantycznych odziedziczonych z OmsiHook
przydatne domeny. Wpis do katalogu nie jest roszczeniem, na które pozwala aktualny profil
operacja. Dostępność jest ustalana przez `OmsiBuildProfile` i natywnych strażników;
`OmsiRuntimeSurface.Resolve` wyraźnie określa tę decyzję dla adaptera profilu.

