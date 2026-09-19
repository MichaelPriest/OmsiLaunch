> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Buduj profile

Status: NORMATYWNY

`OmsiBuildProfile` jest niezależny od `RuntimePlatform`. Jest właścicielem pliku wykonywalnego OMSI
odcisk palca, globale, formularze, pola, metody, strony wywołań i natywne zabezpieczenia bajtów.
Nigdy nie koduje zgodności generacji hosta z systemem Windows.

`Omsi23004_692EBFBF` akceptuje tylko te znane wartości LAA SHA-256:

- `692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243`
  („ALTERNATE_LAA”, zweryfikowane w czasie wykonywania);
- `7DAB063D1F62E73B3A2C7A6AC1921D7EDF5E5DB0FBC731481D117EEC8DE7D759`
  („STEAM_LAA”, uzgodnione statycznie; oczekiwanie na sprawdzenie pola środowiska wykonawczego wersji beta).

Żaden ciąg wersji, nazwa pliku wykonywalnego, rozmiar pliku ani ogólny znacznik LAA nie jest
kryterium akceptacji. Każdy inny skrót jest odrzucany przed stacjonowaniem natywnego środowiska wykonawczego.
Profil rejestruje „SetActualDateTime”, wysłanie zapisanej sytuacji i
prymitywne pojazdy zaobserwowane w dowodach statycznych/poprzednich. Zapisanie symbolu jest
nie jest to autoryzacja do wywołania tego: każde natywne opakowanie ABI wymaga własnego
ochrona linii papilarnych i sprawdzona umowa telefoniczna.

W profilu rejestrowane jest również aktualne miejsce kandydata D3D „0x008627D0” jako
`Urządzenie D3DD`. Sesja uruchomieniowa `3cbd7bb6-d73f-4853-adc8-1213200527d2` została udowodniona
że pożyczony kandydat na slot obsługuje `QueryInterface<IDirect3DDevice9>`,
`TestCooperativeLevel` zwraca `S_OK` i wszystkie operacje D3D są wykonywane na
Główny wątek interfejsu użytkownika/głównego/właściciela renderowania OMSI. OmsiLaunch zachowuje jedno odniesienie do QI;
samo pożyczone miejsce nigdy nie jest zwalniane. Ten wpis nie jest uogólniony
kolejny wykonywalny odcisk palca.

Pasujący statyczny obiekt wywołujący dla kandydata referencyjnego `TProgMan.MakeVehicle`
globalne pod adresami `0x00859DEC`, `0x008591DC` i `0x00858D28`. Ich dokładna lista/indeks
role pozostają nierozwiązane, dlatego celowo nie nazywa się ich BuildProfile
symbole. Nie są one wskaźnikami publicznymi i nie stanowią uzupełnienia
implementacja pojazdu-gracza.

