> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Chargement mondial

Statut : NORMATIF

`NEW_MAP` est le chemin canonique validé lors de l'exécution. Carte et identité du point d'entrée
sont des valeurs sémantiques ; un index de point d'entrée présenté est un diagnostic/de bas niveau
remplacement, pas une identité persistante.

Le transfert de démarrage v4 peut transporter un sélecteur de point d'entrée et une situation canonique enregistrée
identité à la limite de démarrage native. Le sélecteur de point d'entrée atteint le natif
Limite de présentation `Tform_setpos`. L'adaptateur peut correspondre exactement à un unique
Étiquette de liste présentée Unicode, puis délègue la sélection à l'existant
Flux `Button1Click`. Il ne revient jamais silencieusement à un index. Le public
l'identité canonique reste dépendante des capacités : les étiquettes brutes `global.cfg` sont
pas nécessairement les étiquettes présentées et peuvent être dupliquées, donc leur exacte
une corrélation d'exécution est requise avant qu'une identité puisse être lancée.

La découverte hors ligne expose chaque enregistrement « [points d'entrée] » comme
`<map>#entrypoint :<sha256-of-normalized-record>`. Ceci est un contenu stable
identité plutôt qu'un ordinal ou une étiquette, mais cela reste uniquement une découverte jusqu'à ce que
le profil ferme son mappage avec la liste native présentée. Le validé
Enregistrements de base de Grundorf `index présenté 1 -> index brut 0 -> Nordspitze
"Bauernhof" ; la représentation du texte présenté par la VCL n'est toujours pas résolue.
la corrélation nécessite une identité structurée avant la publication.

`SAVED_SITUATION` représente un chemin canonique `.osn` sélectionné et utilise le
branche native de situation du formulaire de démarrage. Le profil enregistre la limite de preuve :
`Tform_start+0x3E4` est le contrôle de la situation sélectionnée, `+0x3F8` est son ordre
collection native et la répartition est `Tform_start.LoadSelectedSituation`
(`0x0064307C`) pour `Omsi23004_692EBFBF`. Le pont natif actuel résout le problème
chemin canonique dans cette collection Unicode en direct mais n'invoque jamais ce répartiteur
directement : cela contourne l’État qui appartient à la forme. Il sélectionne la radio appartenant à l'OMSI
mode, définit le sélecteur profilé via son setter VMT natif, appelle le
gestionnaire de synchronisation de sélection, puis exécute `Button1Click`. C'est
`RUNTIME_VALIDATED` : la situation Berlin-Spandau installée
`situations\\Baustelle Falkenseer Ch..osn` a atteint `gameplay.entered` via
le chemin de la session publique. Son chargement natif a pris environ 54 secondes, donc le Current
le délai d'expiration de démarrage par défaut est de 180 secondes ; les appelants peuvent fournir un explicite plus serré
lié lorsque leur contenu est connu pour se charger plus rapidement.

Avant de préparer un artefact d'exécution, `PlanSession` résout la carte déclarée par
le `.osn` sélectionné par rapport à l'installation actuelle. Une carte manquante est rejetée
comme `OL_E_SITUATION_MAP_NOT_FOUND` ; OmsiLaunch n'autorise pas le chargeur natif
pour présenter une erreur modale de contenu manquant et bloquer le chemin de démarrage sans tête.

« LAST_MAP_STATE » désigne le comportement natif de restauration automatique de la dernière carte. C'est
pas le plus récent `.osn`, un horodatage du système de fichiers ou une heuristique d'ordre des répertoires.
`laststn.osn` et `laststn.osn.owt` ne sont des preuves que jusqu'à ce que la branche native soit
fermé. La fonctionnalité n'est donc pas disponible pour le profil actuel.

