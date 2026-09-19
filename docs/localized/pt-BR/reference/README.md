> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

<p alinhar="centro">
  <img src="assets/branding/omsilaunch-logo.png" alt="OmsiLaunch" width="620">
</p>

<p align="center"><strong>Controle de sessão para OMSI 2.</strong></p>
<p align="center">Código aberto · Programável · Orientado pela comunidade</p>

---

#OmsiLaunch

**OmsiLaunch** é um lançamento programável de código aberto, gerenciamento de sessão,
e camada de controle de tempo de execução para OMSI 2. Ele fornece uma API pública, um funcional
CLI, um plugin/tempo de execução OMSI em processo e um `BuildProfile` de construção exata
limite. É infraestrutura para lançadores, ferramentas, automação e comunidade
integrações em vez de um iniciador gráfico.

> **Defina a sessão, não os cliques.**

## Beta 0.1

A primeira versão beta pública é **0.1.0-beta1**. Suporta o perfil OMSI exato
`Omsi23004_692EBFBF`, valida a impressão digital executável antes do lançamento e
não reivindica compatibilidade com compilações desconhecidas do OMSI. O pacote e detalhado
o status de compatibilidade está documentado em [`docs/`](../README.md).

OmsiLaunch suporta planejamento de sessão semântica com `LaunchSpec` e
`PlanSession`, inicialização através de `StartSession`, controle de tempo de execução observado e
limpeza normal `StopSession`/`CloseSession`. Os recursos de tempo de execução suportados são
catalogado explicitamente; capacidades experimentais e indisponíveis não ficam ocultas.

## Propriedade de sessão segura

Todas as substituições de configuração de inicialização têm escopo de sessão. Instantâneos do OmsiLaunch,
registra, aplica, verifica e restaura cada configuração temporária ou GUI
arquivo ele muda. Não oferece edição permanente de configuração nesta versão beta.

Os arquivos de plug-in do produto são instalados permanentemente em `plugins\\OmsiLaunch.*`.
Eles não são copiados e removidos em todas as sessões e plug-ins de terceiros
nunca são propriedade transacional. `.omsilaunch\\` é um OmsiLaunch-privado
diretório para ativos, diagnósticos, diários e exemplos de usuários.

A apresentação inicial gerenciada é o padrão. Produto PTB, ENG, DEU e FRA
os ativos são temporariamente sobrepostos e restaurados com exatidão. Splash nativo/`Não definido`
modo preserva arquivos OMSI.

## Baixar

Baixe **`OmsiLaunch-0.1.0-beta1.zip`** da página de lançamento do projeto e
extraia-o diretamente na raiz OMSI suportada. O pacote inclui o
controlador, suas dependências necessárias, o fechamento permanente do plugin, splash
ativos, um exemplo de sessão de lançamento e um pequeno guia do usuário off-line.

Consulte [Instalação](../README.md) e
[Primeira Sessão](../README.md). Usar
`OmsiLaunch.exe /version` para inspecionar o controlador instalado.

## Para desenvolvedores

A superfície de produto preferida é a API pública semântica. A CLI é uma
frontend de referência sobre a mesma API; ele não contém lógica OMSI separada.
O controle de tempo de execução tem escopo de sessão, validação de perfil e usa semântica opaca
manipula em vez de ponteiros nativos públicos.

- [API pública](../README.md)
- [Controle de tempo de execução](../README.md)
- [Catálogo de recursos](../README.md)
- [Limitações conhecidas](../README.md)
- [Política de criação de perfil](../README.md)

## Comunidade e licença

OmsiLaunch é um projeto de código aberto voltado para a comunidade. É independente de
outros lançadores OMSI e podem ser consumidos por ferramentas comunitárias compatíveis.

OmsiLaunch está licenciado sob [somente LGPL-3.0](../README.md). Veja o
[avisos de terceiros](../README.md) para fonte incorporada
proveniência e avisos aplicáveis.

