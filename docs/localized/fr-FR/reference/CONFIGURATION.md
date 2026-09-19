> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

#Configuration

Statut : NORMATIF

Tous les remplacements de configuration LaunchSpec sont limités à la session. Instantanés OmsiLaunch
chacun a touché `options.cfg`, `Inputs\keyboard.cfg` ou `Inputs\gamectrler.cfg`
fichier avant d'appliquer une superposition, l'enregistre dans le journal durable des transactions,
et restaure les octets d'origine et SHA-256 après la sortie normale, `StopSession`,
échec de démarrage ou récupération de journal périmé. Il n'existe pas d'API d'édition permanente.

`UNSET` est préservé : il ne produit aucune mutation. Jetons inconnus, commande,
l'encodage, les nouvelles lignes, les queues vectorielles et les valeurs non liées survivent aux correctifs sans perte.

## Surface des options implémentées

`ConfigurationCatalog` prend actuellement en charge les superpositions de session éprouvées pour les applications générales,
vue, contrôles, collision, ticket, sauvegarde automatique, distance/complexité graphique,
réflexion du pochoir/pluie, trafic, son et les « systèmes de fumée » à quatre valeurs
bloquer. La sémantique publique négative est inversée à la limite du codec ; pour
l'exemple `simulation.collisionTerrain=false` écrit `no_collision_terrain`.

`advanced.reducedMultithreading` est un paramètre sémantique et synchronise les deux
indicateurs natifs de multithread réduit. `AIMaxCountRandom` corrige uniquement la route
trafic ou composante humaine et préserve les sept autres valeurs vectorielles.

`graphics.realTimeReflections` n'accepte actuellement que `economy` et `full`.
La représentation native de la sélection Désactivé de l'interface utilisateur reste volontairement
non inscriptible jusqu'à ce qu'une preuve statique le ferme. `graphics.texture` et
`graphics.textureFilter` sont connus mais non accessibles en écriture pour la même raison.

`graphics.particles` est une valeur composée :
`enabled, maxPerEmitter, playerVehicleOnly, inReflections`. Il traduit le
quatrième champ natif (`disableInReflections`) sans exposer ce nom négatif.

