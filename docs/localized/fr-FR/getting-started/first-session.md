> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Première séance

À partir de la racine OMSI, inspectez un exemple de version portable avant de démarrer OMSI :

```powershell
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
```

`RootPath : "."` dans l'exemple packagé se résout dans le répertoire contenant
`OmsiLaunch.exe`. Démarrez la même session en omettant `/plan`.

La présentation splash gérée est la présentation par défaut. OmsiLaunch utilise temporairement son
PTB, ENG, DEU ou FRA splash Asset et restaure les fichiers OMSI d'origine sur
arrêt normal, échec de démarrage ou récupération durable. `/splash:Natif` et
`/splash:Unset` préserve le splash OMSI d'origine.

