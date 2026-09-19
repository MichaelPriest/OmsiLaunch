> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Znane ograniczenia OmsiLaunch Beta 0.1

## Zgodność

Beta 0.1 obsługuje tylko `Omsi23004_692EBFBF`, identyfikowany przez dokładny plik `Omsi.exe` SHA-256 udokumentowany w katalogu możliwości. Nieznane lub nieprofilowane pliki wykonywalne są odrzucane; Wersja Beta 0.1 nie wymaga ogólnej obsługi OMSI 2.

## Ładowanie świata i pojazdy

- `LAST_MAP_STATE` nie jest zaimplementowany. OmsiLaunch nigdy nie zastępuje najnowszą zapisaną sytuacją.
- Obsługiwany jest prezentowany indeks punktu wejścia; tożsamość semantycznego punktu wejścia pozostaje częściowa.
- Podstawowe natywne funkcje „MakeVehicle” i „PlaceRandomBus” są sprawdzane, ale deterministyczne umieszczanie/przypisywanie bezgłowych PlayerVehicle nie jest obsługiwaną funkcją publiczną.
- Pola pozycji można sprawdzić. Dowolna relokacja pojazdu, ponowne wiązanie przestrzenne między płytkami i uprawnienia do transformacji bezpiecznej dla ODE nie są obsługiwane.

## Mutacja w czasie wykonywania

- Zapisy numerycznych zmiennych skryptowych są obsługiwane tylko poprzez profilowaną ścieżkę semantyczną.
- Zapisy zmiennych łańcuchowych, wyzwalacze nazwane pojazdami i wyzwalacze dźwiękowe obiektów są niedostępne w oczekiwaniu na bezpieczną granicę czasu życia zarządzanego przez Delphi.
- `SetActualDateTime` i jawna mutacja kalendarza są niedostępne do czasu zamknięcia ABI/warunku końcowego.
- Można odczytać aktualną pogodę/pogodę ICAO, ale konfiguracja/aktywacja/odświeżenie są niedostępne.

## Stan zaawansowany

- Niektóre pola wykresów zaawansowanej mapy/kafelka/ścieżki/splajnu/obiektu aktualnie tworzonej mapy są częściowe, ponieważ reprezentacja „Kacheln” nie jest w pełni uzgodniona.
- „NoRVNumbers” i wybrane szczegółowe przedstawienia rozkładu jazdy pozostają częściowe.
- Zaawansowane interfejsy API to migawki typu bramkowanego profilem, a nie umowa na dowolny dostęp do pamięci lub użycie natywnego wskaźnika.

##D3D

Tworzenie tekstur, opisywanie, aktualizacja, wydawanie, odrzucanie przestarzałych/zwolnionych uchwytów, wymuszone czyszczenie i ponowne uruchamianie są sprawdzone w czasie wykonywania. Prawdziwe „DEVICELOST”, „Reset” OMSI, powtarzająca się utrata/reset, wyścigi żądanie kontra resetowanie oraz dowód utraty/resetowania/przywrócenia w przypadku wydarzeń publicznych pozostają eksperymentalne/częściowe.

## Bezpieczeństwo sesji i konfiguracji

Wszystkie zmiany konfiguracji i tymczasowe pliki OMSI mają zasięg sesji. `options.cfg`, `keyboard.cfg`, `gamectrler.cfg`, nakładki powitalne i inne tymczasowo dotknięte artefakty są migawką/dziennikiem/przywracane. Trwałe zamknięcie wtyczki OmsiLaunch w obszarze `plugins\\OmsiLaunch.*` jest instalowane wraz z produktem i nie jest uczestnikiem transakcji. Stała edycja konfiguracji wykracza poza zakres wersji Beta 0.1.

## Zasoby prezentacji

`.omsilaunch` jest katalogiem instalacyjnym należącym do OmsiLaunch, a nie plikiem lub
trwałe nadpisanie konfiguracji OMSI. Domyślne zasoby powitalne pozostają takie same
aktywa produktowe; Zamienniki GUI `NewSplashscreen_*.bmp` dotyczą tylko sesji i
przywrócony. Przerwany host bez trwałego dziennika nie może bezpiecznie wnioskować
czy dowolny pozostały plik GUI jest własnością użytkownika, więc odzyskuj celowo
nie usuwa go na ślepo.

