> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# LaunchSpec

Situação: NORMATIVA

`LaunchSpec` são dados semânticos portáteis. `UNSET` significa preservar o existente
configuração OMSI e nunca deve ser resumido em `falso`, zero ou inventado
padrão. A representação JSON é o mesmo modelo usado por `/spec`; explícito
Os switches CLI substituem seus valores de arquivo.

Os grupos estruturais suportados são instalação, mundo, identidade de ponto de entrada ou
índice apresentado, data, hora, ano, clima, veículo do jogador, configuração,
entrada, diagnóstico e comportamento em tempo de execução. `NEW_MAP`, `SAVED_SITUATION` e
`LAST_MAP_STATE` são modos semânticos. `LAST_MAP_STATE` significa nativo do OMSI
ramo de restauração automática do último mapa; nunca é inferido do arquivo `.osn`
carimbos de data/hora ou ordem de diretório. Os índices numéricos da lista nativa não são
identidades de conteúdo persistentes.

Para `NEW_MAP`, `World.EntrypointIdentity` está reservado para uma estrutura futura
identidade canônica. Um rótulo `global.cfg` bruto não é suficiente porque pode
ser duplicado e pode diferir do rótulo nativo apresentado. Até o
profile fecha essa correlação, `PresentedEntrypointIndex` é o suportado
seletor de diagnóstico/baixo nível. A transferência de inicialização v3 carrega um campo de identidade
sem valores do tamanho de um ponteiro, mas o uso público permanece limitado pela capacidade.

A data e a hora usam registros explícitos de `ano/mês/dia` e `hora/minuto/segundo`.
O esquema não inclui versões do sistema operacional, identificadores Win32, objetos de tempo de execução CLR,
Detalhes de DNNE, mapeamentos de memória ou endereços nativos.

PlanSession é o compilador desses dados. Um plano só é inexequível quando
a capacidade necessária solicitada não está disponível. Capacidade opcional não solicitada
portões são informativos.

