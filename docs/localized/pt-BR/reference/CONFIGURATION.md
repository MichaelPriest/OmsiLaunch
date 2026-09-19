> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

#Configuração

Situação: NORMATIVA

Todas as substituições de configuração do LaunchSpec têm escopo de sessão. Instantâneos do OmsiLaunch
cada um tocou em `options.cfg`, `Inputs\keyboard.cfg` ou `Inputs\gamectrler.cfg`
arquivo antes de aplicar uma sobreposição, registra-o no diário de transações duráveis,
e restaura os bytes originais e SHA-256 após a saída normal, `StopSession`,
falha de inicialização ou recuperação de diário obsoleto. Não há API de edição permanente.

`UNSET` é preservado: não produz mutação. Tokens desconhecidos, pedidos,
codificação, novas linhas, caudas de vetor e valores não relacionados sobrevivem a patches sem perdas.

## Superfície de opções implementadas

`ConfigurationCatalog` atualmente suporta sobreposições de sessão comprovadas para uso geral,
visualização, controles, colisão, ticket, salvamento automático, distância/complexidade gráfica,
reflexão de estêncil/chuva, tráfego, som e os `sistemas de fumaça` de quatro valores
bloco. A semântica negativa pública é invertida no limite do codec; para
exemplo `simulation.collisionTerrain=false` escreve `no_collision_terrain`.

`advanced.reducedMultithreading` é uma configuração semântica e sincroniza ambos
sinalizadores nativos de multithreading reduzido. `AIMaxCountRandom` corrige apenas a estrada
tráfego ou componente humano e preserva os outros sete valores vetoriais.

`graphics.realTimeReflections` atualmente aceita apenas `economy` e `full`.
A representação nativa da seleção Desativado da IU permanece deliberadamente
não gravável até que uma evidência estática o feche. `gráficos.textura` e
`graphics.textureFilter` são conhecidos, mas não graváveis pelo mesmo motivo.

`graphics.particles` é um valor composto:
`habilitado,maxPerEmitter,playerVehicleOnly,inReflections`. Ele traduz o
quarto campo nativo (`disableInReflections`) sem expor esse nome negativo.

