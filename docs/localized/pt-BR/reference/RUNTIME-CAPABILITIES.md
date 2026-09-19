> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Capacidades de tempo de execução

Status: INVENTÁRIO DE IMPLEMENTAÇÃO

| Capacidade | API/canal | Interoperabilidade | Evidência estática | Evidência de tempo de execução | Estado de liberação |
| --- | --- | --- | --- | --- | --- |
| Solicitação/resposta de sessão | `ExecuteRuntimeAsync` | caixa de correio independente de perfil | testes de protocolo e plugin | lote canônico PASS | RUNTIME_PASS |
| Tempo de leitura | `time.read` | `OmsiTimeAdapter` | fixou OmsiHook mais perfil exato | PASSAR | RUNTIME_PASS |
| Leitura do mapa | `mapa.read` | `OmsiRuntimeReaders` | layout `OmsiMap` fixado | PASSAR; estado carregado e blocos retornados | RUNTIME_PASS |
| Gráfico de mapa/bloco | nenhum | teste de perfil retido internamente | upstream rotula `OmsiMap+0x118` como `Kacheln`, mas a memória ativa atual não expôs um cabeçalho de array Delphi válido | A avaliação `map.tiles.list` falhou com segurança nas sessões `ec85b51a-f69a-4f05-8fb3-638c63f9f20d`, `129482a4-b3a2-448a-830e-4043d11de246` e `90011081-f5e9-4b31-b907-941bb3a9fd1a`; tudo limpo normalmente | BLOCKED_PROFILE_LAYOUT |
| Leitura do tempo | `clima.leitura` | `OmsiRuntimeReaders` | fixou `OmsiWeather` e layouts de registro de clima ativo | PASS: escalares básicos mais temperatura, ponto de orvalho, pressão, precipitação e taxa | RUNTIME_PASS |
| Leitura do tempo real/ICAO | `clima.actual.read` | `OmsiRuntimeReaders` | layout `OmsiActuWeather` fixado | PASSAR | RUNTIME_PASS |
| Leitura da câmera | `câmera.read` | `OmsiRuntimeReaders` | invólucro fixado e evidência da família da câmera | PASSAR | RUNTIME_PASS |
| Mutação FOV da câmera | `câmera.set` | `OmsiCameraWriter` | layout escalar de câmera fixada | PASSAR: `45 -> 46 -> 45` | RUNTIME_PASS |
| Mutação do relógio | `tempo.set` | `OmsiTimeAdapter` + `SetTime` perfilado | campos de relógio upstream e chamada nativa | PASSE: minuto `17 -> 18 -> 17` | RUNTIME_PASS |
| Mutação climática escalar | `clima.set` | Lista de permissões `OmsiWeatherWriter` | campos escalares upstream e layouts de perfil | PASSAR: vento `0 -> 1 -> 0` | RUNTIME_PASS |
| Ponto de entrada semântico apresentado | fechado | correspondência de lista única exata `Tform_setpos` nativa | `FormShow` com perfil, texto de item de lista e fluxo `Button1Click` | Parcial: execução canônica comprovada `índice apresentado 1 -> índice bruto 0 -> Nordspitze Bauernhof`; a representação do texto apresentada permanece sem solução e os rótulos brutos são duplicados | RUNTIME_PARTIAL |
| Inicialização de situação salva | `WorldMode.SavedSituation` | modo/seletor/sequência Button1Click perfilado do formulário inicial | `Tform_start+0x3E4/+0x3F8/+0x434/+0x438/+0x43C`, nativo `SetChecked`, setter de índice de item VMT e `Button1Click` | PASS: `situations\\Baustelle Falkenseer Ch..osn` carregou Berlin-Spandau, alcançou `gameplay.entered`, permaneceu RUNNING por 8 s, então completou a parada solicitada e a restauração normal | RUNTIME_PASS |
| Calendário/mutação real da ICAO | nenhum | Limite de string Delphi/ABI incompleto | referências de método/perfil | não executado | RELEASE_IF_CLOSED |
| Estado de recolha de veículos rodoviários | `road-vehicles.read` | estado `OmsiMyOmsiList` com perfil | contêiner OmsiHook fixado | PASS: contagem/IA/índice de jogador | RUNTIME_PASS |
| Telemetria detalhada de veículos rodoviários | `road-vehicles.list`, `road-vehicle.read`, `player-vehicle.read` | snapshots `MemArrayList` e `OmsiRoadVehicleInst` com perfil | layouts upstream reconciliados com `Omsi23004_692EBFBF` | PASS: alça opaca, posição, rotação, controles, iluminação e campos de IA; solicitação sem veículo retorna `present=false` | RUNTIME_PASS |
| Variável de script do veículo lida | `vehicle.variables.list`, `vehicle.variable.get` | tabela de nomes ANSI perfilada e matriz de ponteiros de valor público | cadeia `OmsiComplMapObjInst.GetVariable` fixada; profile usa valores de instância `+0x23C` | PASS: 1.025 nomes enumerados; `Refresh_Strings=0` | RUNTIME_PASS |
| Mutação de variável de script de veículo | `veículo.variável.set` | tabela de nomes ANSI perfilada e matriz de ponteiros de valor público | cadeia `OmsiComplMapObjInst.SetVariable` fixada; entrada flutuante finita, validação de identificador opaco ao vivo e leitura imediata | PASS: `Refresh_Strings 0 -> 1 -> 0`; limpeza normal PASS | RUNTIME_PASS |
| Leitura de variável de string do veículo | `veículo.string-variables.list`, `vehicle.string-variável.get` | tabela de nomes ANSI perfilada e matriz de valores Unicode | cadeia `OmsiComplMapObjInst.GetStringVariable` fixada | PASS: 27 nomes enumerados; `ident=GRN-V 30` | RUNTIME_PASS |
| Constantes do veículo | `vehicle.constants.list`, `vehicle.constant.get` | bloco `ScriptConstants` com perfil | matrizes de nomes/constantes `OmsiConstBlock` fixadas | PASSAR: `AI_lights_blinkgeberintervall = 0,3`; limpeza normal PASS | RUNTIME_PASS |
| Curvas do veículo | `vehicle.curves.list`, `vehicle.curve.evaluate` | função `OmsiConstBlock` perfilada e matrizes de pontos | fixação de curva fixada/semântica de interpolação linear | PASSAR: `AI_Wandler_last(0) = 1300`; limpeza normal PASS | RUNTIME_PASS |
| Metadados HOF do veículo | `veículo.hofs.read` | matriz HOF com definição de veículo perfilada | nome UTF-16 `OmsiHOF` fixado mais viagem de serviço ANSI | PASS: 11 entradas, incluindo `Grundorf` e `Betriebsfahrt`; limpeza normal PASS | RUNTIME_PASS |
| Motoristas | `drivers.read` | matriz de driver com perfil | campos `OmsiDriver` fixados | PASS: um instantâneo `OMSI-Fan`; limpeza normal PASS | RUNTIME_PASS |
| Pacote de ingressos | `ingressos.read` | pacotes de ingressos/registros de ingressos perfilados | campos `OmsiTicketPack`/`OmsiTicket` fixados | PASS: cinco registros de ingressos `Berlin_1`; limpeza normal PASS | RUNTIME_PASS |
| Registros de horários | `tabelahorária.logs.read` | matriz de log dinâmico com perfil | registro `OmsiTimeTableLog` fixado | PASS: coleção Grundorf vazia válida; limpeza normal PASS | RUNTIME_PASS |
| Coloque ônibus aleatório | `road-vehicles.place-random` | ponte nativa perfilada | ABI `TProgMan.PlaceRandomBus` upstream | PASS: retorno bruto `2`, RoadVehicles `2 -> 4`; limpeza normal PASS | RUNTIME_PASS |
| Gatilhos de veículos/objetos nomeados | nenhum | nenhum | métodos upstream exigem prova de propriedade de string Delphi gerenciada não resolvida | não executado | BLOCKED_STRING_OWNERSHIP |
| Gatilhos sonoros | nenhum | nenhum | método upstream requer prova de propriedade de string Delphi gerenciada não resolvida | não executado | BLOCKED_STRING_OWNERSHIP |
| Contagem de coleta humana | `humanos.read` | Matriz de ponteiros Delphi | fixou OmsiHook global | PASSE: 408 | RUNTIME_PASS |
| Telemetria detalhada humana | `humans.list`, `human.read` | instantâneos `OmsiHumanBeingInst` com perfil | layouts upstream reconciliados com `Omsi23004_692EBFBF` | PASS: alça opaca, movimento, alvo, bilhete, assento, estação e campos AI | RUNTIME_PASS |
| Contagens do gestor de horários | `horário.read` | matrizes perfiladas | fixado `OmsiTimeTableMan` | PASS: trilhas/viagens/paradas/linhas | RUNTIME_PASS |
| Horários Trilhas/Viagens/Linhas | `timetable.tracks.list`, `timetable.trips.list`, `timetable.lines.list` | registros `OmsiTT*Internal` de largura fixa perfilados | registros de horários fixados reconciliados com `Omsi23004_692EBFBF` | PASSE: 3 trilhas, 3 viagens e 2 linhas; `76_BH-Kk` / `76` identidades | RUNTIME_PASS |
| Horários Paradas / EstaçõesLinks | `timetable.bus-stops.list`, `timetable.station-links.list` | registros `OmsiTTBusstopListEntryInternal` e `OmsiTTStnLinkInternal` de largura fixa com perfil | registros de horários fixados reconciliados com `Omsi23004_692EBFBF` | PASSE: 13 paradas de ônibus incluindo `Nordspitze`; 14 ligações de estações | RUNTIME_PASS |
| Horários Passeios | `horário.tours.list` | matrizes `OmsiTTTourInternal` aninhadas com perfil em linhas | registros de horários fixados reconciliados com `Omsi23004_692EBFBF` | PASSE: 3 passeios; linha `0`, tour `1`, grupo AI `Busses`, 72 TourEntries | RUNTIME_PASS |
| Perfis de horários | `timetable.profiles.list` | matrizes `OmsiTTProfileInternal` aninhadas com perfil em Viagens | registros de horários fixados reconciliados com `Omsi23004_692EBFBF` | PASSE: 3 perfis; `padrão`, tempo total 420, 8 tempos de parada | RUNTIME_PASS |
| Horário TourInscrições | `timetable.tour-entries.list` | matrizes `OmsiTTTourEntryInternal` aninhadas com perfil em Tours | registros de horários fixados reconciliados com `Omsi23004_692EBFBF` | PASSE: 130 entradas; `76_BH-Kk`, viagem/perfil 0, 14820 a 15240 | RUNTIME_PASS |
| Horário TrackEntradas | `timetable.track-entries.list` | matrizes `OmsiTTTrackEntryInternal` aninhadas com perfil em trilhas | Tr de largura fixa fixadaregistro ackEntry reconciliado com `Omsi23004_692EBFBF`; limitado a 512 entradas | PASSE: 91 entradas; primeiro ID `99`, bloco `4`, distância `0` em 93 ms | RUNTIME_PASS |
| Arquivos RV de horários | `tabela de horário.rv-files.list` | registros `OmsiRVFileInternal` com perfil | layout de data/linha/lista/probabilidade fixado reconciliado com `Omsi23004_692EBFBF` | PASS: array vazio válido em Grundorf em 62 ms | RUNTIME_PASS |
| Horário NoRVNumbers | nenhum | nenhum | o acesso ao array upstream `raw:true` entra em conflito com o layout `+0x24` observado deste perfil | FAIL: cabeçalho de matriz dinâmica inválido e endereço bruto inválido; limpeza canônica PASS | NÃO RESOLVIDO |
| Mutação de variável de cadeia de veículo | não anunciado | Limite de atribuição/refcount de string Delphi pendente | métodos nativos upstream | não executado | BLOCKED_STRING_OWNERSHIP |
| Status do dispositivo D3D | digitou `D3DRuntimeApi` / `d3d.status` | slot de perfil, QI protegido e observador de nível cooperativo | DXHook fixado mais validação de slot de construção exata | PASS: `S_OK`, READY, referência de propriedade única, gancho de reinicialização instalado | RUNTIME_PASS |
| Criar/descrever textura D3D | digitado opaco `D3DTextureHandle` | registro nativo limitado, textura de pool padrão dinâmico | semântica de textura fixada reconciliada com Atual | PASS: dois formatos, metadados de nível, múltiplos identificadores | RUNTIME_PASS |
| Atualização de textura D3D | digitou `D3DTextureUpdate` | `GetLevelDesc`, limites, `LockRect`, cópia com reconhecimento de pitch, `UnlockRect` | fixou o comportamento do DXHook sem seus defeitos de propriedade | PASS: atualizações completas e retangulares retornaram `S_OK` | RUNTIME_PASS |
| Liberação de textura D3D | operação de liberação digitada | aposentadoria atômica e lançamento COM exatamente uma vez | gráfico de referência própria e modelo de geração | PASS: liberação, rejeição de liberação repetida e três ciclos de reutilização | RUNTIME_PASS |
| Eventos do ciclo de vida D3D | público `RuntimeEvents` (`d3d.ready/lost/resetting/restored`) | Redefinir observador vtable mais fila de transição fixa ordenada | Contrato de redefinição de slot ABI e D3D9 de compilação atual | `d3d.ready` PASSAR; perda/reinicialização/restauração não induzida com segurança | PARTIALMENTE_SUPPORTADO |
| Rejeição de identificador obsoleto D3D | identificador opaco marcado com sessão mais geração de dispositivo | geração nativa e verificações do estado dos recursos | canal de sessão de largura fixa e política de redefinição de pool padrão | PASS para identificadores liberados e de sessões anteriores; redefinir caminho de invalidação não exercido em tempo de execução | PARTIALMENTE_SUPPORTADO |

## Estado da Onda D

OmsiLaunch possui a implementação D3D9. Ele lê o dispositivo específico do perfil
slot, retém exatamente uma referência `QueryInterface<IDirect3DDevice9>`, executa
comandos por meio do temporizador UI/principal/proprietário de renderização existente do OMSI e não expõe
Ponteiro COM. As texturas do pool padrão são invalidadas antes da reinicialização e
as gerações antigas nunca redirecionam um recurso de substituição. Trabalho de textura normal,
StopSession, saída forçada, reinicialização e rejeição de sessão obsoleta são tempo de execução
comprovado. Perda de dispositivo, reinicialização bem-sucedida, perda/redefinição repetida e solicitação versus reinicialização
as corridas permanecem implementadas, mas não comprovadas em tempo de execução porque não há segurança e verdade
o produtor de perda/redefinição estava disponível; nenhuma reinicialização sintética foi invocada.

As operações não anunciadas são rejeitadas; descoberta de capacidade não deve implicar que uma
o endereço bruto é seguro para escrever ou ligar.

