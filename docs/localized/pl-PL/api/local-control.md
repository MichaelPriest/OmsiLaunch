> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Protokół sterowania lokalnego

`OmsiLaunch.exe /serve` jest właścicielem jednej zarządzanej sesji i udostępnia sesję tylko dla bieżącego użytkownika
Punkt końcowy kontroli nazwanego potoku. Dodatkowe wywołania `OmsiLaunch.exe` działają jak
klienci; nigdy nie tworzą konkurencyjnego hosta dla tej sesji.

Wersja protokołu: `0.1`.

Obsługiwane trasy klienckie:

- `status sesji --json`
- `zatrzymanie sesji --json`
- `zdarzenia czytane --json`
- `obserwacja zdarzeń --json` (aż do `Ctrl+C`)
- Aliasy środowiska wykonawczego, takie jak `time get --json` i `time set --hour=18 --minuta=10 --json`

Każde żądanie i odpowiedź ma przedrostek JSON o długości i jest ograniczony do 64 KiB. The
punkt końcowy jest tylko lokalny i używa `PipeOptions.CurrentUserOnly`. Jest oddzielny
ze skrzynki pocztowej hosta do wtyczki, która pozostaje powiązana z sesją/procesem i tak nie jest
publiczna wykonywalna powierzchnia IPC.

Gdy nie istnieje żaden aktywny host, klienci zwracają `OL_E_NO_ACTIVE_SESSION` i wychodzą
z kodem „4”. Żądanie klienta nigdy nie uruchamia OMSI w sposób niejawny.

Każdy wynik to koperta zawierająca „ok”, „polecenie”, „wersję_protokołu” i
albo „wynik”, albo ustrukturyzowany „błąd”. Początkowa stabilna umowa dotycząca kodu wyjścia
to: `0` sukces, `2` nieprawidłowe argumenty, `3` nieobsługiwany profil, `4` nie
sesja aktywna, `5` niedostępny czas wykonania, `6` nie odnaleziono, `7` operacja
odrzucony, `8` błąd odzyskiwania i `10` błąd wewnętrzny. Kategorie błędów
są semantyczne („nieprawidłowy_argument”, „nieobsługiwany_profil”, „sesja”,
`runtime`, `not_found`, `transakcja` lub `wewnętrzne`); osoby wywołujące nie mogą analizować
wiadomości czytelne dla człowieka.

Polecenia środowiska wykonawczego otrzymują unikalne identyfikatory żądań w ramach sesji będącej właścicielem. The
potok publiczny nie przesyła żadnych wskaźników OMSI i nie udostępnia wtyczki hosta
nazwa skrzynki pocztowej. Wszystkie uchwyty zwrócone przez polecenie środowiska wykonawczego pozostają w zakresie sesji
i stają się nieaktualne, gdy właściciel zatrzyma lub zamknie sesję.

Nieznane nazwy operacji środowiska wykonawczego są odrzucane lokalnie jako
`OL_E_RUNTIME_OPERATION_UNKNOWN`; CLI nie tworzy sesji ani nie przesyła dalej
nierozpoznana operacja natywna.

Aktualnie aktywny rejestr poleceń publicznych dostępny jest poprzez:

```powłoka mocy
Możliwości programu OmsiLaunch.exe --json
Czas pomocy programu OmsiLaunch.exe --json
```

