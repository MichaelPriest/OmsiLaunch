> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Construir perfis

Situação: NORMATIVA

`OmsiBuildProfile` é independente de `RuntimePlatform`. Possui o executável OMSI
impressão digital, globais, formulários, campos, métodos, callsites e protetores de bytes nativos.
Ele nunca codifica a compatibilidade da geração Windows do host.

`Omsi23004_692EBFBF` aceita apenas estes valores LAA SHA-256 conhecidos:

- `692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243`
  (`ALTERNATE_LAA`, validado em tempo de execução);
- `7DAB063D1F62E73B3A2C7A6AC1921D7EDF5E5DB0FBC731481D117EEC8DE7D759`
  (`STEAM_LAA`, reconciliado estaticamente; validação do campo de tempo de execução beta pendente).

Nenhuma string de versão, nome de executável, tamanho de arquivo ou marcador LAA genérico é um
critério de aceitação. Qualquer outro hash é rejeitado antes da preparação do tempo de execução nativo.
O perfil registra `SetActualDateTime`, envio de situação salva e o
primitivas de veículo observadas em evidências estáticas/a montante. Gravar um símbolo é
não é uma autorização para chamá-lo: cada wrapper ABI nativo requer seu próprio
protetor de impressão digital e contrato de chamada comprovado.

O perfil também registra o slot candidato D3D atual `0x008627D0` como
`D3DDispositivo`. Sessão de tempo de execução `3cbd7bb6-d73f-4853-adc8-1213200527d2` comprovada
que o candidato ao slot emprestado suporta `QueryInterface<IDirect3DDevice9>`,
`TestCooperativeLevel` retorna `S_OK` e todas as operações D3D são executadas no
Thread principal da UI/principal/proprietário da renderização do OMSI. OmsiLaunch mantém uma referência QI;
o slot emprestado em si nunca é liberado. Esta entrada não é generalizada para
outra impressão digital executável.

O chamador estático correspondente para `TProgMan.MakeVehicle` faz referência ao candidato
globais em `0x00859DEC`, `0x008591DC` e `0x00858D28`. Sua lista/índice exato
as funções permanecem sem solução, portanto, deliberadamente não são nomeadas BuildProfile
símbolos. Eles não são indicadores públicos e não constituem um documento completo
implementação jogador-veículo.

