> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Véhicule du joueur

Statut : MISE EN ŒUVRE EN COURS

`PlayerVehicleSpec` utilise le véhicule canonique `.bus`, repeint, HOF, numéro de flotte,
et les identités d'enregistrement. La découverte hors ligne résout ces identités
sans attribuer d'index de liste OMSI persistants.

Le profil actuel enregistre les primitives statiques nécessaires pour les versions natives ultérieures.
création (`TProgMan.MakeVehicle`, création/copie temporaire d'une liste de véhicules, aléatoire
placement des bus et positionnement des bus). Leur wrapper ABI-à-session n'est pas encore
implémenté, de sorte que chaque sous-capacité joueur-véhicule demandée reste fermée
indépendamment. Une situation sauvegardée ne doit pas être superposée à un nouveau joueur-véhicule
flux de création à moins que sa sémantique native ne l’exige explicitement.

La signature d'invocation OmsiHook épinglée constitue une preuve uniquement jusqu'à ce que chaque requête soit requise.
L'entrée global/list/critical-section est rapprochée avec `Omsi23004_692EBFBF`.
Le premier rapprochement ciblé a révélé une inadéquation concrète des profils : en amont
`MakeVehicle` lit son global `ProgMan` à `0x00862F28`, alors que ce profil
la sémantique globale `ProgMan` existante est `0x00858BDC`. Par conséquent pas d'amont
L'adresse est copiée dans le pont actuel. Les travaux de fermeture restants consistent à
identifier le propriétaire actuel/global exact utilisé par le chemin de création natif et
validez la durée de vie de la liste temporaire et le delta de la collection après la création.

