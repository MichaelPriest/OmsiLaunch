> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Portabilidade legada

Situação: NORMATIVA
Status de implementação: FUTURO

A portabilidade legada é uma restrição de design, não uma meta de implementação atual.

## Objetivo

Depois que Current estiver estável e antes que Current deixe sua linha de base inicial do .NET,
congelar um `legacy-port-base` funcional para dois backports explícitos: Legacy NT6
e XP legado.

## Princípio

Atual não envelheceu para suportar o Windows antigo. Semântica do LaunchSpec, público
resultados/erros, dados BuildProfile, identidades de conteúdo, semântica de configuração,
protocolo de transferência de inicialização e identidades de operação nativas permanecem portáteis;
as implementações da plataforma variam abaixo.

`OmsiBuildProfile` é independente de `RuntimePlatform`. DNNE é uma corrente
adaptador plugin-host, não um requisito de protocolo.

## Alvos legados

O NT6 legado é direcionado ao Vista SP2 x64, Windows 7 SP1 x64, Windows 8 x64 e
Janelas 8.1 x64. O XP legado é direcionado apenas ao Windows XP SP3 x86. O verdadeiro legado
os conjuntos de ferramentas são selecionados em seus marcos de porta dedicados.

## Compatibilidade de fio

As estruturas de ligação de inicialização/sessão são versionadas; use campos de largura fixa, UTF-8
strings com comprimentos explícitos e regras documentadas de empacotamento/alinhamento/endian.
Eles nunca usam serialização de objetos CLR ou layout dependente do tamanho do ponteiro.

## Reprodutibilidade

Onde a construção/conteúdo do OMSI o suporta, o mesmo LaunchSpec semântico deve ser
reproduzível em implementações de plataformas atuais e legadas para benchmark e
pesquisa de otimização.

## Sequência de Desenvolvimento

1. Complete e estabilize a corrente.
2. Congelar/marcar `legacy-port-base`.
3. Crie `legacy/nt6` e `legacy/xp`.
4. Execute backports explícitos e preserve essas ramificações.
5. Modernize o Current somente depois que essa linha de base existir.

A implementação legada não começa durante a construção atual inicial.

