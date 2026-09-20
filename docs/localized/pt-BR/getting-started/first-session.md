> Tradução localizada. A documentação canônica em inglês prevalece quando houver divergência técnica.

# Primeira Sessão

## Interface gráfica

Para uso normal no Windows, abra `OmsiLaunch.Launcher.exe`.

Quando o pacote estiver instalado na raiz do OMSI, a interface detecta
`Omsi.exe` automaticamente. Em uma instalação standalone/manual em outra
pasta, clique em **Procurar...** uma vez; o caminho escolhido fica salvo nas
preferências do usuário do Windows.

Escolha **Novo mapa** ou **Situação salva**, selecione o conteúdo e use
**Validar** antes de **Iniciar OMSI**. O painel de diagnóstico mostra perfil de
executável incompatível, plugin ausente, falhas de inicialização e os estados da
sessão.

Mantenha o launcher aberto durante a sessão gerenciada. Se tentar fechá-lo com o
OMSI ativo, ele perguntará antes de encerrar a sessão com segurança e restaurar
os arquivos temporários.

## Linha de comando

Na raiz do OMSI, inspecione um exemplo de versão portátil antes de iniciar o OMSI:

```PowerShell
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
```

`RootPath: "."` no exemplo empacotado resolve para o diretório que contém
`OmsiLaunch.exe`. Inicie a mesma sessão omitindo `/plan`.

A apresentação inicial gerenciada é o padrão. OmsiLaunch usa temporariamente seu
Ativo inicial PTB, ENG, DEU ou FRA e restaura os arquivos OMSI originais em
parada normal, falha na inicialização ou recuperação durável. `/splash:Nativo` e
`/splash:Unset` preserva o splash original do OMSI.

