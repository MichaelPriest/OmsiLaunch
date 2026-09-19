> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Stan wdrożenia

Stan: RUNTIME_NEW_MAP_PASS

| Składnik | stan | Notatki |
| --- | --- | --- |
| Abstrakcja RuntimePlatform | STATICALLY_VALIDATED | Zaimplementowano bieżącego dostawcę systemu Windows x64. |
| Obecny dostawca systemu Windows x64 | STATICALLY_VALIDATED | Weryfikuje platformę przed modyfikowalnymi operacjami środowiska wykonawczego. |
| Zbuduj profil | STATICALLY_VALIDATED | `Omsi23004_692EBFBF` ma zasięg odcisku palca. |
| Natywny.x86 | RUNTIME_VALIDATED | wersja 145 Win32; osłony profilu/oryginalnego bajtu stosowane na ścieżce kanonicznej. |
| Podstawa współdziałania oparta na OmsiHook | STATICALLY_VALIDATED | Ciągi Delphi x86, tablice dynamiczne, migawki obiektów i wyraźna granica zdalnej alokacji; brak nadrzędnego protokołu RPC. |
| Współpraca wtyczek w procesie | STATICALLY_VALIDATED | `PluginRuntime` korzysta z dostawcy pamięci procesowej o zasięgu profilowym; dołączenie zewnętrzne nie jest normalną ścieżką sesji. |
| Kanał poleceń środowiska wykonawczego | STATICALLY_VALIDATED | Powiązana z sesją skrzynka pocztowa o stałej szerokości/SHA-256 z odpowiedzią na żądanie i wysyłana przez licznik czasu wątku OMSI UI. |
| Czytniki map/pogody/kamer w czasie wykonywania | RUNTIME_VALIDATED | Sesja kanoniczna zweryfikowała czas, mapę, w pełni profilowane odczyty skalarne pogody, rzeczywistą pogodę i kamerę za pośrednictwem kanału wtyczek powiązanego z sesją. |
| Zaawansowany wykres mapy/kafelków | BLOCKED_PROFILE_LAYOUT | Etykieta nadrzędna `OmsiMap.Kacheln` pod adresem `Map+0x118` nie została przekształcona w prawidłowy nagłówek tablicy dynamicznej Delphi w trzech strzeżonych sesjach bieżących. Wersja próbna nigdy nie zapisuje, a wszystkie sesje są przywracane normalnie. |
| Zegar wykonawczy/pogoda skalarna zapisuje | RUNTIME_VALIDATED | `time.set` zastosowany profilowany `SetTime` z odczytem zwrotnym; `weather.set` sprawdził białą listę skalarną opartą na profilu i przywrócenie. |
| Zapis FOV kamery w czasie wykonywania | RUNTIME_VALIDATED | `camera.set` zmieniony i przywrócony FOV podczas sesji kanonicznej; rodzina kamer pozostaje strzeżona. |
| Podsumowanie czasu pracy pojazdu/człowieka/rozkładu jazdy | RUNTIME_VALIDATED | Aplikacja Canonical Grundorf wsadowa zwróciła stan pojazdu drogowego, liczbę osób i liczbę tablic menedżera rozkładu jazdy bez ujawniania surowych wskaźników. |
| Szczegółowa telemetria rozkładu jazdy w czasie wykonywania | RUNTIME_VALIDATED | Profilowane, niezmienne migawki zwracały ścieżki, wycieczki i linie z ich nazwami semantycznymi, ścieżkami, licznikami i metadanymi przypisań za pośrednictwem kanału sesji. |
| Przystanki rozkładu jazdy i linki | RUNTIME_VALIDATED | Profilowane, niezmienne migawki zwracały nazwy przystanków autobusowych, identyfikatory, łącza i metadane punktów końcowych łączy stacji. „Nordspitze” został odczytany na żywo z pola UTF-16 Delphi. |
| Wycieczki według rozkładu jazdy | RUNTIME_VALIDATED | Profilowane zagnieżdżone migawki wycieczek zwróciły przynależność linii, grupę/typ AI, metadane rezerwacji pojazdu i liczbę TourEntry bez ujawniania wskaźników Delphi. |
| Profile rozkładów jazdy | RUNTIME_VALIDATED | Zagnieżdżone profilowane migawki profili zwracały informacje o przynależności do podróży, nazwie, całkowitym czasie, czasie zatrzymania i czasie wejścia na ścieżkę. |
| Wpisy w harmonogramie wycieczek | RUNTIME_VALIDATED | Profilowane zagnieżdżone migawki TourEntry zwracały semantyczną tożsamość podróży, indeksy podróży/profilu, czas i stan płynnego przejścia. |
| Wpisy w harmonogramie czasu wykonania | RUNTIME_VALIDATED | Migawki Bounded TrackEntry zwróciły 91 rekordów na żywo z przynależnością do ścieżki, identyfikatorem, kafelkiem/ścieżką, odległościami, ważnością, kolejnością i metadanymi chronologicznymi za pośrednictwem kanału sesji. |
| Harmonogram wykonania RVFiles | RUNTIME_VALIDATED | Profilowana tablica RVFile zwróciła prawidłową pustą kolekcję na Grundorfie za pośrednictwem kanału sesji. |
| Rozkład jazdy NoRVNumbers | NIEROZWIĄZANE | Dekodowanie „raw:true” poprzedzającego powoduje konflikt z obserwowanym układem pola „Omsi23004_692EBFBF”. Usunięto z partii automatycznej; nie ma to wpływu na inną telemetrię rozkładu jazdy. |
| Szczegółowa telemetria pojazdu/człowieka w czasie działania | RUNTIME_VALIDATED | Nieprzezroczyste uchwyty i migawki tylko do odczytu o zasięgu sesji zwracały spójny ruch pojazdu/sterowanie/oświetlenie/AI i stan ludzkiego celu/biletu/siedzenia/stacji/AI na Grundorfie. Nie są włączone żadne zapisy przestrzenne ani dowolne obiekty. |
| Odczyt zmiennej skryptu środowiska wykonawczego | RUNTIME_VALIDATED | Tabele nazw ANSI i inpostawa Tablica wskaźników „PublicVars” rozwiązała 1025 nazw i „Refresh_Strings=0” poprzez nieprzezroczysty uchwyt pojazdu. |
| Mutacja zmiennej skryptu wykonawczego | RUNTIME_VALIDATED | `vehicle.variable.set` rozpoznaje aktywny nieprzezroczysty uchwyt i profilowaną szczelinę `PublicVars` według otwartej nazwy ciągu, odrzuca nieskończone wartości i zwraca natychmiastowy odczyt. `Refresh_Strings 0 -> 1 -> 0` przeszło przy normalnym czyszczeniu. |
| Odczyt zmiennej łańcuchowej czasu wykonania | RUNTIME_VALIDATED | Tabele nazw ANSI plus wartości ciągów Unicode rozwiązały 27 nazw i `ident=GRN-V 30` poprzez nieprzezroczysty uchwyt pojazdu. Zapisy dotyczące wymiany/refundacji Delphi pozostają oddzielne. |
| Czytniki metadanych środowiska wykonawczego Wave A | RUNTIME_VALIDATED | Metadane HOF, kierowcy, pakiety biletów i dzienniki rozkładów jazdy przekazane w sesji `e33aae7e-450a-4c00-8c7a-5f550ac4263f`; stałe i krzywe przekazane w `46260d64-a8e9-4ee9-a907-00e46b07a1ae`, wszystko przy normalnym czyszczeniu. |
| Stała instalacja wtyczki | STATICALLY_VALIDATED | Release ZIP umieszcza zamknięcie wtyczki OmsiLaunch bezpośrednio w `plugins\\`. Sesja weryfikuje dokładne skróty produktów przed uruchomieniem; nigdy nie przygotowuje, nie tworzy migawek, nie przywraca, nie usuwa ani nie zgłasza roszczeń do wtyczek stron trzecich. |
| MiejsceLosowyAutobus | RUNTIME_VALIDATED | Sesja `4e2321d4-4c09-428b-8760-d348205b7392` zwróciła `2`, zwiększyła liczbę pojazdów drogowych z 2 do 4, a następnie zakończyła dokładne czyszczenie. Różni się od make-basic i nie stanowi żadnego roszczenia PlayerVehicle. |
| Nazwane wyzwalacze i mutacja ciągu | BLOCKED_STRING_OWNERSHIP | Wysyłka produkcji jest celowo wstrzymywana do czasu, aż alokacja łańcucha Delphi, przypisanie i czas życia zostaną zamknięte bez dziedziczenia ryzyka wycieku/korupcji na wyższym szczeblu łańcucha dostaw. |
| Katalog operacji domeny OmsiHook | STATICALLY_VALIDATED | Domeny programu, mapy, czasu, pogody, pojazdów, ludzi, rozkładu jazdy, kamery, odtwarzacza i dźwięku są bramkowane semantycznie/profilowo. |
| Urządzenie i tekstury Wave D D3D | RUNTIME_VALIDATED | Pozyskanie urządzenia o dokładnym profilu, jedno posiadane odniesienie QI, nieprzejrzyste uchwyty sesji, utworzenie/opisanie/pełna i poprawna aktualizacja/wydanie, wielokrotne odrzucenie wydania, wiele zasobów, pomyślne wymuszone wyjście i ponowne uruchomienie. |
| Cykl życia/reset Wave D D3D | IMPLEMENTED_NOT_RUNTIME_VALIDATED | Przechwytywanie resetowania unieważnia zasoby domyślnej puli przed wywołaniem natywnym, przyspiesza generowanie i publikuje uporządkowane przejścia cyklu życia. Bezpieczna utrata urządzenia/indukcja resetu była niedostępna, więc scenariusze utraty/resetowania/powtórzonego cyklu/wyścigu pozostają raczej zablokowane niż zgłaszane. |
| Adapter DNNE | STATICALLY_VALIDATED | Bieżący adapter jest oddzielony od PluginRuntime. |
| Czas wykonania wtyczki | RUNTIME_VALIDATED | Zużycie przełączania, synchroniczne ramię bez głowy i ogólne wykonanie NEW_MAP osiągnęły rozgrywkę. |
| Przenośne przekazanie | STATICALLY_VALIDATED | Protokół UTF-8/SHA-256 o stałej szerokości v4 przenosi semantyczny punkt wejścia i tożsamość zapisanych sytuacji; Dekodowanie wersji 3 pozostaje obsługiwane, a testy w obie strony/korupcji przechodzą pomyślnie. |
| Odkrywanie treści | STATICALLY_VALIDATED | Odkrycie mapy/sytuacji/pojazdu/przemalowania/HOF offline oraz nieprzezroczyste, zahaszowane rekordy „[punkty wejścia]” w trybie offline; korelacja uruchomienia punktu wejścia pozostaje oddzielnie bramkowana. |
| Konfiguracja | STATICALLY_VALIDATED | Tylko o zakresie sesji; Bezstratne no-op, negatywne flagi, zakresy, bloki złożone i testy zachowania wektora przechodzą pomyślnie. |
| Katalog instalacyjny `.omsilaunch` | RELEASE_VALIDATED | Stan katalogu jest tworzony leniwie po przejęciu dzierżawy dla zarządzanej prezentacji. Zasoby trwałe znajdują się w „aktywach”; diagnostyka utrzymuje się; trwały dziennik zostanie usunięty po przywróceniu. Nie jest to plik ani miejsce docelowe wdrażania wtyczek. |
| Zarządzana prezentacja powitalna | RELEASE_RUNTIME_VALIDATED | Opcja „Zarządzana” jest ustawieniem domyślnym. Spakowane zasoby PTB/ENG/DEU/FRA 640x480 24-bitowe znajdują się w pliku `.omsilaunch/assets/splash` i transakcyjnie nakładką `GUI/NewSplashscreen_ENG.bmp` plus rozwiązane ustawienia regionalne. Sesje wydawnicze `388cf5d2-7310-4301-9313-e62c4c08a346` (domyślny) i `794cae34-a2ac-4fcf-8981-f142ac14c308` (katalog niestandardowy) osiągnęły poziom rozgrywki i zakończyły normalne czyszczenie. `Wyłącz` sesję `12a74b24-6c78-4863-8ffe-947651212eeb` osiągnął rozgrywkę bez planowanej nakładki GUI. Wszystkie trzy przywróciły oryginalny stan GUI i usunęły dziennik; stałe wtyczki produktów nie były uczestnikami transakcji. |
| Wydanie pakietu | RELEASE_VALIDATED | `tools/New-ReleasePackage.ps1 -Configuration Release` tworzy plik `OmsiLaunch-current.zip` z manifestem SHA-256, zamknięciem środowiska wykonawczego wydania, zasobami powitalnymi, przykładem `omsilaunch` i bez ścieżek debugowania w manifeście. |
| Transakcja/odzyskiwanie | RUNTIME_RECOVERY_VALIDATED | Nieaktualny dziennik „HANDOFF_CREATED” przywrócił tylko artefakty będące własnością sesji i usunął swój dziennik. Weryfikacja właściciela wymaga niezakończonego procesu, czasu utworzenia i ścieżki pliku wykonywalnego, zanim odzyskiwanie zostanie wstrzymane. |
| Zaplanuj sesję | STATICALLY_VALIDATED | Kanoniczny plan Grundorfa rozwiązuje artefakty środowiska wykonawczego w trybie tylko do odczytu. |
| Bezgłowy Start | WDROŻONE / RUNTIME_VALIDATED | Natywny start został uzbrojony synchronicznie, a rozgrywka była obserwowanym stanem docelowym. |
| NOWA_MAPA | WDROŻONE / RUNTIME_VALIDATED | Grundorf, przedstawił indeks 1, a Nordspitze Bauernhof ukończono poprzez rozgrywkę. |
| NEW_MAP tożsamość punktu wejścia | RUNTIME_PARTIAL | Surowe rekordy offline mają stabilne skróty. Środowisko wykonawcze potwierdza, że ​​przedstawiony przez Grundorfa indeks „1” wybiera surowy indeks „0” („Nordspitze Bauernhof”), ale natywny tekst/mapowanie pozostaje nierozwiązany. Żądania dotyczące tożsamości publicznej pozostają zamknięte. |
| ZAPISANA_SYTUACJA | RUNTIME_VALIDATED | `sytuacje\\Baustelle Falkenseer Ch..osn` załadował Berlin-Spandau przez w pełni profilowane radio Start-form, selektor, synchronizację i sekwencję `Button1Click`, osiągnął rozgrywkę, pozostawał DZIAŁANY przez 8 sekund, następnie zakończył żądanie zatrzymania i normalnego przywracania. PlanSession nadal odrzuca pliki `.osn`, których zadeklarowana mapa nie istnieje przed inscenizacją. |
| LAST_MAP_STATE | UNSUPPORTED_FOR_CURRENT_PROFILE | Dokładna natywna gałąź przywracania ostatniej mapy nie jest zamknięta; nie istnieje rezerwowa kolejność plików. |
| Jawna/systemowa data-godzina | STATYCZNIE_CZĘŚCIOWE | Nie wymagane przez regresję kanoniczną. |
| Pojazd gracza | STATYCZNIE_CZĘŚCIOWE | Nie wymagane przez regresję kanoniczną. |
| Implementacja starszej wersji NT6 | PRZYSZŁOŚĆ / NIE DOTYCZY | Tylko granica architektoniczna. |
| Implementacja starszego XP | PRZYSZŁOŚĆ / NIE DOTYCZY | Tylko granica architektoniczna. |

