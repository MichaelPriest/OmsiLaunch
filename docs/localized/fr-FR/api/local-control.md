> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Protocole de contrôle local

`OmsiLaunch.exe /serve` possède une session gérée et expose une session réservée à l'utilisateur actuel
point de terminaison de contrôle de canal nommé. Les invocations secondaires `OmsiLaunch.exe` agissent comme
clients ; ils ne créent jamais d'hôte concurrent pour cette session.

Version du protocole : `0.1`.

Itinéraires clients pris en charge :

- `statut de la session --json`
- `arrêt de session --json`
- `événements lus --json`
- `events watch --json` (jusqu'à `Ctrl+C`)
- Alias d'exécution tels que `time get --json` et `time set --hour=18 --minute=10 --json`

Chaque requête et réponse est au format JSON avec un préfixe de longueur et est limitée à 64 Ko. Le
le point de terminaison est uniquement local et utilise `PipeOptions.CurrentUserOnly`. C'est séparé
de la boîte aux lettres hôte-plug-in, qui reste liée à la session/au processus et n'est pas
une surface IPC exécutable publique.

Lorsqu'aucun hôte actif n'existe, les clients renvoient `OL_E_NO_ACTIVE_SESSION` et quittent
avec le code « 4 ». Une requête client ne démarre jamais implicitement OMSI.

Chaque résultat est une enveloppe avec `ok`, `command`, `protocol_version` et
soit un « résultat » soit une « erreur » structurée. Le contrat initial de code de sortie stable
est : `0` succès, `2` arguments invalides, `3` profil non pris en charge, `4` non
session active, `5` runtime indisponible, `6` introuvable, `7` opération
rejeté, échec de récupération « 8 » et échec interne « 10 ». Catégories d'erreurs
sont sémantiques (`invalid_argument`, `unsupported_profile`, `session`,
`runtime`, `not_found`, `transaction` ou `internal`); les appelants ne doivent pas analyser
messages lisibles par l'homme.

Les commandes d'exécution reçoivent des ID de requête uniques au sein de la session propriétaire. Le
le canal public ne transporte aucun pointeur OMSI et n'expose pas l'hôte au plugin
nom de la boîte aux lettres. Tous les handles renvoyés par une commande d'exécution restent limités à la session
et deviennent obsolètes lorsque le propriétaire arrête ou ferme la session.

Les noms d'opérations d'exécution inconnus sont rejetés localement car
`OL_E_RUNTIME_OPERATION_UNKNOWN` ; la CLI ne crée pas de session ni ne transfère
une opération native non reconnue.

Le registre de commandes public actuellement actif est disponible via :

```powershell
Capacités d'OmsiLaunch.exe --json
Heure d'aide OmsiLaunch.exe --json
```

