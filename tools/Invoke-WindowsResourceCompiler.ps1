param(
    [Parameter(Mandatory = $true)][string] $Source,
    [Parameter(Mandatory = $true)][string] $IncludeDirectory,
    [Parameter(Mandatory = $true)][string] $OutputPath
)

$ErrorActionPreference = 'Stop'

$kitsRoot = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'
if (-not (Test-Path -LiteralPath $kitsRoot)) {
    throw "Windows 10/11 SDK bin directory was not found: $kitsRoot"
}

$compiler = Get-ChildItem -LiteralPath $kitsRoot -Directory |
    Sort-Object {
        try { [version]$_.Name }
        catch { [version]'0.0' }
    } -Descending |
    ForEach-Object { Join-Path $_.FullName 'x86\rc.exe' } |
    Where-Object { Test-Path -LiteralPath $_ } |
    Select-Object -First 1

if ([string]::IsNullOrWhiteSpace($compiler)) {
    throw "No x86 resource compiler (rc.exe) was found under $kitsRoot"
}

New-Item -ItemType Directory -Path (Split-Path -Parent $OutputPath) -Force | Out-Null

& $compiler /nologo /I $IncludeDirectory /fo $OutputPath $Source
if ($LASTEXITCODE -ne 0) {
    throw "Resource compiler failed with exit code $LASTEXITCODE: $compiler"
}
