> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Portabilité héritée

Statut : NORMATIF
État de mise en œuvre : FUTUR

La portabilité héritée est une contrainte de conception et non un objectif de mise en œuvre actuel.

## Objectif

Une fois que Current est stable et avant que Current ne quitte sa ligne de base .NET initiale,
geler une « base de port héritée » fonctionnelle pour deux rétroportages explicites : Legacy NT6
et XP hérité.

## Principe

L'actuel n'est pas rendu ancien pour prendre en charge l'ancien Windows. Sémantique LaunchSpec, publique
résultats/erreurs, données BuildProfile, identités de contenu, sémantique de configuration,
le protocole de transfert de démarrage et les identités d'opération natives restent portables ;
les implémentations de plate-forme varient en dessous.

`OmsiBuildProfile` est indépendant de `RuntimePlatform`. DNNE est un courant
adaptateur plugin-hôte, pas une exigence de protocole.

## Cibles héritées

L'ancien NT6 cible Vista SP2 x64, Windows 7 SP1 x64, Windows 8 x64 et
Windows 8.1x64. Legacy XP cible uniquement Windows XP SP3 x86. Le véritable héritage
les chaînes d'outils sont sélectionnées dans leurs jalons de port dédiés.

## Compatibilité des fils

Les structures de fil de démarrage/session sont versionnées ; utiliser des champs à largeur fixe, UTF-8
des chaînes avec des longueurs explicites et des règles d'emballage/alignement/endian documentées.
Ils n'utilisent jamais la sérialisation des objets CLR ou la disposition dépendante de la taille du pointeur.

## Reproductibilité

Là où la construction/le contenu d'OMSI le prennent en charge, la même LaunchSpec sémantique doit être
rejouable sur les implémentations de plates-formes actuelles et héritées pour des tests de référence et
recherche d'optimisation.

## Séquence de développement

1. Complétez et stabilisez le courant.
2. Gelez/marquez `legacy-port-base`.
3. Créez `legacy/nt6` et `legacy/xp`.
4. Effectuez des rétroportages explicites et conservez ces branches.
5. Modernisez Current seulement après que cette référence existe.

L’implémentation héritée ne commence pas lors de la version actuelle initiale.

