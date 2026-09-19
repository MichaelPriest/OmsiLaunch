> Tradução localizada. A documentação canônica em inglês prevalece quando houver divergência técnica.

# Limitações conhecidas do OmsiLaunch Beta 0.1

## Compatibilidade

Beta 0.1 suporta apenas `Omsi23004_692EBFBF`, identificado pelo exato `Omsi.exe` SHA-256 documentado no catálogo de recursos. Executáveis ​​desconhecidos ou sem perfil são rejeitados; O Beta 0.1 não reivindica suporte genérico ao OMSI 2.

## Carregamento mundial e veículos

- `LAST_MAP_STATE` não está implementado. OmsiLaunch nunca substitui a situação salva mais recente.
- O índice de ponto de entrada apresentado é suportado; a identidade semântica do ponto de entrada permanece parcial.
- `MakeVehicle` e `PlaceRandomBus` nativos básicos são validados, mas a colocação/atribuição determinística de PlayerVehicle sem cabeça não é um recurso público suportado.
- Os campos de posição são inspecionáveis. A realocação arbitrária de veículos, a religação espacial entre blocos e a autoridade de transformação segura para ODE não são suportadas.

## Mutação em tempo de execução

- As gravações numéricas de variáveis de script são suportadas somente por meio do caminho semântico com perfil.
- Gravações de variáveis ​​de string, gatilhos nomeados de veículo e gatilhos de som de objeto estão indisponíveis enquanto se aguarda um limite seguro de vida útil da string gerenciada pelo Delphi.
- `SetActualDateTime` e mutação explícita do calendário estão indisponíveis enquanto se aguarda o fechamento da ABI/pós-condição.
- O clima real/ICAO pode ser lido, mas configurar/ativar/atualizar não está disponível.

## Estado avançado

- Alguns campos avançados de mapa/bloco/caminho/spline/gráfico de objeto de construção atual são parciais porque a representação `Kacheln` não está totalmente reconciliada.
- `NoRVNumbers` e representações detalhadas de horários selecionadas permanecem parciais.
- APIs avançadas são instantâneos digitados com controle de perfil, e não um contrato para acesso arbitrário à memória ou uso de ponteiro nativo.

##D3D

Criação, descrição, atualização, liberação de textura, rejeição de identificador obsoleto/liberado, limpeza de saída forçada e relançamento são comprovados em tempo de execução. `DEVICELOST` real, `Reset` OMSI, perdas/redefinições repetidas, corridas de solicitação versus redefinição e provas de eventos públicos perdidas/redefinidas/restauradas permanecem experimentais/parciais.

## Segurança de sessão e configuração

Todas as alterações de configuração e arquivos temporários do OMSI têm escopo de sessão. `options.cfg`, `keyboard.cfg`, `gamectrler.cfg`, sobreposições iniciais e outros artefatos tocados temporariamente são instantâneos/diários/restaurados. O fechamento permanente do plugin OmsiLaunch em `plugins\\OmsiLaunch.*` é instalado com o produto e não é um participante da transação. A edição permanente da configuração está fora do escopo do Beta 0.1.

## Ativos de apresentação

`.omsilaunch` é um diretório de instalação de propriedade do OmsiLaunch, não um arquivo ou um
substituição permanente da configuração do OMSI. Seus recursos iniciais padrão persistem como
ativos de produtos; As substituições da GUI `NewSplashscreen_*.bmp` são apenas para sessão e
restaurado. Um host interrompido sem um diário durável não pode inferir com segurança
se um arquivo GUI restante arbitrário é de propriedade do usuário, então a recuperação é deliberada
não o exclui cegamente.

