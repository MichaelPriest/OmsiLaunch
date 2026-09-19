> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

<p align="center">
  <img src="assets/branding/omsilaunch-logo.png" alt="OmsiLaunch" width="620">
</p>

<p align="center"><strong>Contrôle de session pour OMSI 2.</strong></p>
<p align="center">Open source · Programmable · Piloté par la communauté</p>

---

# OmsiLancement

**OmsiLaunch** est un lancement programmable open source, une gestion de session,
et couche de contrôle d'exécution pour OMSI 2. Il fournit une API publique, un
CLI, un plugin/runtime OMSI en cours et un « BuildProfile » de construction exacte
frontière. Il s'agit d'une infrastructure pour les lanceurs, les outils, l'automatisation et la communauté
intégrations plutôt qu’un lanceur graphique.

> **Définissez la session, pas les clics.**

## Bêta 0.1

La première version bêta publique est **0.1.0-beta1**. Il prend en charge le profil OMSI exact
`Omsi23004_692EBFBF`, valide l'empreinte de l'exécutable avant le lancement, et
ne revendique pas la compatibilité avec les versions OMSI inconnues. Le colis et détaillé
les états de compatibilité sont documentés dans [`docs/`](../README.md).

OmsiLaunch prend en charge la planification de sessions sémantiques avec `LaunchSpec` et
`PlanSession`, démarrage via `StartSession`, contrôle d'exécution observé et
nettoyage normal `StopSession`/`CloseSession`. Les fonctionnalités d'exécution prises en charge sont
explicitement catalogué; les capacités expérimentales et indisponibles ne sont pas cachées.

## Propriété de session sécurisée

Tous les remplacements de configuration de lancement sont limités à la session. Instantanés OmsiLaunch,
journalise, applique, vérifie et restaure chaque configuration temporaire ou interface graphique
fichier, il change. Il n'offre pas d'édition de configuration permanente dans cette version bêta.

Les fichiers des plugins du produit sont installés de manière permanente sous `plugins\\OmsiLaunch.*`.
Ils ne sont pas copiés et supprimés à chaque session, ni les plugins tiers
ne sont jamais détenus de manière transactionnelle. `.omsilaunch\\` est un fichier privé OmsiLaunch
répertoire pour les actifs, les diagnostics, les journaux et les exemples d'utilisateurs.

La présentation splash gérée est la présentation par défaut. Produit PTB, ENG, DEU et FRA
les actifs sont temporairement superposés et restaurés exactement. Splash natif/`Non défini`
Le mode préserve les fichiers OMSI.

## Télécharger

Téléchargez **`OmsiLaunch-0.1.0-beta1.zip`** depuis la page de version du projet et
extrayez-le directement dans la racine OMSI prise en charge. Le forfait comprend le
contrôleur, ses dépendances requises, la fermeture permanente du plugin, splash
des ressources, un exemple de session Release et un petit guide de l'utilisateur hors ligne.

Voir [Installation](../README.md) et
[Première session](../README.md). Utiliser
`OmsiLaunch.exe /version` pour inspecter le contrôleur installé.

## Pour les développeurs

La surface de produit préférée est l’API publique sémantique. La CLI est un
interface de référence sur cette même API ; il ne contient aucune logique OMSI distincte.
Le contrôle d'exécution s'étend à la session, est validé par le profil et utilise une sémantique opaque.
gère plutôt que des pointeurs natifs publics.

- [API publique](../README.md)
- [Contrôle d'exécution](../README.md)
- [Catalogue de capacités](../README.md)
- [Limites connues](../README.md)
- [Construire une politique de profil](../README.md)

## Communauté et licence

OmsiLaunch est un projet open source axé sur la communauté. Il est indépendant de
d'autres lanceurs OMSI et peuvent être utilisés par des outils communautaires compatibles.

OmsiLaunch est sous licence [LGPL-3.0 uniquement] (LICENCE). Voir le
[avis de tiers](../README.md) pour les sources incorporées
provenance et mentions applicables.

