> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Carregando mundo

Situação: NORMATIVA

`NEW_MAP` é o caminho canônico validado em tempo de execução. Mapa e identidade do ponto de entrada
são valores semânticos; um índice de ponto de entrada apresentado é um diagnóstico/baixo nível
substituição, não uma identidade persistente.

A transferência de inicialização v4 pode conter um seletor de ponto de entrada e uma situação salva canônica
identidade ao limite de inicialização nativo. O seletor de ponto de entrada atinge o nativo
Limite de apresentação `Tform_setpos`. O adaptador pode corresponder exatamente a um único
Rótulo de lista apresentada Unicode e, em seguida, delega a seleção ao existente
Fluxo `Button1Click`. Ele nunca volta silenciosamente para um índice. O público
a identidade canônica permanece limitada pela capacidade: os rótulos `global.cfg` brutos são
não necessariamente os rótulos apresentados e podem ser duplicados, portanto sua exata
a correlação de tempo de execução é necessária antes que uma identidade se torne iniciável.

A descoberta offline expõe cada registro `[entrypoints]` como
`<mapa>#ponto de entrada:<sha256-of-normalized-record>`. Este é um conteúdo estável
identidade em vez de um ordinal ou rótulo, mas permanece apenas para descoberta até
o perfil fecha seu mapeamento para a lista nativa apresentada. O validado
Registros de linha de base de Grundorf `índice apresentado 1 -> índice bruto 0 -> Nordspitze
Bauernhof`; a representação do texto apresentado pela VCL ainda não foi resolvida.
a correlação requer uma identidade estruturada antes do lançamento.

`SAVED_SITUATION` representa um caminho canônico `.osn` selecionado e usa o
ramificação de situação do formulário inicial nativo. O perfil registra o limite da evidência:
`Tform_start+0x3E4` é o controle da situação selecionada, `+0x3F8` é seu controle ordenado
coleção nativa e envio é `Tform_start.LoadSelectedSituation`
(`0x0064307C`) para `Omsi23004_692EBFBF`. A ponte nativa atual resolve o
caminho canônico naquela coleção Unicode ativa, mas nunca invoca esse despachante
diretamente: fazer isso ignora o estado de propriedade do formulário. Ele seleciona a rádio de propriedade da OMSI
modo, define o seletor de perfil por meio de seu setter VMT nativo, invoca o
manipulador de sincronização de seleção e, em seguida, executa `Button1Click`. Isto é
`RUNTIME_VALIDATED`: a situação instalada Berlim-Spandau
`situations\\Baustelle Falkenseer Ch..osn` alcançou `gameplay.entered` através
o caminho da sessão pública. Seu carregamento nativo demorou cerca de 54 segundos, então o Current
o tempo limite de inicialização padrão é de 180 segundos; os chamadores podem fornecer uma explicação mais explícita
vinculado quando seu conteúdo é conhecido por carregar mais rápido.

Antes de preparar qualquer artefato de tempo de execução, `PlanSession` resolve o mapa declarado por
o `.osn` selecionado em relação à instalação atual. Um mapa ausente é rejeitado
como `OL_E_SITUATION_MAP_NOT_FOUND`; OmsiLaunch não permite o carregador nativo
para apresentar um erro modal de conteúdo ausente e bloquear o caminho de inicialização sem cabeça.

`LAST_MAP_STATE` significa o comportamento nativo de restauração automática do último mapa. É
não o mais novo `.osn`, um carimbo de data/hora do sistema de arquivos ou uma heurística de ordem de diretório.
`laststn.osn` e `laststn.osn.owt` são evidências apenas até que o branch nativo seja
fechado. A capacidade está, portanto, indisponível para o perfil atual.

