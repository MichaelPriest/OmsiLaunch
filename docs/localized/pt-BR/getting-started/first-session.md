> Tradução localizada. A documentação canônica em inglês prevalece quando houver divergência técnica.

# Primeira Sessão

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

