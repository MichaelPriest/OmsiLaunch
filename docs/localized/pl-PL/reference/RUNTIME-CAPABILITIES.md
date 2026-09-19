> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Możliwości wykonawcze

Status: INWENTARYZACJA WDROŻENIOWA

| Zdolność | API/kanał | Współpraca | Dowody statyczne | Dowody wykonawcze | Stan wydania |
| --- | --- | --- | --- | --- | --- |
| Żądanie/odpowiedź na sesję | `ExecuteRuntimeAsync` | niezależna od profilu skrzynka pocztowa | testy protokołów i wtyczek | partia kanoniczna PASS | RUNTIME_PASS |
| Czas czytać | `czas.czytania` | `OmsiTimeAdapter` | przypięty OmsiHook plus dokładny profil | PRZEJDŹ | RUNTIME_PASS |
| Mapa przeczytana | `mapa.czytaj` | `OmsiRuntimeReaders` | przypięty układ `OmsiMap` | PRZECHODZIĆ; stan załadowania i zwrócone kafelki | RUNTIME_PASS |
| Mapa/wykres kafelkowy | żaden | profil próbny przechowywany wewnętrznie | upstream oznacza `OmsiMap+0x118` jako `Kacheln`, ale bieżąca pamięć na żywo nie ujawniła prawidłowego nagłówka tablicy Delphi | próba `map.tiles.list` zakończyła się pomyślnie niepowodzeniem w sesjach `ec85b51a-f69a-4f05-8fb3-638c63f9f20d`, `129482a4-b3a2-448a-830e-4043d11de246`, oraz `90011081-f5e9-4b31-b907-941bb3a9fd1a`; wszystko wyczyszczone normalnie | BLOCKED_PROFILE_LAYOUT |
| Pogoda czytaj | `pogoda.czytaj` | `OmsiRuntimeReaders` | przypięte układy rekordów `OmsiWeather` i aktywnej pogody | PASS: podstawowe skalary plus temperatura, punkt rosy, ciśnienie, opady i natężenie | RUNTIME_PASS |
| Aktualna pogoda/odczyt pogody ICAO | `aktualna.pogoda.przeczytana` | `OmsiRuntimeReaders` | przypięty układ `OmsiActuWeather` | PRZEJDŹ | RUNTIME_PASS |
| Kamera czyta | `odczyt aparatu` | `OmsiRuntimeReaders` | przypięte opakowanie i dowody z rodziny aparatów | PRZEJDŹ | RUNTIME_PASS |
| Mutacja FOV kamery | `zestaw aparatu` | `OmsiCameraWriter` | układ skalarny przypiętej kamery | PRZEJDŹ: `45 -> 46 -> 45` | RUNTIME_PASS |
| Mutacja zegara | `zestaw czasu` | `OmsiTimeAdapter` + profilowany `SetTime` | pola zegara upstream i wywołanie natywne | PODAJ: minuta `17 -> 18 -> 17` | RUNTIME_PASS |
| Skalarna mutacja pogody | `zestaw pogody` | Biała lista `OmsiWeatherWriter` | górne pola skalarne i układy profili | PASS: wiatr `0 -> 1 -> 0` | RUNTIME_PASS |
| Semantyczny przedstawiony punkt wejścia | ogrodzony | dokładne, unikalne dopasowanie natywnej listy `Tform_setpos` | profilowany `FormShow`, tekst elementu listy i przepływ `Button1Click` | Częściowe: potwierdzono przebieg kanoniczny „przedstawiony indeks 1 -> surowy indeks 0 -> Nordspitze Bauernhof”; przedstawiona reprezentacja tekstowa pozostaje nierozwiązana, a surowe etykiety są powielane | RUNTIME_PARTIAL |
| Zapisane uruchomienie sytuacji | `WorldMode.SavedSituation` | profilowany Tryb startowy/selektor/Przycisk1Kliknięcie sekwencji | `Tform_start+0x3E4/+0x3F8/+0x434/+0x438/+0x43C`, natywny `SetChecked`, ustawiający indeks pozycji VMT i `Button1Click` | PASS: `sytuacje\\Baustelle Falkenseer Ch..osn` załadował Berlin-Spandau, osiągnął `gameplay.entered`, pozostał DZIAŁANY przez 8 s, następnie zakończył żądanie zatrzymania i normalnego przywrócenia | RUNTIME_PASS |
| Kalendarz / faktyczna mutacja ICAO | żaden | Łańcuch Delphi/granica ABI niekompletna | odniesienia do metod/profilów | nie biegać | RELEASE_IF_CLOSED |
| Stan odbioru pojazdu drogowego | `pojazdy drogowe.czytaj` | profilowany stan `OmsiMyOmsiList` | przypięty kontener OmsiHook | PASS: liczba/AI/indeks gracza | RUNTIME_PASS |
| Szczegółowa telemetria pojazdów drogowych | `pojazdy-drogowe.lista`, `pojazd-drogowy.czytany`, `pojazd-gracza.czytany` | profilowane migawki `MemArrayList` i `OmsiRoadVehicleInst` | układy nadrzędne uzgodnione z `Omsi23004_692EBFBF` | PASS: nieprzezroczysty uchwyt, pozycja, obrót, sterowanie, oświetlenie i pola AI; żądanie braku pojazdu zwraca `present=false` | RUNTIME_PASS |
| Odczyt zmiennej skryptu pojazdu | `lista.zmiennych pojazdu`, `zmienna.pojazdu.get` | profilowana tabela nazw ANSI i tablica wskaźników wartości publicznych | przypięty łańcuch `OmsiComplMapObjInst.GetVariable`; profil używa wartości instancji `+0x23C` | PASS: wyliczono 1025 nazwisk; `Odśwież_Stringi=0` | RUNTIME_PASS |
| Mutacja zmiennej skryptu pojazdu | `zestaw.zmiennych.pojazdu` | profilowana tabela nazw ANSI i tablica wskaźników wartości publicznych | przypięty łańcuch `OmsiComplMapObjInst.SetVariable`; skończone dane wejściowe typu float, weryfikacja nieprzezroczystego uchwytu na żywo i natychmiastowy odczyt | PASS: `Refresh_Strings 0 -> 1 -> 0`; normalne sprzątanie PASS | RUNTIME_PASS |
| Odczyt zmiennej łańcuchowej pojazdu | `pojazd.lista-zmiennych.stringowych`, `vehicle.string-variable.get` | profilowana tabela nazw ANSI i tablica wartości Unicode | przypięty łańcuch `OmsiComplMapObjInst.GetStringVariable` | PASS: wyliczono 27 nazwisk; `ident=GRN-V 30` | RUNTIME_PASS |
| Stałe pojazdu | `lista.stałych.pojazdów`, `stała.pojazdów.get` | profilowany blok `ScriptConstants` | przypięte tablice stałych/nazw `OmsiConstBlock` | PASS: `AI_lights_blinkgeberintervall = 0,3`; normalne sprzątanie PASS | RUNTIME_PASS |
| Krzywe pojazdu | `lista.krzywych.pojazdów`, `krzywa.pojazdów.ocena` | profilowana funkcja `OmsiConstBlock` i tablice punktowe | przypięte zaciskanie krzywej/semantyka interpolacji liniowej | PASS: `AI_Wandler_last(0) = 1300`; normalne sprzątanie PASS | RUNTIME_PASS |
| Metadane HOF pojazdu | `pojazd.hofs.read` | tablica HOF profilowanej definicji pojazdu | przypięty `OmsiHOF` nazwa UTF-16 plus podróż serwisowa ANSI | PASS: 11 wpisów, w tym `Grundorf` i `Betriebsfahrt`; normalne sprzątanie PASS | RUNTIME_PASS |
| Sterowniki | `sterowniki.czytane` | profilowana tablica sterowników | przypięte pola `OmsiDriver` | PASS: jedna migawka `OMSI-Fan`; normalne sprzątanie PASS | RUNTIME_PASS |
| Pakiet biletów | `bilety.przeczytane` | profilowane zapisy pakietów/biletów | przypięte pola `OmsiTicketPack`/`OmsiTicket` | PASS: pięć rekordów biletów „Berlin_1”; normalne sprzątanie PASS | RUNTIME_PASS |
| Dzienniki rozkładu jazdy | `harmonogram.logs.read` | profilowana tablica logów dynamicznych | przypięty rekord `OmsiTimeTableLog` | PASS: ważna pusta kolekcja Grundorf; normalne sprzątanie PASS | RUNTIME_PASS |
| Umieść losowy autobus | `pojazdy-drogowe.miejsce-losowe` | profilowany mostek natywny | nadrzędny ABI `TProgMan.PlaceRandomBus` | PASS: surowy powrót `2`, RoadVehicles `2 -> 4`; normalne sprzątanie PASS | RUNTIME_PASS |
| Nazwane wyzwalacze pojazdu/obiektu | żaden | żaden | metody nadrzędne wymagają nierozwiązanego zarządzanego dowodu własności Delphi-string | nie biegać | BLOCKED_STRING_OWNERSHIP |
| Wyzwala dźwięk | żaden | żaden | metoda upstream wymaga nierozwiązanego zarządzanego dowodu własności Delphi-string | nie biegać | BLOCKED_STRING_OWNERSHIP |
| Liczba kolekcji ludzkich | `ludzie.czytaj` | Tablica wskaźników Delphi | przypięty OmsiHook global | PRZEJDŹ: 408 | RUNTIME_PASS |
| Szczegółowa telemetria człowieka | `humans.list`, `human.read` | profilowane migawki `OmsiHumanBeingInst` | układy nadrzędne uzgodnione z `Omsi23004_692EBFBF` | PASS: nieprzezroczysty uchwyt, ruch, cel, bilet, miejsce, stacja i pola AI | RUNTIME_PASS |
| Menedżer rozkładu jazdy liczy | `rozkład jazdy.czytaj` | profilowane tablice | przypięty `OmsiTimeTableMan` | PASS: tory/wycieczki/przystanki/linie | RUNTIME_PASS |
| Rozkład jazdy Tory / Wycieczki / Linie | `rozkład.ścieżek.lista`, `rozkład.wycieczek.lista`, `rozkład.linii.lista` | profilowane rekordy `OmsiTT*Internal` o stałej szerokości | przypięte rekordy rozkładu jazdy uzgodnione z `Omsi23004_692EBFBF` | KARNET: 3 tory, 3 wycieczki i 2 linie; `76_BH-Kk` / `76` tożsamości | RUNTIME_PASS |
| Rozkład jazdy przystanków autobusowych / stacji Linki | `rozkład jazdy.przystanków.lista`, `rozkład jazdy.stacja-linków.lista` | profilowane rekordy „OmsiTTBusstopListEntryInternal” i „OmsiTTStnLinkInternal” o stałej szerokości | przypięte rekordy rozkładu jazdy uzgodnione z `Omsi23004_692EBFBF` | KARNET: 13 przystanków autobusowych, w tym „Nordspitze”; 14 linków do stacji | RUNTIME_PASS |
| Rozkład jazdy | `rozkład.wycieczek.lista` | profilowane zagnieżdżone tablice `OmsiTTTourInternal` w obszarze Linie | przypięte rekordy rozkładu jazdy uzgodnione z `Omsi23004_692EBFBF` | KARNET: 3 wycieczki; linia `0`, wycieczka `1`, grupa AI `Autobusy`, 72 wpisy na wycieczki | RUNTIME_PASS |
| Profile harmonogramu | `harmonogram.profile.list` | profilowane zagnieżdżone tablice `OmsiTTProfileInternal` w Trips | przypięte rekordy rozkładu jazdy uzgodnione z `Omsi23004_692EBFBF` | PRZEJŚCIE: 3 profile; `standard`, czas całkowity 420, 8 czasów zatrzymania | RUNTIME_PASS |
| Rozkład jazdyWpisy | `rozkład jazdy.lista wpisów-wycieczek` | profilowane zagnieżdżone tablice `OmsiTTTourEntryInternal` w obszarze Tours | przypięte rekordy rozkładu jazdy uzgodnione z `Omsi23004_692EBFBF` | KARTA: 130 wejść; `76_BH-Kk`, wyłączenie/profil 0, 14820 do 15240 | RUNTIME_PASS |
| Rozkład jazdy Wpisy | `rozkład jazdy.lista wpisów-ścieżek` | profilowane zagnieżdżone tablice `OmsiTTTrackEntryInternal` w obszarze Ścieżki | przypięty Tr o stałej szerokościRekord ackEntry uzgodniony z `Omsi23004_692EBFBF`; ograniczona do 512 wpisów | PASS: 91 wejść; pierwszy identyfikator `99`, kafelek `4`, odległość `0` w 93 ms | RUNTIME_PASS |
| Rozkład jazdy RVFiles | `harmonogram.rv-files.list` | profilowane rekordy `OmsiRVFileInternal` | przypięty układ daty/linii/listy/prawdopodobieństwa uzgodniony z `Omsi23004_692EBFBF` | PASS: poprawna pusta tablica na Grundorfie w 62 ms | RUNTIME_PASS |
| Rozkład jazdy NoRVNumbers | żaden | żaden | Konflikt dostępu do tablicy upstream `raw:true` z układem `+0x24` obserwowanym w tym profilu | FAIL: nieprawidłowy nagłówek tablicy dynamicznej, a następnie nieprawidłowy adres surowy; oczyszczanie kanoniczne PASS | NIEROZWIĄZANE |
| Mutacja zmiennej łańcuchowej pojazdu | nie ogłoszono | Oczekująca granica przypisania/refcount ciągu Delphi | metody natywne upstream | nie biegać | BLOCKED_STRING_OWNERSHIP |
| Stan urządzenia D3D | wpisano `D3DRuntimeApi` / `d3d.status` | szczelina profilowa, strzeżony QI i obserwator na poziomie spółdzielczym | przypięty DXHook plus weryfikacja dokładnego gniazda | PASS: `S_OK`, GOTOWY, jedna posiadana referencja, zainstalowany hak resetujący | RUNTIME_PASS |
| Utwórz/opisz teksturę D3D | wpisano nieprzezroczyste `D3DTextureHandle` | ograniczony rejestr natywny, dynamiczna tekstura puli domyślnej | przypięta semantyka tekstur uzgodniona z bieżącym | PASS: dwa formaty, metadane poziomu, wiele uchwytów | RUNTIME_PASS |
| Aktualizacja tekstur D3D | wpisano `D3DTextureUpdate` | `GetLevelDesc`, granice, `LockRect`, kopia uwzględniająca wysokość tonu, `UnlockRect` | przypięte zachowanie DXHook bez wad własnościowych | PASS: aktualizacje pełne i prostokątne zwróciły `S_OK` | RUNTIME_PASS |
| Wydanie tekstur D3D | wpisana operacja wydania | wycofanie atomowe i wydanie dokładnie raz COM | wykres referencyjny i model generowania | PASS: zwolnienie, odrzucenie ponownego wydania i trzy cykle ponownego wykorzystania | RUNTIME_PASS |
| Zdarzenia cyklu życia D3D | publiczne `RuntimeEvents` (`d3d.ready/lost/resetowanie/przywrócenie`) | Zresetuj obserwatora vtable plus uporządkowaną stałą kolejkę przejściową | Aktualna kompilacja Gniazdo resetowania ABI i umowa resetowania D3D9 | `d3d.ready` PASJA; utrata/reset/przywrócenie nie zostało bezpiecznie wywołane | CZĘŚCIOWO_WSPIERANE |
| Odrzucenie przestarzałego uchwytu D3D | nieprzezroczysty uchwyt z tagiem sesji oraz generowanie urządzeń | kontrola generacji natywnej i stanu zasobów | kanał sesji o stałej szerokości i zasady resetowania puli domyślnej | PASS dla uchwytów zwolnionych i poprzedzających sesję; zresetuj ścieżkę unieważnienia, która nie została wykorzystana w czasie wykonywania | CZĘŚCIOWO_WSPIERANE |

## Stan fali D

OmsiLaunch jest właścicielem implementacji D3D9. Odczytuje urządzenie specyficzne dla profilu
slot, zachowuje dokładnie jedno odniesienie `QueryInterface<IDirect3DDevice9>`, działa
polecenia za pośrednictwem istniejącego licznika czasu OMSI UI/main/właściciela renderowania i ujawnia nie
Wskaźnik COM. Tekstury puli domyślnej są unieważniane przed zaobserwowaniem Resetu i
starsze pokolenia nigdy nie retargetują zasobu zastępczego. Normalna praca z teksturami,
StopSession, wymuszone wyjście, ponowne uruchomienie i odrzucenie nieaktualnej sesji to środowisko wykonawcze
udowodnione. Utrata urządzenia, pomyślny reset, wielokrotna utrata/reset i żądanie kontra reset
wyścigi są nadal wdrażane, ale nie są sprawdzone w czasie wykonywania, ponieważ nie są bezpieczne i zgodne z prawdą
producent utraty/resetu był dostępny; nie wywołano żadnego syntetycznego resetu.

Niezapowiedziane operacje są odrzucane; odkrycie możliwości nie może oznaczać, że a
surowy adres można bezpiecznie napisać lub zadzwonić.

