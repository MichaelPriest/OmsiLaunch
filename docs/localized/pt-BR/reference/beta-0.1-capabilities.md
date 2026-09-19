> Tradução localizada. A documentação canônica em inglês prevalece quando houver divergência técnica.

# Catálogo de recursos OmsiLaunch Beta 0.1

Versão do produto: `0.1.0`  
Protocolo de controle: `0.1`  
Perfil executável suportado: `Omsi23004_692EBFBF` (`Omsi.exe` SHA-256 `692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243`). O perfil Steam LAA SHA-256 é aceito, mas permanece pendente de validação do campo de tempo de execução Beta.

Significados de status:

- **Compatível**: validado em tempo de execução no perfil compatível e apropriado para a API semântica Beta.
- **Experimental**: implementado e validado onde indicado, mas detalhes específicos do perfil ou o formato da API podem evoluir durante a versão Beta.
- **Parcial**: subconjunto útil está disponível; as restrições listadas são materiais.
- **Ainda não disponível**: ausente intencionalmente do contrato Beta público.
- **Interno**: implementação/teste primitivo, não um compromisso beta público.

| Capacidade | Estado | Estabilidade da API | Validação em tempo de execução | Limitação conhecida |
|---|---|---|---|---|
| Planejamento de sessão, lançamento, status, esperar, parar, fechar | Suportado | Beta estável | Passe | Apenas um perfil OMSI exato. |
| Preparação e restauração de tempo de execução/configuração transacional | Suportado | Beta estável | Passe | Todas as alterações na configuração de inicialização têm escopo de sessão. |
| Lançamento de NEW_MAP por índice de ponto de entrada apresentado | Suportado | Beta estável | Passe | A identidade semântica do ponto de entrada permanece parcial. |
| despacho nativo SAVED_SITUATION | Experimental | Experimental | Passe | O conteúdo salvo testado deve corresponder ao mapa/conteúdo instalado. |
| LAST_MAP_STATE | Ainda não disponível | N/A | Não executado | Nunca inferido do mais recente `.osn`. |
| Descoberta de conteúdo e PlanSession | Suportado | Beta estável | Passe off-line | A descoberta depende do perfil/conteúdo. |
| Tempo de leitura e horário nativo definido | Suportado | Beta estável | Passe | A mutação Calendar/`SetActualDateTime` não está disponível. |
| Leitura do tempo e gravação escalar na lista de permissões | Experimental | Experimental | Passe | O ciclo de vida de configuração/aplicação real/ICAO não está disponível. |
| Leitura do controlador meteorológico real | Experimental | Experimental | Passe | Nenhuma operação pública de ativação/atualização da ICAO. |
| Leitura básica do estado do mapa | Suportado | Beta estável | Passe | Os campos avançados de metadados do mapa/gráfico de blocos são parciais. |
| Leitura da câmera e gravação de FOV limitada | Experimental | Experimental | Passe | A semântica do ciclo de vida/família da câmera permanece restrita ao perfil. |
| Coleção de RoadVehicle e instantâneos de detalhes | Experimental | Experimental | Passe | Nenhuma realocação espacial arbitrária ou religação de ODE. |
| Leitura/índice do PlayerVehicle | Suportado | Beta estável | Passe | O veículo do jogador nulo é válido; a atribuição determinística sem cabeça não está disponível. |
| Humanos e instantâneos de horários | Experimental | Experimental | Passe | Alguns layouts avançados de construção atual permanecem parciais. |
| Variáveis ​​numéricas de script de veículo | Experimental | Experimental | Passe de leitura/gravação | A gravação de variável de string não está disponível. |
| Variável de string, constantes, curvas, HOF, drivers, tickets, logs lidos | Experimental | Experimental | Passe | A mutação de string gerenciada e os gatilhos nomeados não estão disponíveis. |
| `road-vehicles.place-random` | Experimental | Experimental | Passe | Mutação do mundo nativo; o valor de retorno é diagnóstico, não identidade. |
| Primitivo MakeVehicle básico | Interno | Apenas interno | Passe | Não cria/atribui PlayerVehicle. |
| Status D3D e ciclo de vida da textura | Experimental | Experimental | Passe | O ciclo de vida de perda/redefinição do dispositivo é apenas parcialmente comprovado. |
| Eventos de tempo de execução/sessão | Parcial | Experimental | Parcial | Eventos D3D perdidos/redefinidos/restaurados não possuem prova de tempo de execução real. |
| Sobreposições de teclado/controlador/configuração | Suportado | Beta estável | Passe offline/integração | Aplicado apenas para a sessão e restaurado byte por byte. |

Todos os identificadores de tempo de execução são opacos e têm escopo de sessão. Eles não são ponteiros OMSI, não podem ser reutilizados após o encerramento da sessão e podem ser rejeitados como obsoletos ou liberados.

OmsiLaunch não requer binários OmsiHook, seu plugin RPC ou seus arquivos de tempo de execução. OmsiHook continua sendo um oráculo de engenharia com origem LGPL documentada; não é uma dependência de tempo de execução.

