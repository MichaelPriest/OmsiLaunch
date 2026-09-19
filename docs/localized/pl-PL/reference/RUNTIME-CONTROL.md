> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Kontrola czasu działania

Status: NORMATYWNY

Kontrola środowiska wykonawczego OmsiLaunch jest ograniczona do sesji. To nie jest OmsiHook RPC
trybie zgodności i nie jest to architektura podłączania procesów zewnętrznych.

```
OmsiLaunch API/CLI -> sesja na żywo -> skrzynka pocztowa środowiska wykonawczego -> PluginRuntime
    -> Brama wątku OMSI UI -> profilowana Interop / Native.x86 -> OMSI
```

Przekazanie uruchamiania pozostaje tylko uruchomieniem. Polecenia środowiska wykonawczego używają osobnego pliku
wersjonowana skrzynka pocztowa powiązana z identyfikatorem GUID sesji i uruchomionym procesem OMSI.
Każde żądanie ma identyfikator żądania o stałej szerokości, ładunek UTF-8 i integralność SHA-256
sprawdzenie, przekroczenie limitu czasu i weryfikacja tożsamości odpowiedzi. Mapowanie jest tworzone wcześniej
`CreateProcessW` jest dostarczany wyłącznie przez środowisko podrzędne i jest
usunięte podczas PluginFinalize, zakończenia procesu, oczyszczenia sesji lub nieudanego uruchomienia.

„RuntimeCommand” to dane semantyczne: nazwa operacji plus klucz/wartość UTF-8
argumenty. Nie zawiera wskaźników OMSI, uchwytów Win32, obiektów DNNE ani CLR
serializacja obiektów. Wtyczka wysyła polecenia z istniejącego OMSI
licznik czasu wątku interfejsu użytkownika; Pracownicy IPC nigdy nie odwołują się bezpośrednio do metod Delphi/Borland.

Kontrola czasu wykonywania jest ograniczona do sesji. CLI to cienki interfejs referencyjny
`IOmsiLaunch.ExecuteRuntimeAsync`; nigdy nie łączy się niezależnie ani nie osadza
Logika specyficzna dla OMSI.

Gdy deklaratywne żądanie uruchomienia osiągnie wartość „RUNNING”, następuje wydanie pojedynczego polecenia semantycznego
może zostać wydany:

```tekst
OmsiLaunch.Cli <instalacja> /nowy /map:maps\Grundorf\global.cfg \
  /indeks punktu wejścia:1 /brak pojazdu /runtime:time.read
OmsiLaunch.Cli <instalacja> /nowy /map:maps\Grundorf\global.cfg \
  /indeks punktu wejścia:1 /brak pojazdu /runtime:time.set /runtime-arg:minuta=18
```

`/runtime-arg` powtarza się dla dodatkowych pól semantycznych. Każde polecenie przechodzi przez
powiązana z sesją skrzynka pocztowa żądań/odpowiedzi jest wykonywana przez `PluginRuntime`
licznik czasu interfejsu OMSI i jest odrzucany w przypadku powiązania sesji lub żądania tożsamości
nie pasuje. Komenda nie jest trwałym edytorem konfiguracji.

Sprawdzone polecenia dla `Omsi23004_692EBFBF` to `time.read`, `time.set`,
`map.read`, `pogoda.read`, `pogoda.set`, `pogoda.aktualna.odczyt`, `kamera.odczyt`,
`pojazdy-drogowe.czytane`, `lista-pojazdów drogowych`, `pojazdy-drogowe.czytane`,
`pojazd-gracz.odczyt`, `lista.zmiennych pojazdu`, `zmienna.pojazdu.get`,
`pojazd.string-variables.list`, `pojazd.string-variable.get`, `humans.read`,
`humans.list`, `human.read`, `harmonogram.read`, `harmonogram.tracks.list`,
`rozkład.wycieczek.lista`, `rozkład.linii.lista`, `rozkład.przystanków.lista.rozkładów jazdy`,
`rozkład jazdy.stacja-links.list`, `rozkład jazdy.tours.list` i
`harmonogram.profiles.list`, `harmonogram.tour-entries.list`, `d3d.status`,
`d3d.texture.create`, `d3d.texture.describe`, `d3d.texture.update` i
`d3d.texture.release`. Wszystkie operacje na harmonogramie są ograniczone i przeznaczone tylko do odczytu.
Adapter profilu celowo umieszcza na białej liście argumenty przemawiające za mutacjami.

