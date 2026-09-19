> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Status de implementação

Status: RUNTIME_NEW_MAP_PASS

| Componente | Estado | Notas |
| --- | --- | --- |
| Abstração RuntimePlatform | STATICALMENTE_VALIDADO | O atual provedor Windows x64 foi implementado. |
| Provedor atual do Windows x64 | STATICALMENTE_VALIDADO | Valida a plataforma antes das operações de tempo de execução mutáveis. |
| ConstruirPerfil | STATICALMENTE_VALIDADO | `Omsi23004_692EBFBF` tem escopo de impressão digital. |
| Nativo.x86 | RUNTIME_VALIDATED | compilação v145 Win32; proteções de perfil/byte original exercidas no caminho canônico. |
| Base de interoperabilidade derivada do OmsiHook | STATICALMENTE_VALIDADO | Strings Delphi x86, arrays dinâmicos, instantâneos de objetos e limites explícitos de alocação remota; nenhum protocolo RPC upstream. |
| Interoperabilidade de plug-in em processo | STATICALMENTE_VALIDADO | `PluginRuntime` usa um provedor de memória em processo com escopo de perfil; anexação externa não é o caminho normal da sessão. |
| Canal de comando em tempo de execução | STATICALMENTE_VALIDADO | Caixa de correio de solicitação-resposta de largura fixa/SHA-256 vinculada à sessão despachada pelo temporizador de thread de UI do OMSI. |
| Leitores de mapa/clima/câmera em tempo de execução | RUNTIME_VALIDATED | Tempo validado da sessão canônica, mapa, leituras escalares meteorológicas com perfil completo, clima real e câmera por meio do canal de plug-in vinculado à sessão. |
| Gráfico avançado de mapa/bloco | BLOCKED_PROFILE_LAYOUT | O rótulo upstream `OmsiMap.Kacheln` em `Map+0x118` não resolveu para um cabeçalho de matriz dinâmica Delphi válido em três sessões atuais protegidas. A avaliação nunca grava e todas as sessões são restauradas normalmente. |
| Gravações de tempo de execução/tempo escalar | RUNTIME_VALIDATED | `time.set` aplicou `SetTime` com perfil com releitura; `weather.set` validou uma lista de permissões e restauração escalar apoiada por perfil. |
| Gravação FOV da câmera em tempo de execução | RUNTIME_VALIDATED | `camera.set` alterou e restaurou o FOV durante a sessão canônica; a família de câmeras permanece protegida. |
| Resumo do tempo de execução do veículo/humano/horário | RUNTIME_VALIDATED | O lote Canonical Grundorf retornou o estado do veículo rodoviário, a contagem humana e as contagens da matriz do gerenciador de horários sem expor ponteiros brutos. |
| Telemetria de cronograma detalhado em tempo de execução | RUNTIME_VALIDATED | Snapshots imutáveis ​​perfilados retornaram trilhas, viagens e linhas com seus nomes semânticos, caminhos, contagens e metadados de atribuição através do canal de sessão. |
| Paradas e links do horário de execução | RUNTIME_VALIDATED | Snapshots imutáveis ​​perfilados retornaram nomes de pontos de ônibus, IDs, links e metadados de endpoint de link de estação. `Nordspitze` foi lido ao vivo no campo UTF-16 Delphi. |
| Passeios com horários em tempo de execução | RUNTIME_VALIDATED | Instantâneos de tour aninhados com perfil retornaram afiliação de linha, grupo/tipo de IA, metadados de reserva de veículo e contagens de TourEntry sem expor ponteiros Delphi. |
| Perfis de horário de execução | RUNTIME_VALIDATED | Os instantâneos de perfil aninhados retornavam contagens de afiliação de viagem, nome, tempo total, tempo de parada e tempo de entrada no trajeto. |
| Entradas de tour no horário de execução | RUNTIME_VALIDATED | Os snapshots TourEntry aninhados com perfil retornaram identidade de viagem semântica, índices de viagem/perfil, tempo e estado de transição suave. |
| Entradas de faixa de horário de execução | RUNTIME_VALIDATED | Os instantâneos limitados do TrackEntry retornaram 91 registros ao vivo com afiliação de trilha, ID, bloco/caminho, distâncias, validade, ordem e metadados crono através do canal de sessão. |
| Horário de execução RVFiles | RUNTIME_VALIDATED | A matriz RVFile com perfil retornou uma coleção vazia válida em Grundorf por meio do canal de sessão. |
| Horário de execução NoRVNumbers | NÃO RESOLVIDO | A decodificação upstream `raw:true` entra em conflito com o layout do campo `Omsi23004_692EBFBF` observado. Removido do lote automático; não afeta outras telemetrias de horários. |
| Telemetria detalhada de veículo/humano em tempo de execução | RUNTIME_VALIDATED | Alças opacas com escopo de sessão e instantâneos somente leitura retornaram movimento/controles/iluminação/IA coerentes do veículo e alvo humano/bilhete/assento/estação/estado de IA em Grundorf. Nenhuma gravação de entidade espacial ou arbitrária está habilitada. |
| Variável de script de tempo de execução lida | RUNTIME_VALIDATED | Tabelas de nomes ANSI e o inA matriz de ponteiros `PublicVars` resolveu 1.025 nomes e `Refresh_Strings = 0` por meio de um identificador de veículo opaco. |
| Mutação de variável de script de tempo de execução | RUNTIME_VALIDATED | `vehicle.variable.set` resolve um identificador opaco ativo e o slot `PublicVars` com perfil pelo nome da string aberta, rejeita valores não finitos e retorna leitura imediata. `Refresh_Strings 0 -> 1 -> 0` passado com limpeza normal. |
| Leitura de variável de string em tempo de execução | RUNTIME_VALIDATED | Tabelas de nomes ANSI mais valores de string Unicode resolveram 27 nomes e `ident=GRN-V 30` por meio de um identificador de veículo opaco. As gravações de substituição/recontagem do Delphi permanecem separadas. |
| Leitores de metadados de tempo de execução Wave A | RUNTIME_VALIDATED | Metadados HOF, drivers, pacotes de ingressos e registros de horários passados ​​na sessão `e33aae7e-450a-4c00-8c7a-5f550ac4263f`; constantes e curvas passadas em `46260d64-a8e9-4ee9-a907-00e46b07a1ae`, todas com limpeza normal. |
| Instalação permanente do plugin | STATICALMENTE_VALIDADO | O Release ZIP coloca o encerramento do plugin OmsiLaunch diretamente em `plugins\\`. Uma sessão valida hashes exatos do produto antes do lançamento; ele nunca prepara, captura instantâneos, restaura, remove ou reivindica plug-ins de terceiros. |
| PlaceRandomBus | RUNTIME_VALIDATED | A sessão `4e2321d4-4c09-428b-8760-d348205b7392` retornou `2`, aumentou RoadVehicles de 2 para 4 e concluiu a limpeza exata. É diferente do make-basic e não faz nenhuma reivindicação do PlayerVehicle. |
| Gatilhos nomeados e mutação de string | BLOCKED_STRING_OWNERSHIP | O despacho de produção é retido intencionalmente até que a alocação, atribuição e vida útil da string Delphi possam ser encerradas sem herdar o risco de vazamento/corrupção upstream. |
| Catálogo de operações de domínio OmsiHook | STATICALMENTE_VALIDADO | Os domínios de programa, mapa, hora, clima, veículos, humanos, horários, câmera, player e som são controlados por semântica/perfil. |
| Dispositivo e texturas Wave D D3D | RUNTIME_VALIDATED | Aquisição de dispositivo de perfil exato, referência de QI de propriedade única, identificadores de sessão opacos, atualização/liberação de criação/descrição/completa e retificada, rejeição de liberação repetida, vários recursos, saída forçada e limpeza de reinicialização aprovada. |
| Ciclo de vida/redefinição do Wave D D3D | IMPLEMENTED_NOT_RUNTIME_VALIDATED | A interceptação de redefinição invalida os recursos do pool padrão antes da chamada nativa, avança a geração e publica transições ordenadas do ciclo de vida. A indução segura de perda/redefinição de dispositivo não estava disponível, portanto, os cenários de perda/redefinição/ciclo repetido/corrida permanecem bloqueados em vez de reivindicados. |
| Adaptador DNNE | STATICALMENTE_VALIDADO | O adaptador atual é separado do PluginRuntime. |
| PluginRuntime | RUNTIME_VALIDATED | Consumo de handoff, braço sem cabeça síncrono e execução genérica de NEW_MAP atingiram o jogo. |
| Transferência portátil | STATICALMENTE_VALIDADO | O protocolo UTF-8/SHA-256 de largura fixa v4 carrega pontos de entrada semânticos e identidades de situações salvas; A decodificação v3 permanece suportada e os testes de ida e volta/corrupção são aprovados. |
| Descoberta de conteúdo | STATICALMENTE_VALIDADO | Descoberta de mapa/situação/veículo/repintura/HOF off-line mais registros `[pontos de entrada]` com hash opaco; a correlação de lançamento do ponto de entrada permanece controlada separadamente. |
| Configuração | STATICALMENTE_VALIDADO | Apenas com escopo de sessão; no-op sem perdas, sinalizadores negativos, intervalos, blocos compostos e testes de preservação de vetores são aprovados. |
| Diretório de instalação `.omsilaunch` | RELEASE_VALIDADO | O estado do diretório é criado lentamente após a aquisição do arrendamento para apresentação gerenciada. Os recursos persistentes residem em “ativos”; os diagnósticos persistem; o diário durável é removido após a restauração. Não é um arquivo e nunca um destino de implantação de plugin. |
| Apresentação inicial gerenciada | RELEASE_RUNTIME_VALIDATED | `Gerenciado` é o padrão. Ativos empacotados PTB/ENG/DEU/FRA 640x480 de 24 bits residem em `.omsilaunch/assets/splash` e sobrepõem transacionalmente `GUI/NewSplashscreen_ENG.bmp` mais a localidade resolvida. As sessões de lançamento `388cf5d2-7310-4301-9313-e62c4c08a346` (padrão) e `794cae34-a2ac-4fcf-8981-f142ac14c308` (diretório personalizado) atingiram o jogo e concluíram a limpeza normal. Sessão `Desativada` `12a74b24-6c78-4863-8ffe-947651212eeb` atingiu o jogo sem nenhuma sobreposição de GUI planejada. Todos os três restauraram o estado original da GUI e removeram o diário; plug-ins de produtos permanentes não eram participantes da transação. |
| Pacote de lançamento | RELEASE_VALIDADO | `tools/New-ReleasePackage.ps1 -Configuration Release` produz `OmsiLaunch-current.zip` com manifesto SHA-256, fechamento de tempo de execução de versão, recursos iniciais, exemplo `.omsilaunch` e nenhum caminho de depuração em seu manifesto. |
| Transação/recuperação | RUNTIME_RECOVERY_VALIDATED | Um diário `HANDOFF_CREATED` obsoleto restaurou apenas artefatos de propriedade da sessão e removeu seu diário. A validação do proprietário requer um processo não encerrado, tempo de criação e caminho executável antes que a recuperação seja retida. |
| Planejar Sessão | STATICALMENTE_VALIDADO | O plano Canonical Grundorf resolve artefatos de tempo de execução somente leitura. |
| Início sem cabeça | IMPLEMENTADO/RUNTIME_VALIDATED | A inicialização nativa foi armada de forma síncrona e a jogabilidade foi o estado alvo observado. |
| NOVO_MAPA | IMPLEMENTADO/RUNTIME_VALIDATED | Grundorf apresentou índice 1 e Nordspitze Bauernhof concluído durante o jogo. |
| Identidade do ponto de entrada NEW_MAP | RUNTIME_PARTIAL | Os registros brutos offline têm hashes estáveis. O tempo de execução confirma que o índice apresentado por Grundorf `1` seleciona o índice bruto `0` (`Nordspitze Bauernhof`), mas o texto/mapeamento nativo apresentado permanece sem solução. As solicitações de identidade pública permanecem bloqueadas. |
| SAVED_SITUATION | RUNTIME_VALIDATED | `situations\\Baustelle Falkenseer Ch..osn` carregou Berlin-Spandau através do rádio de forma inicial com perfil completo, seletor, sincronização e sequência `Button1Click`, alcançou o jogo, permaneceu em RUNNING por 8 segundos, depois completou a parada solicitada e a restauração normal. PlanSession ainda rejeita arquivos `.osn` cujo mapa declarado está ausente antes da preparação. |
| LAST_MAP_STATE | UNSUPPORTED_FOR_CURRENT_PROFILE | A ramificação nativa exata de restauração do último mapa não está fechada; não existe nenhum substituto de ordem de arquivo. |
| Data-hora explícita/sistema | STATICAL_PARCIAL | Não solicitado pela regressão canônica. |
| Veículo do jogador | STATICAL_PARCIAL | Não solicitado pela regressão canônica. |
| Implementação do legado NT6 | FUTURO / N/D | Apenas limite arquitetônico. |
| Implementação do XP legado | FUTURO / N/D | Apenas limite arquitetônico. |

