> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Catalogue de capacités OmsiLaunch Beta 0.1

Version du produit : `0.1.0`  
Protocole de contrôle : `0.1`  
Profil exécutable pris en charge : `Omsi23004_692EBFBF` (`Omsi.exe` SHA-256 `692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243`). Le profil Steam LAA SHA-256 est accepté mais reste en attente de validation sur le terrain de l'exécution bêta.

Significations du statut :

- **Supporté** : validé au moment de l'exécution sur le profil pris en charge et approprié pour l'API sémantique bêta.
- **Expérimental** : implémenté et validé là où indiqué, mais les détails spécifiques au profil ou la forme de l'API peuvent évoluer au cours de la version bêta.
- **Partiel** : un sous-ensemble utile est disponible ; les restrictions énumérées sont importantes.
- **Pas encore disponible** : intentionnellement absent du contrat public Beta.
- **Interne** : primitive d'implémentation/test, pas un engagement bêta public.

| Capacité | Statut | Stabilité de l'API | Validation d'exécution | Limite connue |
|---|---|---|---|---|
| Planification de session, lancement, statut, attente, arrêt, fermeture | Pris en charge | Bêta stable | Passer | Un seul profil OMSI exact. |
| Préparation et restauration du runtime/configuration transactionnel | Pris en charge | Bêta stable | Passer | Toutes les modifications de configuration de lancement sont limitées à la session. |
| Lancement de NEW_MAP par index de point d'entrée présenté | Pris en charge | Bêta stable | Passer | L’identité sémantique du point d’entrée reste partielle. |
| SAVED_SITUATION répartition native | Expérimental | Expérimental | Passer | Le contenu enregistré testé doit être résolu en carte/contenu installé. |
| LAST_MAP_STATE | Pas encore disponible | N/A | Ne pas exécuter | Jamais déduit du plus récent « .osn ». |
| Découverte de contenu et PlanSession | Pris en charge | Bêta stable | Pass hors ligne | La découverte dépend du profil/contenu. |
| Heure de lecture et heure native réglée | Pris en charge | Bêta stable | Passer | La mutation Calendar/`SetActualDateTime` n’est pas disponible. |
| Lecture météo et écriture scalaire sur liste blanche | Expérimental | Expérimental | Passer | Le cycle de vie de configuration/application réel/OACI n’est pas disponible. |
| Lecture du contrôleur météo réel | Expérimental | Expérimental | Passer | Aucune opération publique d’activation/rafraîchissement de l’OACI. |
| Lecture de l'état de la carte de base | Pris en charge | Bêta stable | Passer | Les champs avancés de métadonnées de carte/graphique de tuiles sont partiels. |
| Lecture de la caméra et écriture du FOV limité | Expérimental | Expérimental | Passer | Le cycle de vie de la caméra/la sémantique familiale restent fonction du profil. |
| Collection RoadVehicle et instantanés détaillés | Expérimental | Expérimental | Passer | Pas de relocalisation spatiale arbitraire ni de rereliure ODE. |
| Lecture/index du véhicule Player | Pris en charge | Bêta stable | Passer | Le véhicule du joueur nul est valide ; l’affectation déterministe sans tête n’est pas disponible. |
| Humains et instantanés d'horaires | Expérimental | Expérimental | Passer | Certaines mises en page avancées de la version actuelle restent partielles. |
| Variables numériques du script du véhicule | Expérimental | Expérimental | Passe lecture/écriture | L'écriture de variable de chaîne n'est pas disponible. |
| Variable de chaîne, constantes, courbes, HOF, pilotes, tickets, journaux lus | Expérimental | Expérimental | Passer | Les mutations de chaînes gérées et les déclencheurs nommés ne sont pas disponibles. |
| `véhicules-routiers.place-random` | Expérimental | Expérimental | Passer | Mutation du monde natal ; la valeur de retour est un diagnostic, pas une identité. |
| Primitive MakeVehicle de base | Interne | Interne uniquement | Passer | Ne crée/attribue pas PlayerVehicle. |
| Statut D3D et cycle de vie des textures | Expérimental | Expérimental | Passer | Le cycle de vie d’un appareil perdu/réinitialisé n’est que partiellement prouvé. |
| Événements d'exécution/session | Partielle | Expérimental | Partielle | Les événements D3D perdus/réinitialisés/restaurés manquent de véritable preuve d'exécution. |
| Superpositions de clavier/contrôleur/configuration | Pris en charge | Bêta stable | Pass hors ligne/intégration | Appliqué uniquement pour la session et restauré octet par octet. |

Tous les handles d’exécution sont opaques et limités à la session. Ce ne sont pas des pointeurs OMSI, ils ne peuvent pas être réutilisés après la fin de la session et peuvent être rejetés comme obsolètes ou libérés.

OmsiLaunch ne nécessite pas les binaires OmsiHook, son plugin RPC ou ses fichiers d'exécution. OmsiHook reste un oracle d'ingénierie avec une provenance LGPL documentée ; ce n'est pas une dépendance d'exécution.

