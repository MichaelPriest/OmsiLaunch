> Traduction localisée. La documentation canonique anglaise prévaut en cas de divergence technique.

# Découverte de contenu

Statut : NORMATIF

La découverte est hors ligne et en lecture seule. Les cartes utilisent `maps\...\global.cfg` ; situations
utilisez `situations\...\.osn` ; les véhicules utilisent des fichiers « .bus » relatifs à OMSI ; les repeints sont
Identités CTI à l'échelle du véhicule. HOF, numéro de flotte, enregistrement et module complémentaire
les résultats rapportent uniquement les preuves disponibles dans le contenu installé. Commande découverte ou
les horodatages de fichiers ne définissent jamais la sémantique de l'état enregistré au moment de l'exécution.
## Points d'entrée

`EnumerateEntrypoints(mapIdentity)` analyse les `[entrypoints]` observés par le profil
enregistrements dans `global.cfg`. Son identité canonique est l'identité cartographique plus un
SHA-256 de l'enregistrement brut normalisé complet. Les étiquettes sont des métadonnées d'affichage
uniquement, car les cartes OMSI peuvent contenir des étiquettes en double. Discovery ne prétend pas
que cet enregistrement brut a été corrélé au runtime `Tform_setpos`
liste présentée.

