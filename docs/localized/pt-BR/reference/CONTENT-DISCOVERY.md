> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Descoberta de conteúdo

Situação: NORMATIVA

A descoberta está offline e somente leitura. Os mapas usam `maps\...\global.cfg`; situações
use `situações\...\.osn`; os veículos usam arquivos `.bus` relativos ao OMSI; repinturas são
identidades CTI com escopo de veículo. HOF, número da frota, registro e complemento
os resultados relatam apenas evidências disponíveis no conteúdo instalado. Ordem de descoberta ou
os carimbos de data e hora do arquivo nunca definem a semântica do estado salvo em tempo de execução.
## Pontos de entrada

`EnumerateEntrypoints(mapIdentity)` analisa os `[pontos de entrada]` observados no perfil
registros em `global.cfg`. Sua identidade canônica é a identidade do mapa mais um
SHA-256 do registro bruto normalizado completo. Os rótulos são metadados de exibição
apenas porque os mapas OMSI podem conter rótulos duplicados. A descoberta não reivindica
que este registro bruto foi correlacionado ao tempo de execução `Tform_setpos`
lista apresentada.

