> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# API pública

Situação: NORMATIVA

`OmsiLaunch.Api` é o limite do produto. Os consumidores constroem uma semântica
`LaunchSpec`, chame `PlanSessionAsync` e, em seguida, chame `StartSessionAsync` apenas para um
plano executável. A CLI é um consumidor de referência desta API e não contém
implementação separada do OMSI.

Os contratos públicos nunca contêm ponteiros OMSI, identificadores Win32, tipos DNNE, CLR
objetos, detalhes de memória compartilhada ou valores dependentes da largura do ponteiro. Conteúdo
identidades são caminhos canônicos relativos ao OMSI. `GetCapabilitiesAsync` e um
`SessionPlan` expõe a disponibilidade de operações específicas do perfil de construção.

`StartSessionAsync` retorna após propriedade da sessão, teste transacional e
supervisão do processo é estabelecida. Não promete jogabilidade. Consumidores
espere por `SessionState.Running`; para solicitações de inicialização sem cabeça, isso significa que o
host observou `gameplay.entered` no tempo de execução do processo.

`World.EntrypointIdentity` permanece limitado por capacidade até que seja estruturado
o mapeamento do ponto de entrada para a lista apresentada é fechado para o perfil ativo. O
o adaptador nativo já rejeita rótulos apresentados ausentes ou ambíguos, mas os chamadores
entretanto, use `PresentedEntrypointIndex` como o seletor de baixo nível suportado.

`DiscoverAsync` é somente leitura. A descoberta de complementos é apenas de inventário: OmsiLaunch
não ativa, desativa, modifica direitos ou altera complementos do Steam.

Toda configuração em um `LaunchSpec` é um estado de sessão temporário. A API pública
não expõe edição permanente de configuração: cada configuração tocada
o arquivo é capturado em instantâneo da transação, registrado em diário, verificado e restaurado byte por byte.

## Apresentação da Sessão

`SessionPresentationSpec.Splash` é `Managed` por padrão. Ele seleciona o
Splash padrão do OmsiLaunch em `<installation>/.omsilaunch/assets/splash` e
sobrepõe temporariamente os arquivos iniciais da GUI do OMSI. `CustomAssetDirectory` seleciona um
diretório de ativos de sessão/projeto; caminhos relativos são resolvidos abaixo da instalação.
`Unset` (e o alias de compatibilidade `Native`) preserva explicitamente o nativo
OMSI splash e não adiciona nenhuma mutação inicial da GUI. Esta política de apresentação permanece
com escopo de sessão; `.omsilaunch` é o estado do produto, não uma edição persistente do OMSI
configuração.

## API avançada D3D9

`D3DRuntimeApi` fornece extensões de sessão digitadas para status e textura do dispositivo
criar, descrever, atualizar e lançar. Os modelos públicos são `D3DDeviceStatus`,
`D3DTextureHandle`, `D3DTextureDescription`, `D3DTextureUpdate`,
`D3DDeviceState`, `D3DTextureFormat` e `D3DTextureResourceState`.

Os identificadores são valores opacos com escopo de sessão. Os consumidores não podem obter um COM
ponteiro, endereço OMSI ou identificador Win32. Lançado, geração obsoleta e
os valores de sessão cruzada falham com códigos `OmsiRuntimeException` estruturados. Público
o status da sessão também expõe a coleção `RuntimeEvents` ordenada e limitada,
que transporta eventos do ciclo de vida D3D com carimbo de data/hora, sequência e dados semânticos.

Os métodos digitados são wrappers no mesmo canal de comando de tempo de execução público:

```csharp
var status = aguarda lançamento.GetD3DStatusAsync(sessão, TimeSpan.FromSeconds(5));
var textura = aguarda lançamento.CreateD3DTextureAsync(
    sessão, 64, 64, D3DTextureFormat.A8R8G8B8);
aguarde lançamento.UpdateD3DTextureAsync(sessão, textura.Handle,
    novo D3DTextureUpdate(0, 0, 0, 64, 16, pixels));
aguarde lançamento.ReleaseD3DTextureAsync(sessão, textura.Handle);
```

As chamadas são válidas apenas enquanto a sessão proprietária for `RUNNING` e a sessão correspondente
BuildProfile está ativo. A redefinição não recria nem redireciona um identificador.

