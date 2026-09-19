> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Veículo do Jogador

Status: IMPLEMENTAÇÃO EM ANDAMENTO

`PlayerVehicleSpec` usa veículo canônico `.bus`, repaint, HOF, número da frota,
e identidades de registro. A descoberta offline resolve essas identidades
sem atribuir índices de lista OMSI persistentes.

O perfil Atual registra as primitivas estáticas necessárias para uso nativo posterior.
criação (`TProgMan.MakeVehicle`, criação/cópia temporária de lista de veículos, aleatória
posicionamento do barramento e posicionamento do barramento). O wrapper da ABI para a sessão ainda não foi
implementado, de modo que cada subcapacidade jogador-veículo solicitada permaneça bloqueada
de forma independente. Uma situação salva não deve ser sobreposta a um novo veículo de jogador
fluxo de criação, a menos que sua semântica nativa o exija explicitamente.

A assinatura de invocação fixada do OmsiHook é uma evidência apenas até que todos os requisitos sejam necessários.
a entrada global/lista/seção crítica é reconciliada com `Omsi23004_692EBFBF`.
A primeira reconciliação direcionada encontrou uma incompatibilidade concreta de perfis: upstream
`MakeVehicle` lê seu `ProgMan` global em `0x00862F28`, enquanto este perfil
a semântica existente `ProgMan` global é `0x00858BDC`. Consequentemente, não há montante
endereço é copiado na ponte atual. O restante trabalho de encerramento consiste em
identificar o proprietário/global atual exato usado pelo caminho de criação nativo e
validar o tempo de vida da lista temporária e o delta da coleção pós-criação.

