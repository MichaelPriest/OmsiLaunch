> Tradução localizada. A documentação canônica em inglês prevalece quando houver divergência técnica.

# `.omsilaunch` Diretório de estado de instalação

`.omsilaunch` é um diretório abaixo da raiz de instalação do OMSI. Nunca é um
Arquivo LaunchSpec e não é implantado em `plugins`.

O Release ZIP é extraído diretamente na raiz do OMSI. `OmsiLaunch.exe` e
seus conjuntos de controladores permanecem nessa raiz, enquanto o plugin OMSI permanente
O encerramento é instalado diretamente em `plugins\\OmsiLaunch.*`. Esses arquivos pertencem
para a instalação do produto: as sessões validam seus hashes, mas nunca preparam,
instantâneo, restaure, remova ou use-os como uma fonte de implantação temporária.

## Criação e propriedade

O diretório é criado preguiçosamente. Uma sessão gerenciada inicial cria
`<OMSI>/.omsilaunch/assets/splash` após adquirir o aluguel de instalação e
antes da transação ser aplicada. Estoque faltante Os ativos do OmsiLaunch são copiados
do diretório `.omsilaunch/assets/splash` empacotado. Os ativos existentes nunca são
substituído, permitindo que um conjunto de ativos de projeto gerenciado explicitamente persista.

O diretório é de propriedade do estado OmsiLaunch. É diferente da configuração OMSI
e da instalação permanente do plugin.

##Layout

| Caminho | Vitalício | Finalidade |
|---|---|---|
| `assets/splash/{PTB,ENG,DEU,FRA}.bmp` | Recurso OmsiLaunch persistente | Ativos splash gerenciados padrão de 640 x 480 de 24 bits. Nenhum número de versão está incorporado. |
| `diagnóstico/<sessão>-host.log` | Diagnóstico persistente | Rastreamento de transição semântica do host. |
| `diagnóstico/*-runtime-*.json` | Diagnóstico persistente | Comando de tempo de execução explícito/evidência de lote. |
| `diário.json` | Estado de transação durável temporário | Instantâneo exato, identidade do processo e progresso da restauração. Removido após restauração normal; retidos apenas para recuperação. |
| `*.omsilaunch.tmp` ao lado de um arquivo OMSI tocado | Efêmero | Preparação de gravação atômica; removido ou substituído atomicamente. |

Somente alterações temporárias do OMSI, como `GUI/NewSplashscreen_*.bmp` e solicitadas
sobreposições de configuração, insira a transação de snapshot/diário/restauração.
Plug-ins de terceiros nunca são enumerados quanto à propriedade, copiados, removidos ou
restaurado.

Transferência de inicialização, telemetria e mapeamentos de comando de tempo de execução são chamados de compartilhados
objetos de memória. Eles são vinculados à sessão/processo e **não** arquivos abaixo deste
diretório.

## Seleção inicial

`SessionPresentationSpec.Splash` possui três modos semânticos:

- `Managed` (padrão): OmsiLaunch instala `ENG` mais o idioma resolvido
  target em `GUI/NewSplashscreen_*.bmp` por meio da transação existente. O
  arquivos OMSI originais são restaurados byte por byte após saída, falha ou
  recuperação.
- `Native` ou `Unset`: preserva os arquivos iniciais do OMSI; nenhuma sobreposição inicial da GUI é
  criado.

A resolução de idioma aceita `PTB`, `ENG`, `DEU` e `FRA`; quando ausente ele usa
`options.cfg` e depois volta para `ENG`. `/splash-assets:<diretório>` ou
`Presentation.CustomAssetDirectory` seleciona um diretório personalizado explícito. Um
o diretório personalizado relativo é resolvido abaixo da raiz de instalação do OMSI. Deve
contém um `ENG.bmp` válido e, para um idioma selecionado que não seja o inglês, sua correspondência
BMP localizado. Os ativos devem ser arquivos BMP de 640 x 480 e 24 bits.

Ativos empacotados/padrão ausentes, formato de recurso inválido ou um personalizado inválido
falha no planejamento/inicialização do diretório com um erro inicial semântico; OMSI não é
lançado. Um diretório `.omsilaunch` ausente é normal e é criado como
descrito acima.

## Validação de lançamento

`tools/Test-ReleasePresentation.ps1` valida o executável Release empacotado
e pode executar os três casos de apresentação contra um OMSI autorizado
instalação. Seu passe padrão não sofre mutação e verifica os ativos do pacote, além de
os três resultados `PlanSession`. `-RunOmsi` também prova o temporário
sobreposição enquanto o OMSI é executado, restauração exata da GUI e limpeza de diário/processo.
Com `-RunOmsi -InstallPackage`, ele copia apenas os arquivos de propriedade do produto do pacote
arquivos raiz e arquivos `plugins/OmsiLaunch.*` na raiz de teste autorizada e, em seguida,
também verifica se os hashes de plug-ins de terceiros permanecem inalterados. O resultado é
persistiu como `diagnóstico/liberação-apresentação-validação.json`.

## Arquivos de configuração

LaunchSpec JSON é fornecido via `/spec:<file.json>`. Um exemplo de distribuição é
`exemplos/.omsilaunch/canonical-session.json`; o diretório aninhado é uma amostra
apenas layout. Ele pode ser copiado no diretório `.omsilaunch` de uma instalação,
mas o arquivo de configuração não é necessário para o funcionamento do diretório.

