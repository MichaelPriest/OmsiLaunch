# Pierwsza sesja

W katalogu głównym OMSI sprawdź przenośny przykład Release przed uruchomieniem:

```powershell
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
```

`RootPath: "."` oznacza folder zawierający `OmsiLaunch.exe`. Usuń `/plan`, aby
uruchomić sesję. Zarządzany splash jest domyślny; `Native` i `Unset` zachowują
oryginalny splash OMSI.
