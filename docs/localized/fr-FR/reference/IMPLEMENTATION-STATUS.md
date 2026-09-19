> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Statut de mise en œuvre

Statut : RUNTIME_NEW_MAP_PASS

| Composant | État | Remarques |
| --- | --- | --- |
| Abstraction de RuntimePlatform | STATICALLY_VALIDATED | Le fournisseur Windows x64 actuel est implémenté. |
| Fournisseur Windows x64 actuel | STATICALLY_VALIDATED | Valide la plate-forme avant les opérations d'exécution mutables. |
| Créer un profil | STATICALLY_VALIDATED | « Omsi23004_692EBFBF » est limité aux empreintes digitales. |
| Natif.x86 | RUNTIME_VALIDATED | version v145 Win32 ; protections de profil/octet d'origine exercées sur le chemin canonique. |
| Fondation d'interopérabilité dérivée d'OmsiHook | STATICALLY_VALIDATED | Chaînes Delphi x86, tableaux dynamiques, instantanés d'objets et limite explicite d'allocation à distance ; pas de protocole RPC en amont. |
| Interopérabilité des plugins en cours | STATICALLY_VALIDATED | `PluginRuntime` utilise un fournisseur de mémoire in-process à l'échelle du profil ; l'attachement externe n'est pas le chemin de session normal. |
| Canal de commande d'exécution | STATICALLY_VALIDATED | Boîte aux lettres de demande-réponse à largeur fixe/SHA-256 liée à la session, distribuée par le minuteur de thread d'interface utilisateur OMSI. |
| Lecteurs de cartes/météo/caméras d'exécution | RUNTIME_VALIDATED | Heure validée par la session canonique, carte, lectures scalaires météorologiques profilées, météo réelle et caméra via le canal de plug-in lié à la session. |
| Graphique avancé de carte/tuile | BLOCKED_PROFILE_LAYOUT | L'étiquette `OmsiMap.Kacheln` en amont sur `Map+0x118` n'a pas été résolue en un en-tête de tableau dynamique Delphi valide au cours de trois sessions Current gardées. Le procès n'écrit jamais et toutes les sessions sont restaurées normalement. |
| Horloge d'exécution/écriture météo scalaire | RUNTIME_VALIDATED | `time.set` a appliqué le profil `SetTime` avec relecture ; `weather.set` a validé une liste blanche et une restauration scalaires basées sur un profil. |
| Caméra d'exécution FOV écriture | RUNTIME_VALIDATED | `camera.set` a modifié et restauré le FOV pendant la session canonique ; La famille de caméras reste protégée. |
| Résumé du temps d'exécution véhicule/humain/horaire | RUNTIME_VALIDATED | Le lot canonique de Grundorf a renvoyé l'état des véhicules routiers, le nombre d'humains et le nombre de tableaux de gestionnaires d'horaires sans exposer les pointeurs bruts. |
| Télémétrie des horaires détaillés d'exécution | RUNTIME_VALIDATED | Les instantanés immuables profilés renvoyaient les pistes, les trajets et les lignes avec leurs noms sémantiques, leurs chemins, leurs décomptes et leurs métadonnées d'affectation via le canal de session. |
| Arrêts et liaisons horaires d'exécution | RUNTIME_VALIDATED | Les instantanés immuables profilés ont renvoyé les noms des arrêts de bus, les identifiants, les liens et les métadonnées des points de terminaison des liaisons de station. « Nordspitze » a été lu en direct depuis le champ UTF-16 Delphi. |
| Horaires des visites | RUNTIME_VALIDATED | Les instantanés de tournée imbriqués profilés ont renvoyé l'affiliation de ligne, le groupe/type d'IA, les métadonnées de réservation de véhicule et le nombre de TourEntry sans exposer les pointeurs Delphi. |
| Profils d'horaires d'exécution | RUNTIME_VALIDATED | Les instantanés de profil imbriqués profilés ont renvoyé le nombre d'affiliation au voyage, le nom, la durée totale, l'heure d'arrêt et le temps d'entrée sur la piste. |
| Entrées de tournée du calendrier d'exécution | RUNTIME_VALIDATED | Les instantanés TourEntry imbriqués profilés ont renvoyé l'identité sémantique du voyage, les indices de voyage/profil, le timing et l'état de transition en douceur. |
| Entrées de suivi du calendrier d'exécution | RUNTIME_VALIDATED | Les instantanés Bounded TrackEntry ont renvoyé 91 enregistrements en direct avec l'affiliation de la piste, l'ID, la tuile/le chemin, les distances, la validité, l'ordre et les métadonnées chrono via le canal de session. |
| Calendrier d'exécution RVFiles | RUNTIME_VALIDATED | Le tableau RVFile profilé a renvoyé une collection vide valide sur Grundorf via le canal de session. |
| Calendrier d'exécution NoRVNumbers | NON RÉSOLU | Le décodage `raw:true` en amont est en conflit avec la disposition du champ `Omsi23004_692EBFBF` observée. Retiré du lot automatique ; cela n'affecte pas les autres télémétries d'horaires. |
| Télémétrie détaillée véhicule/humain d'exécution | RUNTIME_VALIDATED | Les poignées opaques à l'échelle de la session et les instantanés en lecture seule ont renvoyé un mouvement/commandes/éclairage/IA cohérent du véhicule et un état de cible/billet/siège/station/IA humain sur Grundorf. Aucune écriture d’entité spatiale ou arbitraire n’est activée. |
| Variable de script d'exécution lue | RUNTIME_VALIDATED | Tables de noms ANSI etLe tableau de pointeurs `PublicVars` a résolu 1 025 noms et `Refresh_Strings=0` via un handle de véhicule opaque. |
| Mutation de variable de script d'exécution | RUNTIME_VALIDATED | `vehicle.variable.set` résout un handle opaque en direct et l'emplacement `PublicVars` profilé par nom de chaîne ouverte, rejette les valeurs non finies et renvoie une relecture immédiate. `Refresh_Strings 0 -> 1 -> 0` réussi avec un nettoyage normal. |
| Lecture de variable de chaîne d'exécution | RUNTIME_VALIDATED | Les tables de noms ANSI et les valeurs de chaîne Unicode ont résolu 27 noms et « ident=GRN-V 30 » via une poignée de véhicule opaque. Les écritures de remplacement/refcount Delphi restent séparées. |
| Lecteurs de métadonnées d'exécution Wave A | RUNTIME_VALIDATED | Métadonnées HOF, chauffeurs, packs de billets et journaux d'horaires transmis dans la session `e33aae7e-450a-4c00-8c7a-5f550ac4263f` ; constantes et courbes transmises dans `46260d64-a8e9-4ee9-a907-00e46b07a1ae`, le tout avec un nettoyage normal. |
| Installation permanente du plugin | STATICALLY_VALIDATED | Le Release ZIP place la fermeture du plugin OmsiLaunch directement dans `plugins\\`. Une session valide les hachages exacts du produit avant le lancement ; il ne met jamais en scène, ne prend pas d'instantanés, ne restaure, ne supprime ou ne revendique jamais de plugins tiers. |
| PlaceRandomBus | RUNTIME_VALIDATED | La session `4e2321d4-4c09-428b-8760-d348205b7392` a renvoyé `2`, a augmenté RoadVehicles de 2 à 4, puis a effectué un nettoyage exact. Il est distinct de make-basic et ne fait aucune réclamation PlayerVehicle. |
| Déclencheurs nommés et mutation de chaîne | BLOCKED_STRING_OWNERSHIP | L'expédition de la production est intentionnellement retardée jusqu'à ce que l'allocation, l'affectation et la durée de vie des chaînes Delphi puissent être clôturées sans hériter du risque de fuite/corruption en amont. |
| Catalogue d'opérations de domaine OmsiHook | STATICALLY_VALIDATED | Les domaines du programme, de la carte, de l'heure, de la météo, des véhicules, des humains, du calendrier, de la caméra, du lecteur et du son sont sémantiques/profils. |
| Appareil et textures Wave D D3D | RUNTIME_VALIDATED | Acquisition d'appareil à profil exact, référence QI unique, descripteurs de session opaques, création/dscription/mise à jour/version complète et correcte, rejet de version répété, ressources multiples, sortie forcée et nettoyage de relance réussi. |
| Cycle de vie/réinitialisation du Wave D D3D | IMPLEMENTED_NOT_RUNTIME_VALIDATED | L'interception de réinitialisation invalide les ressources du pool par défaut avant l'appel natif, avance la génération et publie les transitions ordonnées du cycle de vie. L'induction sécurisée de perte/réinitialisation de l'appareil n'était pas disponible, de sorte que les scénarios de perte/réinitialisation/cycle répété/course restent bloqués plutôt que revendiqués. |
| Adaptateur DNNE | STATICALLY_VALIDATED | L'adaptateur actuel est séparé de PluginRuntime. |
| PluginExécution | RUNTIME_VALIDATED | La consommation de transfert, le bras sans tête synchrone et l'exécution générique de NEW_MAP ont atteint le gameplay. |
| Transfert portable | STATICALLY_VALIDATED | Le protocole UTF-8/SHA-256 v4 à largeur fixe transporte des identités de point d'entrée sémantique et de situation enregistrée ; Le décodage v3 reste pris en charge et les tests aller-retour/corruption réussissent. |
| Découverte de contenu | STATICALLY_VALIDATED | Découverte de carte/situation/véhicule/repeindre/HOF hors ligne ainsi que des enregistrements « [points d'entrée] » hachés opaques ; la corrélation de lancement du point d’entrée reste séparée. |
| Configuration | STATICALLY_VALIDATED | À l'échelle de la session uniquement ; Les tests sans perte, les indicateurs négatifs, les plages, les blocs composés et la préservation des vecteurs réussissent. |
| Répertoire d'installation `.omsilaunch` | RELEASE_VALIDATED | L’état du répertoire est créé paresseusement après l’acquisition du bail pour une présentation gérée. Les ressources persistantes vivent sous des « actifs » ; les diagnostics persistent ; le journal durable est supprimé après la restauration. Ce n'est pas un fichier et jamais une destination de déploiement de plugin. |
| Présentation splash gérée | RELEASE_RUNTIME_VALIDATED | « Géré » est la valeur par défaut. Les actifs PTB/ENG/DEU/FRA 640 x 480 24 bits emballés se trouvent sous « .omsilaunch/assets/splash » et superposent transactionnellement « GUI/NewSplashscreen_ENG.bmp » ainsi que les paramètres régionaux résolus. Les sessions de version `388cf5d2-7310-4301-9313-e62c4c08a346` (par défaut) et `794cae34-a2ac-4fcf-8981-f142ac14c308` (répertoire personnalisé) ont atteint le jeu et ont terminé le nettoyage normal. Session `Désactivée` `12a74b24-6c78-4863-8ffe-947651212eeb` a atteint le gameplay sans superposition d'interface graphique prévue. Tous les trois ont restauré l'état d'origine de l'interface graphique et supprimé le journal ; les plugins de produits permanents n'étaient pas des participants à la transaction. |
| Package de version | RELEASE_VALIDATED | `tools/New-ReleasePackage.ps1 -Configuration Release` produit `OmsiLaunch-current.zip` avec le manifeste SHA-256, la fermeture de l'exécution de la version, les ressources de démarrage, l'exemple `.omsilaunch` et aucun chemin de débogage dans son manifeste. |
| Transaction/récupération | RUNTIME_RECOVERY_VALIDATED | Un journal `HANDOFF_CREATED` obsolète a restauré uniquement les artefacts appartenant à la session et a supprimé son journal. La validation du propriétaire nécessite un processus non quitté, une heure de création et un chemin exécutable avant que la récupération ne soit refusée. |
| PlanifierSession | STATICALLY_VALIDATED | Le plan canonique Grundorf résout les artefacts d’exécution en lecture seule. |
| Démarrage sans tête | IMPLEMENTÉ / RUNTIME_VALIDATED | Le démarrage natif était armé de manière synchrone et le gameplay était l'état cible observé. |
| NEW_MAP | IMPLEMENTÉ / RUNTIME_VALIDATED | Grundorf, a présenté l'indice 1 et Nordspitze Bauernhof complétés grâce au gameplay. |
| NEW_MAP identité du point d'entrée | RUNTIME_PARTIAL | Les enregistrements bruts hors ligne ont des hachages stables. L'exécution confirme que l'index présenté par Grundorf `1` sélectionne l'index brut `0` (`Nordspitze Bauernhof`), mais le texte/cartographie présenté en natif reste non résolu. Les demandes d’identité publique restent bloquées. |
| SAVED_SITUATION | RUNTIME_VALIDATED | `situations\\Baustelle Falkenseer Ch..osn` a chargé Berlin-Spandau via la radio du formulaire de démarrage complet, le sélecteur, la synchronisation et la séquence `Button1Click`, a atteint le gameplay, est resté en cours d'exécution pendant 8 secondes, puis a terminé l'arrêt demandé et la restauration normale. PlanSession rejette toujours les fichiers `.osn` dont la carte déclarée est absente avant la préparation. |
| LAST_MAP_STATE | UNSUPPORTED_FOR_CURRENT_PROFILE | La branche native exacte de restauration de la dernière carte n’est pas fermée ; aucune solution de secours pour l'ordre des fichiers n'existe. |
| Date-heure explicite/système | STATICALLY_PARTIAL | Non demandé par la régression canonique. |
| Véhicule de joueur | STATICALLY_PARTIAL | Non demandé par la régression canonique. |
| Implémentation de l'ancien NT6 | FUTUR / N/A | Limite architecturale uniquement. |
| Implémentation de l'ancien XP | FUTUR / N/A | Limite architecturale uniquement. |

## Base de référence d'exécution canonique

`RUNTIME_NEW_MAP_PASS` : public `StartSessionAsync` a validé l'installation permanente du runtime et
transfert portable, atteint `gameplay.entered` pour `maps\Grundorf\global.cfg`
avec l'indice de point d'entrée présenté `1` (`Nordspitze Bauernhof`), est resté en cours d'exécution
pendant huit secondes, puis a effectué un arrêt demandé et une restauration exacte normale.

Le bail d'installation actuel utilise un sémaphore Windows nommé (nombre maximum
un), pas un mutex : le nettoyage peut avoir lieu sur un autre thread géré. Le durable
le journal et l'identité du processus restent l'autorité de reprise après incident.

Le délai de démarrage par défaut est de 180 secondes. Cela préserve un échec limité
chemin tout en permettant aux situations enregistrées nativement sur de grandes cartes de compléter leur
travail de chargement légitime avant le match ; les appelants peuvent définir un délai d’attente explicite plus court.

## Présentation de la version 001

Version de version, suites d'unités/intégrations, CLI packagée `PlanSession`, package
validation du manifeste et des ressources réussie. Les rapports exécutables packagés
produit `OmsiLaunch`, version du produit `0.1.0`, protocole `0.1` et utilise le
icône officielle multi-résolution. Un démarrage de version géré créé et correspondant au hachage
les quatre actifs de démarrage persistants dans `.omsilaunch/assets/splash`. La libération
Les sessions par défaut, personnalisées et « non définies » ont chacune atteint le gameplay, puis « terminées ».
Les superpositions par défaut/personnalisées et la préservation native ont été sélectionnées par leur public
LaunchSpec et validé par les plans émis. Tous les trois ont restauré l'original
État de l'interface graphique, suppression du journal et conservation du plugin de produit permanent
fermeture. Le
le comportement des transactions à destination absente est couvert séparément par
`transaction.absent-overlay-restore`.

