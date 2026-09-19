> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Ładowanie świata

Status: NORMATYWNY

„NEW_MAP” to ścieżka kanoniczna sprawdzona w czasie wykonywania. Tożsamość mapy i punktu wejścia
są wartościami semantycznymi; przedstawiony wskaźnik punktu wejścia ma charakter diagnostyczny/niskiego poziomu
zastąpić, a nie trwałą tożsamość.

Przekazanie uruchomienia w wersji 4 może zawierać selektor punktu wejścia i kanoniczną zapisaną sytuację
tożsamość z natywną granicą uruchamiania. Selektor punktu wejścia dociera do natywnego
Granica prezentacji `Tform_setpos`. Adapter może pasować do jednego, unikalnego
Etykieta listy prezentowanej w formacie Unicode, a następnie deleguje wybór do istniejącej
Przepływ „Przycisk1Kliknięcie”. Nigdy nie wraca po cichu do indeksu. Publiczność
tożsamość kanoniczna pozostaje ograniczona możliwościami: surowe etykiety `global.cfg` są
niekoniecznie z prezentowanymi etykietami i można je powielać, więc są dokładne
zanim tożsamość stanie się możliwa do uruchomienia, wymagana jest korelacja w czasie wykonywania.

Wykrywanie w trybie offline udostępnia każdy rekord „[punkty wejścia]” jako
`<mapa>#entrypoint:<sha256-of-normalized-record>`. To jest stabilna treść
tożsamość, a nie liczbę porządkową lub etykietę, ale pozostaje ona wyłącznie do odkrycia do czasu
profil zamyka swoje mapowanie na natywną prezentowaną listę. Sprawdzone
Rekordy bazowe Grundorfa „przedstawiony indeks 1 -> surowy indeks 0 -> Nordspitze
Bauernhof”; reprezentacja tekstu prezentowanego w VCL jest nadal nierozwiązana.
korelacja wymaga ustrukturyzowanej tożsamości przed wydaniem.

`SAVED_SITUATION` reprezentuje wybraną ścieżkę kanoniczną `.osn` i używa
natywna gałąź sytuacji w formie początkowej. W profilu rejestrowana jest granica dowodu:
`Tform_start+0x3E4` to kontrola wybranej sytuacji, `+0x3F8` to jej uporządkowanie
kolekcja natywna, a wysyłka to „Tform_start.LoadSelectedSituation”.
(`0x0064307C`) dla `Omsi23004_692EBFBF`. Bieżący most natywny rozwiązuje problem
ścieżka kanoniczna w tej aktywnej kolekcji Unicode, ale nigdy nie wywołuje tego dyspozytora
bezpośrednio: w ten sposób omija się stan będący własnością. Wybiera radio należące do OMSI
mode, ustawia selektor profilowany poprzez natywny setter VMT, wywołuje metodę
procedurę obsługi synchronizacji wyboru, a następnie wykonuje `Button1Click`. To jest
`RUNTIME_VALIDATED`: zainstalowana sytuacja Berlin-Spandau
`sytuacje\\Baustelle Falkenseer Ch..osn` osiągnęły `gameplay.entered` poprzez
ścieżka sesji publicznej. Jego natywne ładowanie trwało około 54 sekund, więc Current
domyślny limit czasu uruchamiania wynosi 180 sekund; dzwoniący mogą podać bardziej szczegółowe informacje
powiązane, gdy wiadomo, że ich zawartość ładuje się szybciej.

Przed przygotowaniem jakiegokolwiek artefaktu wykonania, `PlanSession` rozwiązuje mapę zadeklarowaną przez
wybrany plik `.osn` względem bieżącej instalacji. Brakująca mapa zostaje odrzucona
jako `OL_E_SITUATION_MAP_NOT_FOUND`; OmsiLaunch nie pozwala na natywny moduł ładujący
aby przedstawić modalny błąd braku zawartości i ułożyć bezgłową ścieżkę startową.

„LAST_MAP_STATE” oznacza natywne zachowanie automatycznego przywracania ostatniej mapy. To jest
nie najnowszy plik `.osn`, znacznik czasu systemu plików lub heurystyka kolejności katalogów.
`laststn.osn` i `laststn.osn.owt` są dowodem tylko do momentu, gdy natywna gałąź zostanie
zamknięte. Dlatego ta funkcja jest niedostępna dla bieżącego profilu.

