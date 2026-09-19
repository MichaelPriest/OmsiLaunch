> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Tests et validation

Les tests de configuration de session nécessitent une restauration à l'identique des octets après succès,
échec, arrêt et récupération de journal obsolète. Les tests hors exécution couvrent sans perte
options, drapeaux négatifs, vecteurs, plages, blocs composés, clavier personnalisé
événements, préservation du contrôleur, intégrité du transfert, restauration des transactions et
le bail d’installation cross-thread.

`OmsiLaunch.Native.x86.vcxproj` est une version 145 appartenant à `Debug|Win32`/`Release|Win32`
projet et n'est pas construit par l'invocation gérée `.sln`. Toute exécution d'exécution ou
la construction du package qui modifie `NativeBoundary.cpp` doit construire ce projet
explicitement avant la mise en scène ; le manifeste de déploiement consomme
`artifacts\x86\<Configuration>\OmsiLaunch.Native.x86.dll`.

La validation d'exécution est consolidée par une matrice de capacités. Le protégé en premier
la ligne de base reste `NEW_MAP` Grundorf, indice de point d'entrée 1, Nordspitze Bauernhof,
huit secondes d'exécution, arrêt demandé et restauration exacte normale.

## Validation de la présentation de la version

`tools/Test-ReleaseIdentity.ps1` extrait `OmsiLaunch-current.zip` et audite
chaque OmsiLaunch PE distribué. Il vérifie les champs communs produit/version,
le nom de fichier interne/original « OmsiLaunch.exe » du contrôleur et son contenu intégré
icône. `nethost.dll` est intentionnellement exclu car il s'agit d'un fichier non modifié
Dépendance d'exécution Microsoft, pas un binaire OmsiLaunch.

`tools/Test-ReleasePresentation.ps1` valide le package Release extrait,
pas « artefacts/bin ». Il vérifie que son manifeste a la configuration `Release`,
contient la fermeture permanente `plugins/OmsiLaunch.*` et quatre splash packagés
actifs sous `.omsilaunch/assets/splash`, et n'a pas de débogage ou obsolète
Chemin `runtime/plugin` dans le manifeste. Sans
`-RunOmsi`, il valide les plans gérés par défaut, gérés sur mesure et `Unset`.
Avec `-RunOmsi -InstallPackage`, il installe uniquement le produit appartenant au package
racine et les fichiers `plugins/OmsiLaunch.*` dans la racine autorisée, exécute chaque cas
via la version CLI installée, vérifie la superposition temporaire de l'interface graphique pendant que le
la session est active, restauration de l'interface graphique exacte en octets, aucun processus OMSI restant, non
journal et des hachages de plugin tiers inchangés. Il écrit sa preuve sous
le répertoire d'installation `.omsilaunch/diagnostics`.

Les preuves Wave D D3D utilisent le même chemin public « StartSessionAsync ». Séance
Acquisition d'un appareil validé `3cbd7bb6-d73f-4853-adc8-1213200527d2`, un en propriété
Référence QI, installation Reset-hook, créer/décrire, complet et rectangulaire
mises à jour, ressources multiples, rejet déterministe des versions répétées et
trois cycles de création/publication sur le thread OMSI `16676`. Séance
`fe1e10fd-cc79-484c-aa04-41fc40fef8da` couvrait la sortie forcée du processus avec un live
textures ; `a95d9fb4-66c6-4f7a-8a64-a18d8e3f9225` s'est avéré obsolète lors de la session précédente
gérer le rejet après la relance. Chaque exécution s'est terminée sans processus OMSI, sans journal,
bail; `plugins\OmsiLaunch.*` reste l'installation permanente du produit.
Les tests de perte/réinitialisation de l'appareil sont bloqués
jusqu'à ce qu'un producteur de cycle de vie natif sûr soit disponible ; appeler directement Reset
du harnais de validation n'est pas un substitut acceptable.

