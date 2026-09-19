> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# OmsiLaunch Beta 0.1 Limitations connues

## Compatibilité

La version bêta 0.1 prend uniquement en charge « Omsi23004_692EBFBF », identifié par le SHA-256 exact « Omsi.exe » documenté dans le catalogue de fonctionnalités. Les exécutables inconnus ou non profilés sont rejetés ; La version bêta 0.1 ne revendique pas la prise en charge générique d'OMSI 2.

## Chargement mondial et véhicules

- `LAST_MAP_STATE` n'est pas implémenté. OmsiLaunch ne remplace jamais la situation enregistrée la plus récente.
- L'index du point d'entrée présenté est pris en charge ; l’identité sémantique du point d’entrée reste partielle.
- MakeVehicle et PlaceRandomBus natifs de base sont validés, mais le placement/affectation déterministe sans tête de PlayerVehicle n'est pas une fonctionnalité publique prise en charge.
- Les champs de position sont inspectables. La relocalisation arbitraire des véhicules, la reliure spatiale entre les tuiles et l'autorité de transformation sécurisée ODE ne sont pas prises en charge.

## Mutation d'exécution

- Les écritures de variables de script numériques sont prises en charge uniquement via le chemin sémantique profilé.
- Les écritures de variables de chaîne, les déclencheurs nommés de véhicules et les déclencheurs sonores d'objets ne sont pas disponibles en attendant une limite de durée de vie sûre des chaînes gérées par Delphi.
- `SetActualDateTime` et la mutation explicite du calendrier ne sont pas disponibles en attendant la fermeture de l'ABI/postcondition.
- La météo actuelle/OACI peut être lue, mais la configuration/activation/actualisation n'est pas disponible.

## État avancé

- Certains champs avancés de carte/tuile/chemin/spline/graphique d'objet de la version actuelle sont partiels car la représentation `Kacheln` n'est pas entièrement réconciliée.
- 'NoRVNumbers' et les représentations d'horaires détaillés sélectionnées restent partielles.
- Les API avancées sont des instantanés typés selon le profil, et non un contrat pour un accès arbitraire à la mémoire ou l'utilisation d'un pointeur natif.

##D3D

La création, la description, la mise à jour, la publication, le rejet de poignées obsolètes/libérées, le nettoyage à sortie forcée et la relance de textures sont éprouvés à l'exécution. Les véritables « DEVICELOST », « Reset » d'OMSI, les pertes/réinitialisations répétées, les courses de demande contre réinitialisation et les preuves d'événements publics perdus/réinitialisés/restaurés restent expérimentales/partielles.

## Sécurité des sessions et des configurations

Toutes les modifications de configuration et de fichiers OMSI temporaires sont limitées à la session. `options.cfg`, `keyboard.cfg`, `gamectrler.cfg`, les superpositions de splash et autres artefacts temporairement touchés sont des instantanés/journaux/restaurés. La fermeture permanente du plugin OmsiLaunch sous `plugins\\OmsiLaunch.*` est installée avec le produit et n'est pas un participant à la transaction. La modification permanente de la configuration ne relève pas de la portée de la version bêta 0.1.

## Éléments de présentation

`.omsilaunch` est un répertoire d'installation appartenant à OmsiLaunch, pas un fichier ou un
remplacement permanent de la configuration OMSI. Ses ressources de démarrage par défaut persistent sous la forme
actifs du produit ; Les remplacements de l'interface graphique `NewSplashscreen_*.bmp` sont réservés à la session et
restauré. Un hôte interrompu sans journal durable ne peut pas déduire en toute sécurité
si un fichier GUI restant arbitraire appartient à l'utilisateur, donc récupération délibérée
ne le supprime pas aveuglément.

