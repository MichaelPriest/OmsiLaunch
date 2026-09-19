# Primeira sessão

Na raiz do OMSI, examine o exemplo Release portátil antes de iniciar o jogo:

```powershell
.\OmsiLaunch.exe /spec:.\.omsilaunch\examples\release-session.example.json /plan /json
```

No exemplo, `RootPath: "."` significa o diretório que contém
`OmsiLaunch.exe`. Remova `/plan` para iniciar a sessão. A splash gerenciada é o
comportamento padrão; `Native` e `Unset` preservam a splash original do OMSI.
