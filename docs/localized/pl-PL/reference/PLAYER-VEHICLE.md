> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Pojazd gracza

Status: WDROŻENIE W TRAKCIE

`PlayerVehicleSpec` używa kanonicznego pojazdu `.bus`, ponownego malowania, HOF, numeru floty,
i tożsamości rejestracyjne. Wykrywanie w trybie offline rozwiązuje te tożsamości
bez przypisywania trwałych indeksów list OMSI.

Bieżący profil rejestruje statyczne prymitywy potrzebne do późniejszego natywnego
utworzenie (`TProgMan.MakeVehicle`, utworzenie/kopiowanie tymczasowej listy pojazdów, losowo
rozmieszczenie autobusów i rozmieszczenie autobusów). Ich opakowanie ABI do sesji nie jest jeszcze dostępne
zaimplementowano, więc każda żądana podmożliwość pojazdu gracza pozostaje zamknięta
niezależnie. Na zapisaną sytuację nie można nałożyć nowego pojazdu gracza
przepływu tworzenia, chyba że jego natywna semantyka wyraźnie tego wymaga.

Przypięty podpis wywołania OmsiHook jest dowodem tylko do momentu, gdy jest wymagany
Dane wejściowe global/list/sekcji krytycznej są uzgadniane z `Omsi23004_692EBFBF`.
Pierwsze ukierunkowane uzgodnienie wykazało niedopasowanie profilu betonu: w górę rzeki
`MakeVehicle` odczytuje swój globalny `ProgMan` pod adresem `0x00862F28`, podczas gdy ten profil
istniejący globalny semantyczny `ProgMan` to `0x00858BDC`. W związku z tym brak upstreamu
adres jest kopiowany do mostka bieżącego. Pozostałe prace związane z zamknięciem to
zidentyfikować dokładnego bieżącego właściciela/globalnego używanego przez natywną ścieżkę tworzenia i
sprawdź czas istnienia listy tymczasowej i różnicę kolekcji po utworzeniu.

