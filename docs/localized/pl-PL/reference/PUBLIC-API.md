> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Publiczne API

Status: NORMATYWNY

`OmsiLaunch.Api` to granica produktu. Konsumenci konstruują semantykę
`LaunchSpec`, wywołaj `PlanSessionAsync`, następnie wywołaj `StartSessionAsync` tylko dla
wykonalny plan. Interfejs CLI jest referencyjnym konsumentem tego interfejsu API i zawiera nie
osobna implementacja OMSI.

Kontrakty publiczne nigdy nie zawierają wskaźników OMSI, uchwytów Win32, typów DNNE, CLR
obiekty, szczegóły pamięci współdzielonej lub wartości zależne od szerokości wskaźnika. Treść
tożsamości są kanonicznymi ścieżkami względnymi OMSI. `GetCapabilitiesAsync` i a
`SessionPlan` ujawnia dostępność operacji specyficznych dla profilu kompilacji.

`StartSessionAsync` zwraca po przejęciu sesji, przemieszczeniu transakcyjnym i
ustanawia się nadzór nad procesem. Nie obiecuje rozgrywki. Konsumenci
poczekaj na `SessionState.Running`; w przypadku żądań uruchomienia bezgłowego oznacza to
host zaobserwował `gameplay.entered` ze środowiska wykonawczego w procesie.

`World.EntrypointIdentity` pozostaje ograniczony możliwościami do czasu uzyskania surowej struktury
mapowanie punktu wejścia na listę prezentowaną jest zamknięte dla aktywnego profilu. The
adapter natywny odrzuca już nieobecne lub niejednoznaczne prezentowane etykiety, ale obiekty wywołujące
w międzyczasie użyj `PresentedEntrypointIndex` jako obsługiwanego selektora niskiego poziomu.

`DiscoverAsync` jest tylko do odczytu. Wykrywanie dodatków dotyczy wyłącznie zasobów: OmsiLaunch
nie aktywuje, nie dezaktywuje, nie modyfikuje uprawnień ani nie zmienia dodatków Steam.

Cała konfiguracja w „LaunchSpec” to tymczasowy stan sesji. Publiczne API
nie udostępnia trwałej edycji konfiguracji: każdej dotkniętej konfiguracji
plik jest wykonywany na podstawie migawki transakcji, kronikowany, weryfikowany i odtwarzany bajt po bajcie.

## Prezentacja sesji

`SessionPresentationSpec.Splash` jest domyślnie `Zarządzany`. Wybiera
Domyślny ekran powitalny OmsiLaunch z `<installation>/.omsilaunch/assets/splash` i
tymczasowo nakłada pliki powitalne OMSI GUI. `CustomAssetDirectory` wybiera
katalog zasobów sesji/projektu; ścieżki względne są rozwiązywane poniżej instalacji.
Opcja „Unset” (i alias zgodności „Native”) jawnie zachowuje wersję natywną
Splash OMSI i nie dodaje mutacji powitalnej GUI. Niniejsza polityka prezentacji pozostaje niezmienna
ograniczony do sesji; `.omsilaunch` to stan produktu, a nie trwała edycja OMSI
konfiguracja.

## Zaawansowane API D3D9

`D3DRuntimeApi` zapewnia wpisane rozszerzenia sesji dla stanu urządzenia i tekstury
tworzyć, opisywać, aktualizować i wypuszczać. Modele publiczne to `D3DDeviceStatus`,
`D3DTextureHandle`, `D3DTextureDescription`, `D3DTextureUpdate`,
`D3DDeviceState`, `D3DTextureFormat` i `D3DTextureResourceState`.

Uchwyty są nieprzezroczystymi wartościami o zasięgu sesji. Konsumenci nie mogą uzyskać COM
wskaźnik, adres OMSI lub uchwyt Win32. Wydane, nieaktualne generowanie i
wartości między sesjami nie powiodą się w przypadku ustrukturyzowanych kodów `OmsiRuntimeException`. Publiczne
status sesji udostępnia również ograniczoną uporządkowaną kolekcję „RuntimeEvents”,
który przenosi zdarzenia cyklu życia D3D ze znacznikiem czasu, sekwencją i danymi semantycznymi.

Wpisane metody są opakowaniami tego samego publicznego kanału poleceń środowiska wykonawczego:

```csharp
var status = oczekiwanie na uruchomienie.GetD3DStatusAsync(sesja, TimeSpan.FromSeconds(5));
var tekstura = czekaj na uruchomienie.CreateD3DTextureAsync(
    sesja, 64, 64, D3DTextureFormat.A8R8G8B8);
oczekuj na uruchomienie.UpdateD3DTextureAsync(sesja, tekstura.Uchwyt,
    nowy D3DTextureUpdate(0, 0, 0, 64, 16, piksele));
oczekuj na uruchomienie.ReleaseD3DTextureAsync(sesja, tekstura.Uchwyt);
```

Wywołania są ważne tylko wtedy, gdy sesja będąca właścicielem to „RUNNING” i jest ona zgodna
BuildProfile jest aktywny. Resetowanie nie powoduje ponownego utworzenia ani ponownego skierowania dojścia.

