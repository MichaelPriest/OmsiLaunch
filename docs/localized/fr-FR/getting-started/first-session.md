# Première session

Depuis la racine OMSI, examinez l’exemple Release portable avant de démarrer :

```powershell
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
```

Dans cet exemple, `RootPath: "."` désigne le dossier qui contient
`OmsiLaunch.exe`. Retirez `/plan` pour démarrer. La splash gérée est le choix
par défaut ; `Native` et `Unset` préservent celle d’OMSI.
