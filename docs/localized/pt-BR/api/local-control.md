> Tradução localizada. A documentação canônica em inglês prevalece quando houver divergência técnica.

# Protocolo de controle local

`OmsiLaunch.exe /serve` possui uma sessão gerenciada e expõe apenas o usuário atual
endpoint de controle de pipe nomeado. As invocações secundárias `OmsiLaunch.exe` atuam como
clientes; eles nunca criam um host concorrente para essa sessão.

Versão do protocolo: `0.1`.

Rotas de cliente suportadas:

- `status da sessão --json`
- `parada de sessão --json`
- `evento lido --json`
- `events watch --json` (até `Ctrl+C`)
- Aliases de tempo de execução, como `time get --json` e `time set --hour=18 --second=10 --json`

Cada solicitação e resposta é JSON com prefixo de comprimento e limitado a 64 KiB. O
endpoint é apenas local e usa `PipeOptions.CurrentUserOnly`. É separado
da caixa de correio host-to-plugin, que permanece vinculada à sessão/processo e não é
uma superfície IPC executável pública.

Quando não existe nenhum host ativo, os clientes retornam `OL_E_NO_ACTIVE_SESSION` e saem
com o código `4`. Uma solicitação do cliente nunca inicia o OMSI implicitamente.

Cada resultado é um envelope com `ok`, `command`, `protocol_version` e
`resultado` ou um `erro` estruturado. O contrato inicial de código de saída estável
é: `0` sucesso, `2` argumentos inválidos, `3` perfil não suportado, `4` não
sessão ativa, `5` tempo de execução indisponível, `6` não encontrado, operação `7`
rejeitado, `8` falha de recuperação e `10` falha interna. Categorias de erro
são semânticos (`invalid_argument`, `unsupported_profile`, `session`,
`runtime`, `not_found`, `transaction` ou `internal`); chamadores não devem analisar
mensagens legíveis por humanos.

Os comandos de tempo de execução recebem IDs de solicitação exclusivos dentro da sessão proprietária. O
O canal público não transporta ponteiros OMSI e não expõe o host ao plug-in
nome da caixa de correio. Todos os identificadores retornados por um comando de tempo de execução permanecem no escopo da sessão
e ficará obsoleto quando o proprietário interromper ou fechar a sessão.

Nomes de operações de tempo de execução desconhecidos são rejeitados localmente como
`OL_E_RUNTIME_OPERATION_UNKNOWN`; a CLI não cria uma sessão ou encaminha
uma operação nativa não reconhecida.

O registro de comando público atualmente ativo está disponível através de:

```PowerShell
Capacidades do OmsiLaunch.exe --json
Tempo de ajuda do OmsiLaunch.exe --json
```

