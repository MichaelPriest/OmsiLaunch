> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Prise en charge de la plate-forme

Statut : NORMATIF

## OmsiLaunch actuel

Le support officiel actuel est intentionnellement restreint : final/dernier service
Windows 10 x86-64 et Windows 11 x86-64 actuellement pris en charge. Le système d'exploitation hôte et
L'hôte actuel externe est AMD64/x86-64 ; OMSI, son plugin in-process, et natif
L'interopérabilité OMSI reste x86.

Current ne prend pas en charge Windows 32 bits, ARM64, Vista, Windows 7, Windows 8,
Windows 8.1, Windows XP, Wine, Proton, Linux ou macOS. Plateformes non prises en charge
sont rejetés lors de la validation avant toute mutation d'installation.

## Dépendances d'exécution

La distribution finale du courant doit être autonome lorsque cela est possible. Les utilisateurs font
pas besoin de Visual Studio, du SDK .NET, de Git, Python, Ghidra ou CMake. Normale
le fonctionnement ne nécessite pas d'élévation lorsque l'installation OMSI est accessible en écriture par
l'utilisateur actuel.

## Familles héritées

Les familles héritées sont des ports historiques/de recherche distincts, et non un support actuel.

### Ancien NT6

Les cibles prévues sont Windows Vista SP2 x64, Windows 7 SP1 x64,
Windows 8 x64 et Windows 8.1 x64.

### XP hérité

La cible prévue est Windows XP SP3 x86. Windows XP x64 n'est pas une cible.

## Positionnement de sécurité

Les versions héritées sont destinées aux systèmes hors ligne, à compatibilité historique, contrôlées
des références et des recherches de reproductibilité. Ils ne sont pas recommandés pour des activités normales
utilisation connectée à Internet.

