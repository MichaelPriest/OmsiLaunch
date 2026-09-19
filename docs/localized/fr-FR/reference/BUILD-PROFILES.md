> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Créer des profils

Statut : NORMATIF

`OmsiBuildProfile` est indépendant de `RuntimePlatform`. Il possède l'exécutable OMSI
empreintes digitales, valeurs globales, formulaires, champs, méthodes, sites d'appel et protections d'octets natifs.
Il ne code jamais la compatibilité de la génération hôte Windows.

« Omsi23004_692EBFBF » accepte uniquement ces valeurs LAA SHA-256 connues :

- '692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243'
  (`ALTERNATE_LAA`, validé à l'exécution) ;
- '7DAB063D1F62E73B3A2C7A6AC1921D7EDF5E5DB0FBC731481D117EEC8DE7D759'
  (`STEAM_LAA`, rapprochement statique ; validation du champ d'exécution bêta en attente).

Aucune chaîne de version, nom d'exécutable, taille de fichier ou marqueur LAA générique n'est un
critère d’acceptation. Tout autre hachage est rejeté avant la préparation du runtime natif.
Le profil enregistre `SetActualDateTime`, l'envoi de la situation enregistrée et le
primitives de véhicules observées dans les preuves statiques/en amont. Enregistrer un symbole est
pas une autorisation pour l'appeler : chaque wrapper ABI natif nécessite le sien
garde d'empreintes digitales et contrat d'appel éprouvé.

Le profil enregistre également l'emplacement du candidat D3D actuel « 0x008627D0 » comme
`Périphérique D3DD`. La session d'exécution `3cbd7bb6-d73f-4853-adc8-1213200527d2` a été prouvée
que le candidat à l'emplacement emprunté prend en charge `QueryInterface<IDirect3DDevice9>`,
`TestCooperativeLevel` renvoie `S_OK` et toutes les opérations D3D s'exécutent sur le
Fil principal de l'interface utilisateur/principal/propriétaire du rendu d'OMSI. OmsiLaunch conserve une référence QI ;
l'emplacement emprunté lui-même n'est jamais libéré. Cette entrée n'est pas généralisée à
une autre empreinte digitale exécutable.

L'appelant statique correspondant pour le candidat référence `TProgMan.MakeVehicle`
globals à `0x00859DEC`, `0x008591DC` et `0x00858D28`. Leur liste/index exact
les rôles restent non résolus, ils ne sont donc délibérément pas nommés BuildProfile
symboles. Ce ne sont pas des indications publiques et ne constituent pas un document complet.
implémentation joueur-véhicule.