`timetable.track-entries.list` to zweryfikowana, ograniczona, niezmienna migawka
tożsamość ścieżki semantycznej każdego wpisu ścieżki i metadane istotne dla czasu. The
kanoniczna partia Grundorfa zwróciła 91 rekordów za pośrednictwem kanału sesji.

`timetable.rv-files.list` to zweryfikowana ograniczona migawka. Grundorf jest aktywny
środowisko uruchomieniowe zwróciło prawidłową pustą kolekcję. `NoRVNumbers` celowo nie jest
opublikowano: przypiętą wcześniejszą interpretację kandydata „raw:true”.
pole koliduje z obserwowanym układem `Omsi23004_692EBFBF`, więc pozostaje
profil-nierozwiązany, zamiast ujawniać niebezpieczny odczyt.

„vehicle.variable.set” jest sprawdzany w czasie wykonywania przez ten sam kanał o zasięgu profilu.
Akceptuje nieprzezroczysty „uchwyt”, „nazwę” z otwartym ciągiem i skończoną „wartość zmiennoprzecinkową”,
rozwiązuje bieżący przedział zmiennej publicznej, a następnie zwraca natychmiastowy odczyt.

Zrzuty profilowanych obiektów używają nieprzezroczystych uchwytów zwracanych przez `road-vehicles.list`
i `humans.list`, następnie konsumowane przez `road-vehicle.read` i `human.read` z
`/runtime-arg:handle=<uchwyt>`. Dojście jest ważne tylko w sesji, do której jest właścicielem
i zostaje odrzucony, jeśli jego natywny obiekt nie jest już obecny w nurcie
kolekcja. To nie jest wskaźnik, Lisindeks t lub trwała tożsamość.

## Zaawansowana kontrola D3D9

Publiczna powierzchnia typu w `D3DRuntimeApi` otacza semantyczne operacje D3D.
`D3DTextureHandle` zawiera znacznik sesji i wewnętrzne generowanie urządzenia
żeton; nie jest to ani wskaźnik COM, ani ważny po zwolnieniu, zresetowaniu urządzenia
wymiana, zakończenie procesu lub uruchomienie kolejnej sesji.

Wtyczka pozyskuje kandydata o dokładnym profilu z `D3DDevice` i zatrzymuje jednego
QI i wywołuje wszystkie wywołania tekstur z istniejącego OMSI
Zegar interfejsu użytkownika/głównego/właściciela renderowania. Tekstury są ograniczone dynamicznie `D3DPOOL_DEFAULT`
zasoby. Aktualizacje sprawdzają poziom, prostokąt i ładunek, a następnie używają
`LockRect`/`UnlockRect` z kopiami wierszy uwzględniającymi wysokość tonu. Ładunek skrzynki pocztowej to
ograniczony do 48 KiB; przesyłane większe tekstury są wyrażane jako wielokrotny prostokąt
aktualizacje.

Obserwator cyklu życia publikuje `d3d.ready`, `d3d.lost`, `d3d.resetting` i
`d3d.restored` w `SessionStatus.RuntimeEvents`. Reset unieważnia wszystkie aktywne
default-pool Resources przed wywołaniem oryginalnej metody natywnej i postępów
generacji urządzenia. Ścieżki utraty urządzenia i resetowania są zaimplementowane, ale nie są
sprawdzone w czasie wykonywania, ponieważ Wave D nie miał bezpiecznego producenta dla rodzimych użytkowników
przejścia. `d3d.ready`, normalne operacje na teksturach, StopSession, wymuszone wyjście,
ponowne uruchomienie czyszczenia, zwolnione uchwyty i nieaktualne uchwyty między sesjami są sprawdzone.

