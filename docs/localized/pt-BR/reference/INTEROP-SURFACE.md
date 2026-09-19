> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Superfície de interoperabilidade OMSI

Situação: NORMATIVA

`OmsiLaunch.Interop` é o limite OMSI ABI somente x86. Ele reconstrói o
partes reutilizáveis do OmsiHook sem importar o anexo externo ou RPC do OmsiHook
arquitetura no produto OmsiLaunch.

## Regras

- Todos os ponteiros OMSI são endereços explícitos de 32 bits não assinados dentro do Interop.
- Os contratos de API pública nunca expõem ponteiros, identificadores, tipos DNNE ou gerenciados
  Wrappers de objetos OMSI.
- A leitura de strings e arrays Delphi é genérica; escrita ou alocação requer
  um alocador nativo controlado por perfil explícito.
- Um wrapper de domínio se torna executável somente quando o `OmsiBuildProfile` ativo
  fornece o endereço/deslocamento exato mais os protetores de bytes nativos.
- É proibida a substituição da coleção genérica Delphi. Ciclo de vida nativo
  operações possuem mutações nas coleções OMSI.

## Fundação reconstruída

| Capacidade | Estado | Implementação |
| --- | --- | --- |
| Endereços remotos x86 de largura fixa | IMPLEMENTADO | `OmsiRemoteAddress` |
| Leitura/gravação de memória escalar | IMPLEMENTADO | `OmsiMemoryPrimitives` |
| Delphi UnicodeString lida | IMPLEMENTADO | `ReadStringAsync` |
| Leitura de string ANSI Delphi | IMPLEMENTADO | `ReadStringAsync` |
| Matrizes de string/ponteiro/estrutura Delphi | IMPLEMENTADO | `OmsiDelphiValues` |
| Alocação remota de string/matriz | IMPLEMENTADO, é necessário alocador nativo de perfil | `IOmsiRemoteAllocator` |
| Instantâneos de coleção de objetos somente leitura | IMPLEMENTADO | `OmsiObjectCollection<T>` |
| Instantâneos da coleção de ponteiros `TList`/`OList` | IMPLEMENTADO | `OmsiPointerList<T>` com layout fornecido pelo perfil |
| Instantâneos de strings Delphi `TList`/`OList` | IMPLEMENTADO | `OmsiStringList` com layout e codificação fornecidos pelo perfil |
| Acesso ao campo do objeto validado por perfil | IMPLEMENTADO | `OmsiProfiledObject` valida VMT antes de cada acesso ao campo |
| Organização de reflexão estrutural | DIFERIDO | Requer embalagem/layout confirmado e um consumidor concreto |
| Anexação de processo/RPC externo | EXCLUÍDO | OmsiLaunch possui ciclo de vida do processo e transferência de inicialização |

`OmsiRuntimeSurface` lista as operações semânticas herdadas do OmsiHook
domínios úteis. Uma entrada no catálogo não é uma afirmação de que o perfil atual permite
a operação. A disponibilidade é resolvida por `OmsiBuildProfile` e guardas nativos;
`OmsiRuntimeSurface.Resolve` torna essa decisão explícita para um adaptador de perfil.

