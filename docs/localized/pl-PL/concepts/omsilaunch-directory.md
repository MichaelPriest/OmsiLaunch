> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# `.omsilaunch` Katalog stanu instalacji

`.omsilaunch` to katalog znajdujący się poniżej katalogu głównego instalacji OMSI. Nigdy nie jest to A
LaunchSpec i nie jest wdrażany w „wtyczkach”.

Wersja ZIP jest wyodrębniana bezpośrednio do katalogu głównego OMSI. `OmsiLaunch.exe` i
jego zespoły kontrolerów pozostają w tym katalogu głównym, podczas gdy stała wtyczka OMSI
zamknięcie jest instalowane bezpośrednio w `plugins\\OmsiLaunch.*`. Te pliki należą
do instalacji produktu: sesje weryfikują swoje skróty, ale nigdy nie etapują,
migawki, przywracania, usuwania lub używania ich jako tymczasowego źródła wdrożenia.

## Tworzenie i własność

Katalog jest tworzony leniwie. Tworzy się zarządzana sesja powitalna
`<OMSI>/.omsilaunch/assets/splash` po uzyskaniu dzierżawy instalacji i
przed zastosowaniem transakcji. Kopiowane są brakujące zasoby OmsiLaunch
z spakowanego katalogu `.omsilaunch/assets/splash`. Istniejące aktywa nigdy nie są
nadpisane, co pozwala na zachowanie jawnie zarządzanego zestawu zasobów projektu.

Katalog jest własnością OmsiLaunch. Różni się od konfiguracji OMSI
oraz ze stałej instalacji wtyczki.

## Układ

| Ścieżka | Całe życie | Cel |
|---|---|---|
| `assets/splash/{PTB,ENG,DEU,FRA}.bmp` | Stały zasób OmsiLaunch | Domyślne zarządzane zasoby powitalne o rozdzielczości 640x480, 24-bitowe. Żaden numer wydania nie jest osadzony. |
| `diagnostyka/<sesja>-host.log` | Stała diagnostyka | Śledzenie przejścia semantycznego hosta. |
| `diagnostics/*-runtime-*.json` | Stała diagnostyka | Jawne polecenie/dowody wsadowe w czasie wykonywania. |
| `journal.json` | Tymczasowy trwały stan transakcji | Dokładna migawka, tożsamość procesu i postęp przywracania. Usunięto po normalnym przywróceniu; zachowane wyłącznie w celu odzyskania. |
| `*.omsilaunch.tmp` obok dotkniętego pliku OMSI | Efemeryczne | Inscenizacja zapisu atomowego; usunięte lub zastąpione atomowo. |

Tylko tymczasowe zmiany OMSI, takie jak `GUI/NewSplashscreen_*.bmp` i wymagane
nakładek konfiguracyjnych, wprowadź transakcję migawki/dziennika/przywrócenia.
Wtyczki innych firm nigdy nie są wymieniane pod kątem własności, kopiowane, usuwane lub
przywrócony.

Mapowania poleceń uruchamiania, telemetrii i środowiska wykonawczego są nazywane udostępnionymi
obiekty pamięci. Są one powiązane z sesją/procesem i **nie** są plikami poniżej
katalog.

## Wybór powitalny

`SessionPresentationSpec.Splash` ma trzy tryby semantyczne:

- `Zarządzane` (domyślnie): OmsiLaunch instaluje `ENG` oraz rozwiązany język
  target w `GUI/NewSplashscreen_*.bmp` poprzez istniejącą transakcję. The
  oryginalne pliki OMSI są przywracane bajt po bajcie po wyjściu, awarii lub
  powrót do zdrowia.
- `Native` lub `Unset`: zachowanie plików powitalnych OMSI; nie ma nakładki powitalnej GUI
  stworzony.

Rozdzielczość językowa akceptuje `PTB`, `ENG`, `DEU` i `FRA`; gdy jest nieobecny, używa
`options.cfg`, a następnie wracamy do `ENG`. `/splash-assets:<katalog>` lub
`Presentation.CustomAssetDirectory` wybiera jawny katalog niestandardowy. A
względny katalog niestandardowy jest rozpoznawany poniżej katalogu głównego instalacji OMSI. Musi
zawierać prawidłowy plik `ENG.bmp` oraz, w przypadku wybranego języka innego niż angielski, jego dopasowanie
zlokalizowane BMP. Zasoby muszą mieć rozmiar 640 x 480 i 24-bitowe pliki BMP.

Brak spakowanych/domyślnych zasobów, nieprawidłowy format zasobu lub nieprawidłowy zwyczaj
Niepowodzenie planowania/uruchamiania katalogu z powodu semantycznego błędu powitalnego; OMSI nie
uruchomiony. Sam brakujący katalog `.omsilaunch` jest zjawiskiem normalnym i jest tworzony jako
opisane powyżej.

## Walidacja wydania

`tools/Test-ReleasePresentation.ps1` sprawdza spakowany plik wykonywalny Release
i może przeprowadzić trzy przypadki prezentacji przeciwko autoryzowanemu OMSI
instalacja. Jego domyślny przebieg jest niezmutowany i sprawdza zasoby pakietu plus
trzy wyniki „PlanSession”. `-RunOmsi` dodatkowo dowodzi tymczasowego działania
nakładka podczas działania OMSI, dokładne przywrócenie GUI i czyszczenie dziennika/procesu.
Za pomocą `-RunOmsi -InstallPackage` kopiuje tylko produkty należące do pakietu
następnie pliki root i pliki `plugins/OmsiLaunch.*` do autoryzowanego testowego katalogu głównego
sprawdza również, czy skróty wtyczek innych firm pozostają niezmienione. Rezultatem jest
utrzymywał się jako `diagnostyka/weryfikacja-prezentacji-wydania-weryfikacji.json`.

## Pliki konfiguracyjne

LaunchSpec JSON jest dostarczany poprzez `/spec:<file.json>`. Przykładem dystrybucji jest
`przykłady/.omsilaunch/canonical-session.json`; zagnieżdżony katalog jest próbką
tylko układ. Można go skopiować do katalogu instalacyjnego `.omsilaunch`,
ale plik konfiguracyjny nie jest wymagany do działania katalogu.