## Linha de base de tempo de execução canônica

`RUNTIME_NEW_MAP_PASS`: public `StartSessionAsync` validou a instalação do tempo de execução permanente e
transferência portátil, atingiu `gameplay.entered` para `maps\Grundorf\global.cfg`
com índice de ponto de entrada apresentado `1` (`Nordspitze Bauernhof`), permaneceu em RUNNING
por oito segundos e, em seguida, concluiu uma parada solicitada e uma restauração exata normal.

A concessão de instalação atual usa um semáforo nomeado do Windows (contagem máxima
one), não um mutex: a limpeza pode ocorrer em um thread gerenciado diferente. O durável
o diário e a identidade do processo continuam sendo a autoridade de recuperação de falhas.

O tempo limite de inicialização padrão é de 180 segundos. Isso preserva uma falha limitada
caminho, permitindo que situações nativas salvas em mapas grandes completem seu
trabalho legítimo de carregamento pré-jogo; os chamadores podem definir um tempo limite explícito mais curto.

## Apresentação de lançamento 001

Versão de compilação, suítes de unidade/integração, CLI empacotada `PlanSession`, pacote
validação de manifesto e recurso aprovada. Os relatórios executáveis empacotados
produto `OmsiLaunch`, versão do produto `0.1.0`, protocolo `0.1`, e usa o
ícone oficial de multi-resolução. Um lançamento gerenciado iniciado criado e com correspondência de hash
todos os quatro ativos splash persistentes em `.omsilaunch/assets/splash`. O lançamento
As sessões padrão, personalizada e `Unset` atingiram o jogo e depois foram `Completadas`.
Sobreposições padrão/personalizadas e preservação nativa foram selecionadas pelo público
LaunchSpec e validado pelos planos emitidos. Todos os três restauraram o original
Estado da GUI, removeu o diário e manteve o plugin de produto permanente
fechamento. O
o comportamento da transação com destino ausente é coberto separadamente por
`transaction.absent-overlay-restore`.

