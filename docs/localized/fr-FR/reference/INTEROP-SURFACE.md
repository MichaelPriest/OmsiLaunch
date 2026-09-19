> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Surface d'interopérabilité OMSI

Statut : NORMATIF

« OmsiLaunch.Interop » est la limite OMSI ABI x86 uniquement. Il reconstitue le
parties réutilisables d'OmsiHook sans importer l'attachement externe ou le RPC d'OmsiHook
architecture dans le produit OmsiLaunch.

## Règles

- Tous les pointeurs OMSI sont des adresses 32 bits non signées explicites dans Interop.
- Les contrats d'API publics n'exposent jamais de pointeurs, descripteurs, de types DNNE ou gérés.
  Wrappers d’objets OMSI.
- La lecture de chaînes et de tableaux Delphi est générique ; l'écriture ou l'attribution nécessite
  un allocateur natif explicite et contrôlé par profil.
- Un wrapper de domaine devient exécutable uniquement lorsque le `OmsiBuildProfile` actif
  fournit l'adresse/le décalage exact ainsi que les gardes d'octets natifs.
- Le remplacement générique de la collection Delphi est interdit. Cycle de vie natif
  les opérations possèdent des mutations dans les collections OMSI.

## Fondation reconstruite

| Capacité | État | Mise en œuvre |
| --- | --- | --- |
| Adresses distantes x86 à largeur fixe | MISE EN ŒUVRE | `OmsiRemoteAddress` |
| Mémoire scalaire lecture/écriture | MISE EN ŒUVRE | `OmsiMemoryPrimitives` |
| Delphi UnicodeString lire | MISE EN ŒUVRE | `ReadStringAsync` |
| Lecture de chaîne Delphi ANSI | MISE EN ŒUVRE | `ReadStringAsync` |
| Tableaux de chaînes/pointeurs/struct Delphi | MISE EN ŒUVRE | `OmsiDelphiValues` |
| Allocation de chaîne/tableau distant | IMPLÉMENTÉ, allocateur natif de profil requis | `IOmsiRemoteAllocator` |
| Instantanés de collection d'objets en lecture seule | MISE EN ŒUVRE | `OmsiObjectCollection<T>` |
| Instantanés de collection de pointeurs `TList`/`OList` | MISE EN ŒUVRE | `OmsiPointerList<T>` avec disposition fournie par le profil |
| Instantanés de chaîne Delphi `TList`/`OList` | MISE EN ŒUVRE | `OmsiStringList` avec mise en page et encodage fournis par le profil |
| Accès aux champs d'objets validés par le profil | MISE EN ŒUVRE | `OmsiProfiledObject` valide VMT avant chaque accès au champ |
| Répartition de la réflexion structurelle | DIFFÉRÉ | Nécessite un emballage/mise en page confirmé et un consommateur concret |
| Attachement de processus / RPC externe | EXCLU | OmsiLaunch est propriétaire du cycle de vie des processus et du transfert de démarrage |

`OmsiRuntimeSurface` répertorie les opérations sémantiques héritées de OmsiHook
domaines utiles. Une entrée de catalogue ne constitue pas une revendication autorisée par le profil actuel.
l'opération. La disponibilité est résolue par « OmsiBuildProfile » et les gardes natifs ;
`OmsiRuntimeSurface.Resolve` rend cette décision explicite pour un adaptateur de profil.

