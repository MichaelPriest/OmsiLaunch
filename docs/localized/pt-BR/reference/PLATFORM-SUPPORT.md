> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Suporte de plataforma

Situação: NORMATIVA

## OmsiLaunch atual

O suporte atual oficial é intencionalmente restrito: serviço final/mais recente
Windows 10 x86-64 e Windows 11 x86-64 com suporte atual. O sistema operacional host e
O host atual externo é AMD64/x86-64; OMSI, seu plugin em processo e nativo
A interoperabilidade OMSI permanece x86.

Atual não suporta Windows de 32 bits, ARM64, Vista, Windows 7, Windows 8,
Windows 8.1, Windows XP, Wine, Proton, Linux ou macOS. Plataformas não suportadas
são rejeitados durante a validação antes de qualquer mutação de instalação.

## Dependências de tempo de execução

A distribuição final da corrente deve ser independente sempre que possível. Os usuários fazem
não precisa do Visual Studio, do .NET SDK, do Git, do Python, do Ghidra ou do CMake. Normais
a operação não requer elevação quando a instalação OMSI é gravável por
o usuário atual.

## Famílias Legadas

As famílias legadas são portas históricas/de pesquisa separadas, não o suporte atual.

### Legado NT6

Os alvos pretendidos são Windows Vista SP2 x64 com patch final, Windows 7 SP1 x64,
Windows 8 x64 e Windows 8.1 x64.

### XP Legado

O alvo pretendido é o Windows XP SP3 x86. O Windows XP x64 não é um alvo.

## Posicionamento de segurança

As versões legadas são para sistemas off-line, compatibilidade histórica, controle
benchmarks e pesquisa de reprodutibilidade. Eles não são recomendados para uso normal
uso conectado à internet.

