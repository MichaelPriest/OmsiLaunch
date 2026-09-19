> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Konfiguracja

Status: NORMATYWNY

Wszystkie zastąpienia konfiguracji LaunchSpec mają zasięg sesji. Migawki OmsiLaunch
każde dotknięcie `options.cfg`, `Inputs\keyboard.cfg` lub `Inputs\gamectrler.cfg`
plik przed nałożeniem nakładki zapisuje go w trwałym dzienniku transakcji,
i przywraca oryginalne bajty i SHA-256 po normalnym wyjściu, `StopSession`,
awaria uruchamiania lub odzyskiwanie nieaktualnego dziennika. Nie ma interfejsu API umożliwiającego trwałą edycję.

`UNSET` to zachowanie: nie powoduje mutacji. Nieznane tokeny, zamówienie,
kodowanie, znaki nowej linii, ogony wektorów i niepowiązane wartości przetrwają poprawki bezstratne.

## Zaimplementowane opcje powierzchni

`ConfigurationCatalog` obsługuje obecnie sprawdzone nakładki sesji dla celów ogólnych,
widok, sterowanie, kolizja, bilet, autozapis, odległość/złożoność grafiki,
odbicie szablonu/deszczu, ruch uliczny, dźwięk i czterowartościowe „systemy dymne”.
blok. Publiczna semantyka negatywna jest odwrócona na granicy kodeka; dla
przykład `simulation.collisionTerrain=false` zapisuje `no_collision_terrain`.

`advanced.reducedMultithreading` to jedno ustawienie semantyczne, które synchronizuje oba
natywne flagi o zmniejszonej wielowątkowości. `AIMaxCountRandom` łata tylko drogę
ruch lub element ludzki i zachowuje pozostałe siedem wartości wektorowych.

`graphics.realTimeReflections` obecnie akceptuje tylko wartości `ekonomiczne` i `pełne`.
Natywna reprezentacja wyboru Wyłączone w interfejsie użytkownika pozostaje celowo
niezapisywalny, dopóki nie zamkną go statyczne dowody. `grafika.tekstura` i
Filtry `graphics.textureFilter` są znane, ale z tego samego powodu nie można ich zapisywać.

`graphics.particles` to wartość złożona:
`enabled, maxPerEmitter, playerVehicleOnly, inReflections`. Tłumaczy
natywne czwarte pole („disableInReflections”) bez ujawniania tej negatywnej nazwy.

