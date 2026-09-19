> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Testowanie i walidacja

Testy konfiguracji sesji wymagają po pomyślnym przywróceniu identycznych bajtów,
awaria, zatrzymanie i odzyskiwanie nieaktualnych dzienników. Testy inne niż wykonawcze obejmują bezstratne
opcje, flagi ujemne, wektory, zakresy, bloki złożone, niestandardowa klawiatura
zdarzenia, zachowanie kontrolera, integralność przekazania, przywrócenie transakcji i
dzierżawę instalacji międzywątkowej.

`OmsiLaunch.Native.x86.vcxproj` jest własnością v145 `Debug|Win32`/`Release|Win32`
projektu i nie jest budowany przez zarządzane wywołanie `.sln`. Dowolne uruchomienie w czasie wykonywania lub
kompilacja pakietu wprowadzająca zmiany `NativeBoundary.cpp` musi zbudować ten projekt
wyraźnie przed inscenizacją; manifest wdrożenia zużywa
`artefakty\x86\<Konfiguracja>\OmsiLaunch.Native.x86.dll`.

Walidacja środowiska wykonawczego jest konsolidowana za pomocą macierzy możliwości. Najpierw chronieni
linia bazowa pozostaje „NEW_MAP” Grundorf, indeks punktu wejścia 1, Nordspitze Bauernhof,
osiem sekund DZIAŁA, żądane zatrzymanie i normalne dokładne przywrócenie.

## Wydaj weryfikację prezentacji

`tools/Test-ReleaseIdentity.ps1` wyodrębnia plik `OmsiLaunch-current.zip` i przeprowadza audyty
każdy dystrybuowany program OmsiLaunch PE. Weryfikuje wspólne pola produktu/wersji,
wewnętrzna/oryginalna nazwa pliku `OmsiLaunch.exe` kontrolera i jego osadzenie
ikona. Plik `nethost.dll` został celowo wykluczony, ponieważ jest niezmodyfikowany
Zależność środowiska wykonawczego Microsoft, a nie plik binarny OmsiLaunch.

`tools/Test-ReleasePresentation.ps1` sprawdza wypakowany pakiet Release,
nie „artefakty/kosz”. Sprawdza, czy jego manifest ma konfigurację „Release”,
zawiera trwałe zamknięcie `plugins/OmsiLaunch.*` i cztery pakiety powitalne
aktywa w `.omsilaunch/assets/splash` i nie ma debugowania ani przestarzałości
Ścieżka „runtime/plugin” w manifeście. Bez
`-RunOmsi` sprawdza plany zarządzane domyślnie, zarządzane niestandardowo i `Nieustawione`.
Dzięki `-RunOmsi -InstallPackage` instaluje tylko produkty należące do pakietu
root i `plugins/OmsiLaunch.*` do autoryzowanego katalogu głównego, uruchamia każdą sprawę
za pośrednictwem zainstalowanego interfejsu CLI wydania sprawdza tymczasową nakładkę GUI podczas
sesja jest aktywna, przywracanie GUI z dokładnością do bajtów, brak pozostałych procesów OMSI, nie
journal i niezmienione skróty wtyczek innych firm. Zapisuje swoje dowody pod
katalog instalacyjny `.omsilaunch/diagnostics`.

Dowody Wave D D3D korzystają z tej samej publicznej ścieżki „StartSessionAsync”. Sesja
`3cbd7bb6-d73f-4853-adc8-1213200527d2` potwierdzone pozyskanie urządzenia, jedno posiadane
Odniesienie QI, instalacja resetowania-haka, tworzenie/opisywanie, pełne i prostokątne
aktualizacje, wiele zasobów, deterministyczne odrzucanie ponownych wydań i
trzy cykle tworzenia/wydawania w wątku OMSI „16676”. Sesja
`fe1e10fd-cc79-484c-aa04-41fc40fef8da` obejmował wymuszone wyjście z procesu na żywo
tekstura; `a95d9fb4-66c6-4f7a-8a64-a18d8e3f9225` okazało się nieaktualne z poprzedniej sesji
obsłuż odrzucenie po ponownym uruchomieniu. Każde uruchomienie zakończyło się brakiem procesu OMSI, dziennika,
dzierżawa; `plugins\OmsiLaunch.*` pozostaje stałą instalacją produktu.
Testy utraty urządzenia/resetu są zablokowane
do czasu pojawienia się bezpiecznego rodzimego producenta cyklu życia; dzwoniąc bezpośrednio do Resetu
z uprzęży walidacyjnej nie jest akceptowalnym substytutem.

