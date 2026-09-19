> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Pierwsza sesja

W katalogu głównym OMSI przejrzyj przykładową wersję przenośną przed uruchomieniem OMSI:

```powłoka mocy
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
```

`RootPath: "."` w spakowanym przykładzie oznacza katalog zawierający
`OmsiLaunch.exe`. Rozpocznij tę samą sesję, pomijając `/plan`.

Zarządzana prezentacja powitalna jest domyślna. OmsiLaunch tymczasowo używa swojego
Zasób powitalny PTB, ENG, DEU lub FRA i przywraca oryginalne pliki OMSI
normalne zatrzymanie, nieudane uruchomienie lub trwałe odzyskiwanie. `/splash:Native` i
`/splash:Unset` zachowuje oryginalny wygląd OMSI.

