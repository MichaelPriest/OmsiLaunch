> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# API publique

Statut : NORMATIF

« OmsiLaunch.Api » est la limite du produit. Les consommateurs construisent une sémantique
`LaunchSpec`, appelez `PlanSessionAsync`, puis appelez `StartSessionAsync` uniquement pour un
plan exécutable. La CLI est un consommateur de référence de cette API et ne contient aucun
implémentation OMSI distincte.

Les contrats publics ne contiennent jamais de pointeurs OMSI, de handles Win32, de types DNNE, de CLR
objets, détails de mémoire partagée ou valeurs dépendant de la largeur du pointeur. Contenu
les identités sont des chemins canoniques relatifs à OMSI. `GetCapabilitiesAsync` et un
`SessionPlan` expose la disponibilité des opérations spécifiques au profil de construction.

`StartSessionAsync` revient après la propriété de la session, la préparation transactionnelle et
une supervision du processus est établie. Cela ne promet pas de gameplay. Consommateurs
attendez `SessionState.Running` ; pour les demandes de lancement sans tête, cela signifie le
l'hôte a observé « gameplay.ented » à partir du runtime en cours.

`World.EntrypointIdentity` reste limité aux capacités jusqu'à ce que son brut structuré
Le mappage du point d’entrée vers la liste présentée est fermé pour le profil actif. Le
l'adaptateur natif rejette déjà les étiquettes présentées absentes ou ambiguës, mais les appelants
utilisez `PresentedEntrypointIndex` comme sélecteur de bas niveau pris en charge en attendant.

`DiscoverAsync` est en lecture seule. La découverte de modules complémentaires concerne uniquement l'inventaire : OmsiLaunch
n'active, ne désactive pas, ne modifie pas les droits et n'altère pas les modules complémentaires Steam.

Toute configuration dans un `LaunchSpec` est un état de session temporaire. L'API publique
n'expose pas l'édition permanente de la configuration : chaque configuration touchée
Le fichier est un instantané de transaction, journalisé, vérifié et restauré octet par octet.

## Présentation de la séance

`SessionPresentationSpec.Splash` est `Géré` par défaut. Il sélectionne le
Splash par défaut d'OmsiLaunch depuis `<installation>/.omsilaunch/assets/splash` et
superpose temporairement les fichiers de démarrage de l'interface graphique OMSI. `CustomAssetDirectory` sélectionne un
répertoire des actifs de session/projet ; les chemins relatifs sont résolus sous l'installation.
`Unset` (et l'alias de compatibilité `Native`) préserve explicitement le natif
OMSI splash et n'ajoute aucune mutation de splash GUI. Cette politique de présentation reste
limité à la session ; `.omsilaunch` est l'état du produit, pas une modification persistante d'OMSI
configuration.

## API avancée D3D9

`D3DRuntimeApi` fournit des extensions de session typées pour l'état et la texture du périphérique
créer, décrire, mettre à jour et publier. Les modèles publics sont `D3DDeviceStatus`,
`D3DTextureHandle`, `D3DTextureDescription`, `D3DTextureUpdate`,
`D3DDeviceState`, `D3DTextureFormat` et `D3DTextureResourceState`.

Les handles sont des valeurs opaques à l’échelle de la session. Les consommateurs ne peuvent pas obtenir de COM
pointeur, adresse OMSI ou handle Win32. Libéré, génération obsolète et
les valeurs intersessions échouent avec les codes structurés `OmsiRuntimeException`. Publique
l'état de la session expose également la collection ordonnée délimitée `RuntimeEvents`,
qui transporte les événements du cycle de vie D3D avec des données d'horodatage, de séquence et sémantiques.

Les méthodes typées sont des wrappers sur le même canal de commande d'exécution public :

```csharp
var status = attendre launch.GetD3DStatusAsync(session, TimeSpan.FromSeconds(5));
var texture = attendre le lancement.CreateD3DTextureAsync(
    session, 64, 64, D3DTextureFormat.A8R8G8B8);
attendre le lancement.UpdateD3DTextureAsync(session, texture.Handle,
    nouveau D3DTextureUpdate(0, 0, 0, 64, 16, pixels));
attendre le lancement.ReleaseD3DTextureAsync(session, texture.Handle);
```

Les appels ne sont valides que lorsque la session propriétaire est « RUNNING » et que la session correspondante
BuildProfile est actif. La réinitialisation ne recrée ni ne recible un handle.

