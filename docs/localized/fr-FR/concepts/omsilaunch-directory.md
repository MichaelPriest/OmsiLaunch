> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Répertoire d'état d'installation `.omsilaunch`

`.omsilaunch` est un répertoire sous la racine d'installation d'OMSI. Ce n'est jamais un
Fichier LaunchSpec et n'est pas déployé dans les « plugins ».

Le Release ZIP est extrait directement dans la racine OMSI. `OmsiLaunch.exe` et
ses assemblys de contrôleur restent à cette racine, tandis que le plugin OMSI permanent
La fermeture est installée directement sous `plugins\\OmsiLaunch.*`. Ces fichiers appartiennent
à l'installation du produit : les sessions valident leurs hashs mais jamais de mise en scène,
instantané, restaurez-les, supprimez-les ou utilisez-les comme source de déploiement temporaire.

## Création et propriété

Le répertoire est créé paresseusement. Une session splash gérée crée
`<OMSI>/.omsilaunch/assets/splash` après avoir acquis le bail d'installation et
avant que la transaction soit appliquée. Les actifs OmsiLaunch du stock manquant sont copiés
à partir du répertoire package `.omsilaunch/assets/splash`. Les actifs existants ne sont jamais
écrasé, permettant à un ensemble d'actifs de projet explicitement géré de persister.

Le répertoire appartient à l’État d’OmsiLaunch. Elle est distincte de la configuration OMSI
et depuis l'installation permanente du plugin.

## Mise en page

| Chemin | Durée de vie | Objectif |
|---|---|---|
| `actifs/splash/{PTB,ENG,DEU,FRA}.bmp` | Ressource OmsiLaunch persistante | Ressources de démarrage gérées 640 x 480 24 bits par défaut. Aucun numéro de version n’est intégré. |
| `diagnostics/<session>-host.log` | Diagnostic persistant | Trace de transition sémantique de l’hôte. |
| `diagnostics/*-runtime-*.json` | Diagnostic persistant | Preuve explicite de commande/lot d’exécution. |
| `journal.json` | État de transaction durable et temporaire | Instantané exact, identité du processus et progression de la restauration. Supprimé après une restauration normale ; conservé uniquement pour récupération. |
| `*.omsilaunch.tmp` à côté d'un fichier OMSI touché | Éphémère | Mise en scène d'écriture atomique ; supprimé ou remplacé atomiquement. |

Seules les modifications OMSI temporaires, telles que `GUI/NewSplashscreen_*.bmp` et demandées
superpositions de configuration, entrez la transaction instantané/journal/restauration.
Les plugins tiers ne sont jamais répertoriés pour leur propriété, copiés, supprimés ou
restauré.

Les mappages de transfert de démarrage, de télémétrie et de commandes d'exécution sont nommés partagés
objets de mémoire. Ils sont liés à la session/au processus et ne sont **pas** des fichiers en dessous de celui-ci.
répertoire.

## Sélection Splash

`SessionPresentationSpec.Splash` a trois modes sémantiques :

- `Géré` (par défaut) : OmsiLaunch installe `ENG` plus la langue résolue
  cible dans `GUI/NewSplashscreen_*.bmp` via la transaction existante. Le
  Les fichiers OMSI originaux sont restaurés octet par octet après une sortie, un échec ou
  récupération.
- `Native` ou `Unset` : préserve les fichiers de démarrage OMSI ; aucune superposition de splash GUI n'est
  créé.

La résolution linguistique accepte `PTB`, `ENG`, `DEU` et `FRA` ; lorsqu'il est absent, il utilise
`options.cfg` puis revient à `ENG`. `/splash-assets:<répertoire>` ou
`Presentation.CustomAssetDirectory` sélectionne un répertoire personnalisé explicite. Un
Le répertoire personnalisé relatif est résolu sous la racine d'installation d'OMSI. Il faut
contenir un `ENG.bmp` valide et, pour une langue sélectionnée autre que l'anglais, sa correspondance
BMP localisé. Les ressources doivent être des fichiers BMP 640 x 480 et 24 bits.

Actifs packagés/par défaut absents, format d'actif non valide ou personnalisation non valide
échec de planification/démarrage du répertoire avec une erreur d'éclaboussure sémantique ; OMSI n'est pas
lancé. Un répertoire `.omsilaunch` manquant lui-même est normal et est créé comme
décrit ci-dessus.

## Validation de la version

`tools/Test-ReleasePresentation.ps1` valide l'exécutable Release packagé
et peut exécuter les trois dossiers de présentation contre un OMSI autorisé
mise en place. Sa passe par défaut ne mute pas et vérifie les actifs du package ainsi que
les trois résultats de « PlanSession ». `-RunOmsi` prouve également le temporaire
superposition pendant l'exécution d'OMSI, restauration exacte de l'interface graphique et nettoyage du journal/processus.
Avec `-RunOmsi -InstallPackage`, il copie uniquement les fichiers appartenant au produit du package.
fichiers racine et fichiers `plugins/OmsiLaunch.*` dans la racine de test autorisée, puis
vérifie également que les hachages des plugins tiers restent inchangés. Le résultat est
persisté sous le nom "diagnostics/release-presentation-validation".json`.

## Fichiers de configuration

LaunchSpec JSON est fourni via `/spec:<file.json>`. Un exemple de distribution est
`exemples/.omsilaunch/canonical-session.json` ; le répertoire imbriqué est un exemple
mise en page uniquement. Il peut être copié dans le répertoire `.omsilaunch` d'une installation,
mais le fichier de configuration n'est pas requis pour que le répertoire fonctionne.

