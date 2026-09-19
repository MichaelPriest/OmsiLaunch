> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Controle de tempo de execução

Situação: NORMATIVA

O controle de tempo de execução do OmsiLaunch tem escopo de sessão. Não é OmsiHook RPC
modo de compatibilidade e não é uma arquitetura de anexação de processo externo.

```
API/CLI OmsiLaunch -> sessão ao vivo -> caixa de correio de tempo de execução -> PluginRuntime
    -> Gateway de thread de UI OMSI -> Interop / Native.x86 com perfil -> OMSI
```

A transferência de inicialização permanece apenas para inicialização. Os comandos de tempo de execução usam um separado
caixa de correio versionada vinculada ao GUID da sessão e ao seu processo OMSI iniciado.
Cada solicitação tem um ID de solicitação de largura fixa, carga útil UTF-8, integridade SHA-256
verificação, tempo limite e validação de identidade de resposta. Um mapeamento é criado antes
`CreateProcessW`, é fornecido apenas através do ambiente filho e é
descartado em PluginFinalize, saída de processo, limpeza de sessão ou falha na inicialização.

`RuntimeCommand` são dados semânticos: nome da operação mais chave/valor UTF-8
argumentos. Ele não contém ponteiros OMSI, identificadores Win32, objetos DNNE ou CLR
serialização de objetos. O plugin despacha comandos de seu OMSI existente
Temporizador de thread de UI; Os trabalhadores do IPC nunca chamam métodos Delphi/Borland diretamente.

O controle de tempo de execução tem escopo de sessão. A CLI é uma interface de referência fina sobre
`IOmsiLaunch.ExecuteRuntimeAsync`; ele nunca é anexado de forma independente ou incorporado
Lógica específica do OMSI.

Depois que uma solicitação de inicialização declarativa atinge `RUNNING`, um único comando semântico
pode ser emitido com:

```texto
OmsiLaunch.Cli <instalação> /new /map:maps\Grundorf\global.cfg \
  /entrypoint-index:1 /no-vehicle /runtime:time.read
OmsiLaunch.Cli <instalação> /new /map:maps\Grundorf\global.cfg \
  /entrypoint-index:1 /no-vehicle /runtime:time.set /runtime-arg:minuto=18
```

`/runtime-arg` se repete para campos semânticos adicionais. Cada comando viaja através
a caixa de correio de solicitação/resposta vinculada à sessão, é executada por `PluginRuntime` em
o temporizador UI OMSI e é rejeitado quando a ligação da sessão ou solicitação de identidade
não corresponde. O comando não é um editor de configuração persistente.

Os comandos validados para `Omsi23004_692EBFBF` são `time.read`, `time.set`,
`map.read`, `weather.read`, `weather.set`, `weather.actual.read`, `camera.read`,
`road-vehicles.read`, `road-vehicles.list`, `road-vehicle.read`,
`player-vehicle.read`, `vehicle.variables.list`, `vehicle.variable.get`,
`vehicle.string-variables.list`, `vehicle.string-variable.get`, `humans.read`,
`humans.list`, `human.read`, `timetable.read`, `timetable.tracks.list`,
`timetable.trips.list`, `timetable.lines.list`, `timetable.bus-stops.list`,
`timetable.station-links.list`, `timetable.tours.list` e
`timetable.profiles.list`, `timetable.tour-entries.list`, `d3d.status`,
`d3d.texture.create`, `d3d.texture.describe`, `d3d.texture.update` e
`d3d.texture.release`. Todas as operações de horário são limitadas e somente leitura.
Argumentos para mutações são deliberadamente colocados na lista de permissões pelo adaptador de perfil.

`timetable.track-entries.list` é um instantâneo validado e imutável de
a identidade do caminho semântico de cada entrada de trilha e os metadados relevantes para o tempo. O
O lote canônico de Grundorf retornou 91 registros por meio do canal de sessão.

`timetable.rv-files.list` é um instantâneo limitado validado. Ativo de Grundorf
runtime retornou uma coleção vazia válida. `NoRVNumbers` não é intencionalmente
publicado: a interpretação upstream fixada `raw:true` do candidato
campo entra em conflito com o layout `Omsi23004_692EBFBF` observado, então permanece
perfil não resolvido em vez de expor uma leitura insegura.

`vehicle.variable.set` é validado em tempo de execução por meio do mesmo canal com escopo de perfil.
Ele aceita um `handle` opaco, um `name` de string aberta e um `value` flutuante finito,
resolve o slot de variável pública atual e retorna uma leitura imediata.

Instantâneos de entidades com perfil usam identificadores opacos retornados por `road-vehicles.list`
e `humans.list`, então consumido por `road-vehicle.read` e `human.read` com
`/runtime-arg:handle=<handle>`. Um identificador é válido apenas para sua sessão proprietária
e é rejeitado se seu objeto nativo não estiver mais presente no atual
coleção. Não é um ponteiro, lisíndice t ou identidade persistente.

## Controle Avançado D3D9

A superfície digitada pública em `D3DRuntimeApi` envolve as operações semânticas do D3D.
`D3DTextureHandle` contém uma tag de sessão e um gerador de dispositivo interno
ficha; não é um ponteiro COM nem é válido após liberação, reinicialização, dispositivo
substituição, saída de processo ou outro lançamento de sessão.

O plugin adquire o candidato de perfil exato de `D3DDevice`, retém um
Referência QI e despacha todas as chamadas de textura do OMSI existente
UI/principal/temporizador do proprietário da renderização. As texturas são limitadas e dinâmicas `D3DPOOL_DEFAULT`
recursos. As atualizações validam o nível, o retângulo e a carga útil e, em seguida, usam
`LockRect`/`UnlockRect` com cópias de linha com reconhecimento de pitch. A carga útil da caixa de correio é
limitado a 48 KiB; uploads de textura maiores são expressos como retângulos múltiplos
atualizações.

O observador do ciclo de vida publica `d3d.ready`, `d3d.lost`, `d3d.resetting` e
`d3d.restored` em `SessionStatus.RuntimeEvents`. Redefinir invalida todos ao vivo
recursos do pool padrão antes de invocar o método nativo original e avanços
a geração do dispositivo. Os caminhos de perda de dispositivo e redefinição são implementados, mas não são
comprovado em tempo de execução porque o Wave D não tinha um produtor seguro para aqueles nativos
transições. `d3d.ready`, operações normais de textura, StopSession, saída forçada,
limpeza de relançamento, identificadores liberados e identificadores obsoletos de sessão cruzada são comprovados.

