> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Capacités d'exécution

Statut : INVENTAIRE DE MISE EN ŒUVRE

| Capacité | API/canal | Interopérabilité | Preuve statique | Preuve d'exécution | État de sortie |
| --- | --- | --- | --- | --- | --- |
| Demande/réponse de session | `ExecuteRuntimeAsync` | boîte aux lettres indépendante du profil | tests de protocoles et plugins | lot canonique PASS | RUNTIME_PASS |
| Temps de lecture | `time.read` | `OmsiTimeAdaptateur` | épinglé OmsiHook plus profil exact | PASS | RUNTIME_PASS |
| Lecture de la carte | `map.read` | `OmsiRuntimeReaders` | mise en page `OmsiMap` épinglée | PASSER; état chargé et tuiles renvoyées | RUNTIME_PASS |
| Graphique de carte/tuile | aucun | profil d'essai retenu en interne | en amont étiquete `OmsiMap+0x118` comme `Kacheln`, mais la mémoire active actuelle n'a pas exposé d'en-tête de tableau Delphi valide | L'essai `map.tiles.list` a échoué en toute sécurité dans les sessions `ec85b51a-f69a-4f05-8fb3-638c63f9f20d`, `129482a4-b3a2-448a-830e-4043d11de246` et « 90011081-f5e9-4b31-b907-941bb3a9fd1a » ; tout est nettoyé normalement | BLOCKED_PROFILE_LAYOUT |
| Lecture météo | `météo.lire` | `OmsiRuntimeReaders` | épinglé `OmsiWeather` et présentations d'enregistrement de météo active | PASS : scalaires de base plus température, point de rosée, pression, précipitations et taux | RUNTIME_PASS |
| Lecture météo réelle/OACI | `météo.actual.read` | `OmsiRuntimeReaders` | mise en page « OmsiActuWeather » épinglée | PASS | RUNTIME_PASS |
| Lecture de la caméra | `caméra.lire` | `OmsiRuntimeReaders` | emballage épinglé et preuves de la famille des caméras | PASS | RUNTIME_PASS |
| Mutation du champ de vision de la caméra | `caméra.set` | `OmsiCameraWriter` | disposition scalaire de la caméra épinglée | RÉUSSITE : `45 -> 46 -> 45` | RUNTIME_PASS |
| Mutation d'horloge | `heure.set` | `OmsiTimeAdapter` + `SetTime` profilé | champs d'horloge en amont et appel natif | PASS : minute `17 -> 18 -> 17` | RUNTIME_PASS |
| Mutation météorologique scalaire | `météo.set` | Liste blanche `OmsiWeatherWriter` | champs scalaires en amont et dispositions de profil | PASS : vent `0 -> 1 -> 0` | RUNTIME_PASS |
| Point d'entrée sémantique présenté | fermé | correspondance de liste unique exacte `Tform_setpos` native | profilé `FormShow`, texte d'élément de liste et flux `Button1Click` | Partiel : exécution canonique prouvée `index présenté 1 -> index brut 0 -> Nordspitze Bauernhof` ; la représentation textuelle présentée reste non résolue et les étiquettes brutes sont dupliquées | RUNTIME_PARTIAL |
| Démarrage de la situation enregistrée | `WorldMode.SavedSituation` | Mode de formulaire de démarrage profilé/sélecteur/séquence Button1Click | `Tform_start+0x3E4/+0x3F8/+0x434/+0x438/+0x43C`, `SetChecked` natif, setter d'index d'élément VMT et `Button1Click` | PASS : `situations\\Baustelle Falkenseer Ch..osn` a chargé Berlin-Spandau, a atteint `gameplay.entered`, est resté en cours d'exécution pendant 8 s, puis a terminé l'arrêt demandé et la restauration normale | RUNTIME_PASS |
| Calendrier / mutation actuelle de l'OACI | aucun | Chaîne Delphi/limite ABI incomplète | références méthode/profil | ne fonctionne pas | RELEASE_IF_CLOSED |
| Etat de collecte des véhicules routiers | `véhicules-routiers.read` | état `OmsiMyOmsiList` profilé | conteneur OmsiHook épinglé | PASS : nombre/IA/index des joueurs | RUNTIME_PASS |
| Télémétrie détaillée des véhicules routiers | `road-vehicles.list`, `road-vehicle.read`, `player-vehicle.read` | instantanés profilés `MemArrayList` et `OmsiRoadVehicleInst` | configurations en amont rapprochées avec `Omsi23004_692EBFBF` | PASS : poignée opaque, position, rotation, commandes, éclairage et champs IA ; la demande d'absence de véhicule renvoie `present=false` | RUNTIME_PASS |
| Variable de script de véhicule lue | `vehicle.variables.list`, `vehicle.variable.get` | table de noms ANSI profilée et tableau de pointeurs de valeur publique | chaîne `OmsiComplMapObjInst.GetVariable` épinglée ; le profil utilise les valeurs d'instance `+0x23C` | PASS : 1 025 noms recensés ; `Refresh_Strings=0` | RUNTIME_PASS |
| Mutation de variable de script de véhicule | `véhicule.variable.set` | table de noms ANSI profilée et tableau de pointeurs de valeur publique | chaîne `OmsiComplMapObjInst.SetVariable` épinglée ; entrée flottante finie, validation de poignée opaque en direct et relecture immédiate | PASS : `Refresh_Strings 0 -> 1 -> 0` ; PASS nettoyage normal | RUNTIME_PASS |
| Lecture variable de chaîne de véhicule | `vehicle.string-variables.list`, `vehicle.string-variable.get` | table de noms ANSI profilée et tableau de valeurs Unicode | chaîne `OmsiComplMapObjInst.GetStringVariable` épinglée | PASS : 27 noms recensés ; `ident=GRN-V 30` | RUNTIME_PASS |
| Constantes du véhicule | `vehicle.constants.list`, `vehicle.constant.get` | bloc `ScriptConstants` profilé | tableaux de constantes/noms épinglés `OmsiConstBlock` | PASS : `AI_lights_blinkgeberintervall = 0,3` ; PASS nettoyage normal | RUNTIME_PASS |
| Courbes du véhicule | `vehicle.curves.list`, `vehicle.curve.evaluate` | fonction `OmsiConstBlock` profilée et tableaux de points | sémantique de serrage de courbe épinglée/interpolation linéaire | PASS : `AI_Wandler_last(0) = 1300` ; PASS nettoyage normal | RUNTIME_PASS |
| Métadonnées HOF du véhicule | `vehicle.hofs.read` | réseau HOF de définition de véhicule profilé | épinglé le nom UTF-16 « OmsiHOF » plus le déclenchement du service ANSI | PASS : 11 entrées, dont "Grundorf" et "Betriebsfahrt" ; PASS nettoyage normal | RUNTIME_PASS |
| Pilotes | `drivers.read` | tableau de pilotes profilé | champs `OmsiDriver` épinglés | PASS : un instantané `OMSI-Fan` ; PASS nettoyage normal | RUNTIME_PASS |
| Pack de billets | `tickets.lire` | pack de billets profilés/enregistrements de billets | champs `OmsiTicketPack`/`OmsiTicket` épinglés | PASS : cinq enregistrements de billets « Berlin_1 » ; PASS nettoyage normal | RUNTIME_PASS |
| Journaux d'horaires | `timetable.logs.read` | tableau de journaux dynamique profilé | enregistrement `OmsiTimeTableLog` épinglé | PASS : collection Grundorf vide valide ; PASS nettoyage normal | RUNTIME_PASS |
| Placer un bus aléatoire | `véhicules-routiers.place-random` | pont natif profilé | en amont `TProgMan.PlaceRandomBus` ABI | PASS : retour brut `2`, RoadVehicles `2 -> 4` ; PASS nettoyage normal | RUNTIME_PASS |
| Déclencheurs de véhicule/objet nommés | aucun | aucun | les méthodes en amont nécessitent une preuve de propriété de chaîne Delphi gérée non résolue | ne fonctionne pas | BLOCKED_STRING_OWNERSHIP |
| Déclencheurs sonores | aucun | aucun | méthode en amont nécessite une preuve de propriété de chaîne Delphi gérée non résolue | ne fonctionne pas | BLOCKED_STRING_OWNERSHIP |
| Nombre de collections humaines | `humains.read` | Tableau de pointeurs Delphi | épinglé OmsiHook mondial | PASS: 408 | RUNTIME_PASS |
| Télémétrie humaine détaillée | `humains.list`, `humain.read` | instantanés profilés `OmsiHumanBeingInst` | configurations en amont rapprochées avec `Omsi23004_692EBFBF` | PASS : champs poignée opaque, déplacement, cible, ticket, siège, gare et IA | RUNTIME_PASS |
| Le gestionnaire d'horaires compte | `timetable.read` | tableaux profilés | épinglé `OmsiTimeTableMan` | PASS : pistes/trajets/arrêts/lignes | RUNTIME_PASS |
| Horaires Pistes / Trajets / Lignes | `timetable.tracks.list`, `timetable.trips.list`, `timetable.lines.list` | enregistrements `OmsiTT*Internal` profilés à largeur fixe | enregistrements d'horaires épinglés rapprochés avec `Omsi23004_692EBFBF` | PASS : 3 pistes, 3 voyages et 2 lignes ; Identités `76_BH-Kk` / `76` | RUNTIME_PASS |
| Horaires Arrêts de bus / Liens vers les gares | `timetable.bus-stops.list`, `timetable.station-links.list` | enregistrements profilés à largeur fixe `OmsiTTBusstopListEntryInternal` et `OmsiTTStnLinkInternal` | enregistrements d'horaires épinglés rapprochés avec `Omsi23004_692EBFBF` | PASS : 13 arrêts de bus dont « Nordspitze » ; 14 liaisons de gares | RUNTIME_PASS |
| Horaires des visites | `horaire.tours.list` | tableaux `OmsiTTTourInternal` imbriqués profilés sous Lignes | enregistrements d'horaires épinglés rapprochés avec `Omsi23004_692EBFBF` | PASS : 3 visites ; ligne `0`, tour `1`, groupe AI `Buss`, 72 TourEntries | RUNTIME_PASS |
| Profils d'horaires | `horaire.profiles.list` | tableaux `OmsiTTProfileInternal` imbriqués profilés sous Trips | enregistrements d'horaires épinglés rapprochés avec `Omsi23004_692EBFBF` | PASS : 3 profils ; `standard`, temps total 420, 8 temps d'arrêt | RUNTIME_PASS |
| Horaires TourEntrées | `horaire.tour-entries.list` | tableaux `OmsiTTTourEntryInternal` imbriqués profilés sous Tours | enregistrements d'horaires épinglés rapprochés avec `Omsi23004_692EBFBF` | PASS : 130 entrées ; `76_BH-Kk`, déclenchement/profil 0, 14820 à 15240 | RUNTIME_PASS |
| Horaires TrackEntries | `timetable.track-entries.list` | tableaux `OmsiTTTrackEntryInternal` imbriqués profilés sous Tracks | Tr épinglé à largeur fixeEnregistrement ackEntry rapproché avec « Omsi23004_692EBFBF » ; limité à 512 entrées | PASS : 91 entrées ; premier ID `99`, tuile `4`, distance `0` en 93 ms | RUNTIME_PASS |
| Horaires RVFiles | `timetable.rv-files.list` | enregistrements `OmsiRVFileInternal` profilés | disposition date/ligne/liste/probabilité épinglée rapprochée avec `Omsi23004_692EBFBF` | PASS : tableau vide valide sur Grundorf en 62 ms | RUNTIME_PASS |
| Horaires NoRVNuméros | aucun | aucun | L'accès au tableau `raw:true` en amont est en conflit avec la disposition `+0x24` observée par ce profil | FAIL : en-tête de tableau dynamique non valide, puis adresse brute non valide ; nettoyage canonique PASS | NON RÉSOLU |
| Mutation variable de chaîne de véhicule | pas annoncé | Affectation de chaîne Delphi/limite de refcount en attente | méthodes natives en amont | ne fonctionne pas | BLOCKED_STRING_OWNERSHIP |
| État de l'appareil D3D | tapé `D3DRuntimeApi` / `d3d.status` | emplacement de profil, QI gardé et observateur de niveau coopératif | épinglé DXHook plus validation exacte de l'emplacement de construction | PASS : `S_OK`, READY, une référence possédée, un hook de réinitialisation installé | RUNTIME_PASS |
| Texture D3D créer/décrire | tapé opaque `D3DTextureHandle` | registre natif délimité, texture dynamique de pool par défaut | sémantique de texture épinglée réconciliée avec Current | PASS : deux formats, niveaux de métadonnées, plusieurs poignées | RUNTIME_PASS |
| Mise à jour des textures D3D | tapé `D3DTextureUpdate` | `GetLevelDesc`, limites, `LockRect`, copie sensible au pitch, `UnlockRect` | comportement épinglé de DXHook sans ses défauts de propriété | PASS : les mises à jour complètes et rectangulaires ont renvoyé `S_OK` | RUNTIME_PASS |
| Version de texture D3D | opération de libération dactylographiée | retraite atomique et publication COM exactement une fois | graphe de référence détenu et modèle de génération | PASS : libération, rejet de libération répétée et trois cycles de réutilisation | RUNTIME_PASS |
| Événements du cycle de vie D3D | public `RuntimeEvents` (`d3d.ready/lost/resetting/restored`) | Réinitialiser l'observateur de la table vtable et la file d'attente de transition fixe ordonnée | Contrat de réinitialisation de l'emplacement de réinitialisation de la version actuelle ABI et D3D9 | PASS `d3d.ready` ; perte/réinitialisation/restauration non induite en toute sécurité | PARTIELLEMENT_SUPPORTÉ |
| Rejet de la poignée périmée D3D | handle opaque marqué par session et génération de périphérique | génération native et contrôles de l'état des ressources | canal de session à largeur fixe et politique de réinitialisation du pool par défaut | PASS pour les descripteurs publiés et de session antérieure ; réinitialiser le chemin d'invalidation non exécuté pendant l'exécution | PARTIELLEMENT_SUPPORTÉ |

## État de la vague D

OmsiLaunch possède l'implémentation D3D9. Il lit le périphérique spécifique au profil
slot, conserve exactement une référence `QueryInterface<IDirect3DDevice9>`, s'exécute
commandes via le minuteur OMSI UI/main/render-owner existant, et n'expose aucun
Pointeur COM. Les textures du pool par défaut sont invalidées avant d'être observées. Réinitialisation et
les anciennes générations ne reciblent jamais une ressource de remplacement. Travail de texture normal,
StopSession, sortie forcée, relance et rejet de session obsolète sont des éléments d'exécution
prouvé. Perte de périphérique, réinitialisation réussie, perte/réinitialisation répétée et demande contre réinitialisation
les courses restent mises en œuvre mais n'ont pas fait leurs preuves car il n'y a pas de sécurité et de vérité
Le producteur de perte/réinitialisation était disponible ; aucune réinitialisation synthétique n’a été invoquée.

Les opérations inopinées sont rejetées ; la découverte de capacités ne doit pas impliquer qu'un
l'adresse brute peut être écrite ou appelée en toute sécurité.

