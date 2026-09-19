> Tradução localizada. A documentação canônica em inglês prevalece em caso de divergência técnica.

# Teste e validação

Os testes de configuração de sessão requerem restauração de bytes idênticos após sucesso,
falha, parada e recuperação de diário obsoleto. Testes fora do tempo de execução cobrem sem perdas
opções, sinalizadores negativos, vetores, intervalos, blocos compostos, teclado personalizado
eventos, preservação do controlador, integridade de transferência, restauração de transações e
o arrendamento de instalação cross-thread.

`OmsiLaunch.Native.x86.vcxproj` é um v145 de propriedade `Debug|Win32`/`Release|Win32`
projeto e não é construído pela invocação `.sln` gerenciada. Qualquer execução em tempo de execução ou
compilação de pacote que altera `NativeBoundary.cpp` deve construir esse projeto
explicitamente antes da preparação; o manifesto de implantação consome
`artefatos\x86\<Configuração>\OmsiLaunch.Native.x86.dll`.

A validação em tempo de execução é consolidada por matriz de capacidade. O primeiro protegido
a linha de base permanece `NEW_MAP` Grundorf, índice de ponto de entrada 1, Nordspitze Bauernhof,
oito segundos RUNNING, parada solicitada e restauração exata normal.

## Validação da apresentação de lançamento

`tools/Test-ReleaseIdentity.ps1` extrai `OmsiLaunch-current.zip` e audita
cada OmsiLaunch PE distribuído. Ele verifica os campos comuns de produto/versão,
o nome do arquivo interno/original `OmsiLaunch.exe` do controlador e seu incorporado
ícone. `nethost.dll` foi excluído intencionalmente porque é um arquivo não modificado
Dependência de tempo de execução da Microsoft, não um binário OmsiLaunch.

`tools/Test-ReleasePresentation.ps1` valida o pacote Release extraído,
não `artefatos/bin`. Ele verifica se seu manifesto possui configuração `Release`,
contém o fechamento permanente `plugins/OmsiLaunch.*` e quatro pacotes splash
assets em `.omsilaunch/assets/splash` e não tem depuração ou está obsoleto
Caminho `runtime/plugin` no manifesto. Sem
`-RunOmsi` valida os planos gerenciados por padrão, gerenciados de forma personalizada e `Unset`.
Com `-RunOmsi -InstallPackage`, ele instala apenas os pacotes de propriedade do produto
root e `plugins/OmsiLaunch.*` arquivos na raiz autorizada, executa cada caso
por meio do Release CLI instalado, verifica a sobreposição temporária da GUI enquanto o
sessão está ativa, restauração de GUI com bytes exatos, nenhum processo OMSI restante, sem
diário e hashes de plug-ins de terceiros inalterados. Ele escreve suas evidências sob
o diretório de instalação `.omsilaunch/diagnostics`.

A evidência Wave D D3D usa o mesmo caminho público `StartSessionAsync`. Sessão
`3cbd7bb6-d73f-4853-adc8-1213200527d2` aquisição de dispositivo validado, um de propriedade
Referência QI, instalação de gancho de reinicialização, criar/descrever, completo e retangular
atualizações, vários recursos, rejeição determinística de lançamento repetido e
três ciclos de criação/liberação no thread OMSI `16676`. Sessão
`fe1e10fd-cc79-484c-aa04-41fc40fef8da` cobriu a saída forçada do processo com um live
textura; `a95d9fb4-66c6-4f7a-8a64-a18d8e3f9225` provou ser obsoleto na sessão anterior
lidar com a rejeição após o relançamento. Cada execução terminou sem processo OMSI, diário,
arrendamento; `plugins\OmsiLaunch.*` permanece como a instalação permanente do produto.
Os testes de perda/redefinição do dispositivo estão bloqueados
até que um produtor de ciclo de vida nativo seguro esteja disponível; chamando Reset diretamente
do equipamento de validação não é um substituto aceitável.

