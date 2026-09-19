> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Contrôle d'exécution

Statut : NORMATIF

Le contrôle d'exécution OmsiLaunch est limité à la session. Ce n'est pas OmsiHook RPC
mode de compatibilité et il ne s’agit pas d’une architecture d’attachement de processus externe.

```
OmsiLaunch API/CLI -> session en direct -> boîte aux lettres d'exécution -> PluginRuntime
    -> Passerelle OMSI UI-thread -> Interop profilée / Native.x86 -> OMSI
```

Le transfert de démarrage reste réservé au démarrage. Les commandes d'exécution utilisent un
boîte aux lettres versionnée liée au GUID de session et à son processus OMSI lancé.
Chaque requête a un ID de requête de largeur fixe, une charge utile UTF-8 et une intégrité SHA-256.
vérification, délai d'attente et validation de l'identité de la réponse. Un mappage est créé avant
`CreateProcessW` est fourni uniquement via l'environnement enfant et est
éliminé lors de PluginFinalize, de la sortie du processus, du nettoyage de la session ou de l'échec du démarrage.

`RuntimeCommand` est une donnée sémantique : nom de l'opération plus clé/valeur UTF-8
arguments. Il ne contient aucun pointeur OMSI, handle Win32, objet DNNE ou CLR
sérialisation d'objets. Le plugin envoie des commandes depuis son OMSI existant
Minuterie de thread d'interface utilisateur ; Les travailleurs IPC n'appellent jamais directement les méthodes Delphi/Borland.

Le contrôle d’exécution s’étend à la session. La CLI est une interface de référence légère sur
`IOmsiLaunch.ExecuteRuntimeAsync` ; il ne s'attache jamais indépendamment ni ne s'intègre
Logique spécifique à OMSI.

Après qu'une requête de lancement déclarative atteigne « RUNNING », une seule commande sémantique
peut être délivré avec :

```texte
OmsiLaunch.Cli <installation> /new /map:maps\Grundorf\global.cfg \
  /entrypoint-index:1 /no-vehicle /runtime:time.read
OmsiLaunch.Cli <installation> /new /map:maps\Grundorf\global.cfg \
  /entrypoint-index:1 /no-vehicle /runtime:time.set /runtime-arg:minute=18
```

`/runtime-arg` se répète pour des champs sémantiques supplémentaires. Chaque commande voyage via
la boîte aux lettres de requête/réponse liée à la session, est exécutée par `PluginRuntime` sur
le minuteur de l'interface utilisateur OMSI et est rejeté lorsque la liaison de session ou la demande d'identité
ne correspond pas. La commande n'est pas un éditeur de configuration persistant.

Les commandes validées pour `Omsi23004_692EBFBF` sont `time.read`, `time.set`,
`map.read`, `weather.read`, `weather.set`, `weather.actual.read`, `camera.read`,
`road-vehicles.read`, `road-vehicles.list`, `road-vehicle.read`,
`player-vehicle.read`, `vehicle.variables.list`, `vehicle.variable.get`,
`vehicle.string-variables.list`, `vehicle.string-variable.get`, `humans.read`,
`humans.list`, `human.read`, `timetable.read`, `timetable.tracks.list`,
`timetable.trips.list`, `timetable.lines.list`, `timetable.bus-stops.list`,
`timetable.station-links.list`, `timetable.tours.list` et
`timetable.profiles.list`, `timetable.tour-entries.list`, `d3d.status`,
`d3d.texture.create`, `d3d.texture.describe`, `d3d.texture.update` et
`d3d.texture.release`. Toutes les opérations d'horaire sont limitées et en lecture seule.
Les arguments en faveur des mutations sont délibérément ajoutés à la liste blanche par l'adaptateur de profil.

`timetable.track-entries.list` est un instantané immuable validé et limité de
l'identité du chemin sémantique de chaque entrée de piste et les métadonnées pertinentes pour le timing. Le
Le lot canonique Grundorf a renvoyé 91 enregistrements via le canal de session.

`timetable.rv-files.list` est un instantané limité validé. Grundorf est actif
le runtime a renvoyé une collection vide valide. `NoRVNumbers` n'est pas intentionnellement
publié : l'interprétation `raw:true` épinglée en amont du candidat
Le champ est en conflit avec la disposition `Omsi23004_692EBFBF` observée, il reste donc
profil non résolu plutôt que d’exposer une lecture dangereuse.

`vehicle.variable.set` est validé au moment de l'exécution via le même canal de portée de profil.
Il accepte un « handle » opaque, un « nom » de chaîne ouverte et une « valeur » flottante finie,
résout l'emplacement de variable publique actuel, puis renvoie une relecture immédiate.

Les instantanés d'entités profilées utilisent des descripteurs opaques renvoyés par `road-vehicles.list`
et `humans.list`, puis consommé par `road-vehicle.read` et `human.read` avec
`/runtime-arg:handle=<handle>`. Un handle n’est valide que pour sa propre session
et est rejeté si son objet natif n'est plus présent dans le fichier actuel
collecte. Ce n'est pas un pointeur, List index, ou identité persistante.

## D3D9 Contrôle avancé

La surface typée publique dans `D3DRuntimeApi` enveloppe les opérations sémantiques D3D.
`D3DTextureHandle` contient une balise de session et une génération de périphérique interne
jeton ; ce n'est ni un pointeur COM ni valide après libération, réinitialisation, appareil
remplacement, sortie de processus ou autre lancement de session.

Le plugin acquiert le profil exact du candidat auprès de `D3DDevice`, en conserve un
Référence QI et répartit tous les appels de texture depuis l'OMSI existant
Minuterie UI/principale/propriétaire de rendu. Les textures sont dynamiques limitées `D3DPOOL_DEFAULT`
ressources. Les mises à jour valident le niveau, le rectangle et la charge utile, puis utilisent
`LockRect`/`UnlockRect` avec des copies de lignes tenant compte du pitch. La charge utile de la boîte aux lettres est
limité à 48 Ko ; les téléchargements de textures plus grands sont exprimés sous forme de rectangles multiples
mises à jour.

L'observateur du cycle de vie publie `d3d.ready`, `d3d.lost`, `d3d.resetting` et
`d3d.restored` dans `SessionStatus.RuntimeEvents`. La réinitialisation invalide tous les live
ressources de pool par défaut avant d'invoquer la méthode native d'origine et les avancées
la génération de l'appareil. Les chemins de perte de périphérique et de réinitialisation sont implémentés mais ne le sont pas
éprouvé à l'exécution car Wave D n'avait pas de producteur sûr pour ces natifs
transitions. `d3d.ready`, opérations de texture normales, StopSession, sortie forcée,
le nettoyage de relance, les handles libérés et les handles obsolètes entre sessions sont prouvés.

