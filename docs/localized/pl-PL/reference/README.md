> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

<p wyrównanie="centrum">
  <img src="assets/branding/omsilaunch-logo.png" alt="OmsiLaunch" szerokość="620">
</p>

<p array="center"><strong>Kontrola sesji dla OMSI 2.</strong></p>
<p array="center">Open source · Programowalny · Kierowany przez społeczność</p>

---

#OmsiLaunch

**OmsiLaunch** to programowalne uruchamianie typu open source, zarządzanie sesjami,
i warstwa kontroli czasu wykonania dla OMSI 2. Zapewnia publiczny interfejs API, funkcjonalność
CLI, działająca wtyczka/środowisko wykonawcze OMSI i dokładna kompilacja `BuildProfile`
granica. Jest to infrastruktura dla programów uruchamiających, narzędzi, automatyzacji i społeczności
integracje zamiast graficznego programu uruchamiającego.

> **Określ sesję, a nie kliknięcia.**

## Wersja beta 0.1

Pierwsza publiczna wersja beta to **0.1.0-beta1**. Obsługuje dokładny profil OMSI
`Omsi23004_692EBFBF`, sprawdza wykonywalny odcisk palca przed uruchomieniem oraz
nie zapewnia kompatybilności z nieznanymi kompilacjami OMSI. Pakiet i szczegółowe
stan zgodności jest udokumentowany w [`docs/`](../README.md).

OmsiLaunch obsługuje semantyczne planowanie sesji za pomocą „LaunchSpec” i
`PlanSession`, uruchomienie poprzez `StartSession`, obserwowana kontrola czasu wykonania i
normalne czyszczenie `StopSession`/`CloseSession`. Obsługiwane możliwości środowiska wykonawczego to
wyraźnie skatalogowane; możliwości eksperymentalne i niedostępne nie są ukryte.

## Własność bezpiecznej sesji

Wszystkie zastąpienia konfiguracji uruchamiania mają zasięg sesji. migawki OmsiLaunch,
dzienniki, stosuje, weryfikuje i przywraca każdą tymczasową konfigurację lub GUI
plik, który się zmienia. Nie oferuje stałej edycji konfiguracji w tej wersji beta.

Pliki wtyczek produktów są instalowane na stałe w katalogu `plugins\\OmsiLaunch.*`.
Nie są one kopiowane i usuwane przy każdej sesji, podobnie jak wtyczki innych firm
nigdy nie są własnością transakcyjną. `.omsilaunch\\` jest prywatnym programem OmsiLaunch
katalog zasobów, diagnostyki, czasopism i przykładów użytkowników.

Zarządzana prezentacja powitalna jest domyślna. Produkt PTB, ENG, DEU i FRA
zasoby są tymczasowo nakładane i dokładnie przywracane. Natywny/„Nieustawiony” powitalny
tryb zachowuje pliki OMSI.

## Pobierz

Pobierz **`OmsiLaunch-0.1.0-beta1.zip`** ze strony wydania projektu i
wyodrębnij go bezpośrednio do obsługiwanego katalogu głównego OMSI. Pakiet zawiera
kontroler, wymagane zależności, trwałe zamknięcie wtyczki, powitanie
zasoby, przykład sesji wydawniczej i mały podręcznik użytkownika offline.

Zobacz [Instalacja](../README.md) i
[Pierwsza sesja](../README.md). Użyj
`OmsiLaunch.exe /version`, aby sprawdzić zainstalowany kontroler.

## Dla programistów

Preferowaną powierzchnią produktu jest semantyczny publiczny interfejs API. Interfejs CLI to:
referencyjny interfejs użytkownika za pośrednictwem tego samego interfejsu API; nie zawiera oddzielnej logiki OMSI.
Kontrola środowiska wykonawczego ma zasięg sesji, jest sprawdzana pod kątem profilu i wykorzystuje nieprzezroczystą semantykę
uchwyty, a nie publiczne wskaźniki natywne.

- [Publiczne API](../README.md)
- [Kontrola czasu pracy](../README.md)
- [Katalog możliwości](../README.md)
- [Znane ograniczenia](../README.md)
- [Buduj zasady profilu](../README.md)

## Społeczność i licencja

OmsiLaunch to projekt open source zorientowany na społeczność. Jest niezależny od
inne programy uruchamiające OMSI i mogą być używane przez kompatybilne narzędzia społecznościowe.

Oprogramowanie OmsiLaunch jest objęte licencją [tylko LGPL-3.0](../README.md). Zobacz
[powiadomienia osób trzecich](../README.md) dla źródła zarejestrowanego
pochodzenie i obowiązujące uwagi.

