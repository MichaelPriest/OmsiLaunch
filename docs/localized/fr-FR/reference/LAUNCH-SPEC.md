> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Spécification de lancement

Statut : NORMATIF

`LaunchSpec` est une donnée sémantique portable. « UNSET » signifie préserver l'existant
paramètre OMSI et ne doit jamais être réduit à « faux », à zéro ou à un chiffre inventé.
par défaut. La représentation JSON est le même modèle utilisé par `/spec` ; explicite
Les commutateurs CLI remplacent ses valeurs de fichier.

Les groupes structurels pris en charge sont l'installation, le monde, l'identité du point d'entrée ou
index présenté, date, heure, année, météo, véhicule du joueur, configuration,
comportement d'entrée, de diagnostic et d'exécution. `NEW_MAP`, `SAVED_SITUATION` et
`LAST_MAP_STATE` sont des modes sémantiques. `LAST_MAP_STATE` signifie le natif d'OMSI
branche de restauration automatique de la dernière carte ; il n'est jamais déduit du fichier `.osn`
horodatages ou ordre du répertoire. Les indices de liste native numériques ne sont pas
identités de contenu persistantes.

Pour `NEW_MAP`, `World.EntrypointIdentity` est réservé à un futur plan structuré
identité canonique. Une étiquette brute `global.cfg` n'est pas suffisante car elle peut
être dupliqué et peut différer de l’étiquette native présentée. Jusqu'à ce que
le profil ferme cette corrélation, `PresentedEntrypointIndex` est le support pris en charge
sélecteur de diagnostic/niveau bas. Le transfert de démarrage v3 comporte un champ d'identité
sans valeurs de la taille d'un pointeur, mais l'utilisation publique reste limitée aux capacités.

La date et l'heure utilisent des enregistrements explicites « année/mois/jour » et « heure/minute/seconde ».
Le schéma n'inclut pas les versions du système d'exploitation, les handles Win32, les objets d'exécution CLR,
Détails DNNE, mappages de mémoire ou adresses natives.

PlanSession est le compilateur de ces données. Un plan n'est pas exécutable uniquement lorsqu'un
la capacité requise demandée n’est pas disponible. Capacité facultative non demandée
les portes sont informatives.