## Kanoniczna linia bazowa środowiska wykonawczego

`RUNTIME_NEW_MAP_PASS`: publiczny `StartSessionAsync` sprawdził poprawność instalacji stałego środowiska wykonawczego i
przenośne przekazanie, osiągnięto `gameplay.entered` dla `maps\Grundorf\global.cfg`
z prezentowanym indeksem punktu wejścia „1” („Nordspitze Bauernhof”), pozostał w trybie URUCHAMIANYM
przez osiem sekund, a następnie wykonał żądane zatrzymanie i normalne dokładne przywrócenie.

Bieżąca dzierżawa instalacji korzysta z nazwanego semafora systemu Windows (maksymalna liczba
one), a nie mutex: czyszczenie może nastąpić w innym zarządzanym wątku. Trwałe
tożsamość dziennika i procesu pozostaje organem odpowiedzialnym za usuwanie awarii.

Domyślny limit czasu uruchamiania wynosi 180 sekund. Zachowuje to ograniczoną awarię
ścieżkę, jednocześnie umożliwiając natywne zapisanie sytuacji na dużych mapach
legalne prace związane z ładowaniem przed grą; osoby wywołujące mogą ustawić krótszy, jawny limit czasu.

## Wydanie prezentacji 001

Kompilacja wersji, zestawy jednostek/integracji, pakiet CLI `PlanSession`, package
manifest i weryfikacja zasobów przebiegła pomyślnie. Spakowane raporty wykonywalne
produkt `OmsiLaunch`, wersja produktu `0.1.0`, protokół `0.1` i wykorzystuje
oficjalna ikona o wielu rozdzielczościach. Utworzono zarządzany początek wydania i dopasowano skrótowo
wszystkie cztery trwałe zasoby powitalne w `.omsilaunch/assets/splash`. Wydanie
Sesje domyślne, niestandardowe i „Nieustawione” osiągnęły poziom rozgrywki, a następnie „Ukończone”.
Domyślne/niestandardowe nakładki i natywna ochrona zostały wybrane przez ich publiczność
LaunchSpec i potwierdzone przez wyemitowane plany. Wszyscy trzej przywrócili oryginał
Stan GUI, usunięto dziennik i zachowano stałą wtyczkę produktu
zamknięcie. The
zachowanie transakcji pod nieobecność miejsca docelowego jest osobno omówione w
`przywrócenie.nieobecnej-nakładki-transakcji`.

