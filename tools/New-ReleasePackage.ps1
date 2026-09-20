[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug',
    [string] $OutputDirectory = ''
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
[xml] $identity = Get-Content -LiteralPath (Join-Path $root 'OmsiLaunch.Version.props') -Raw
$identityProperties = $identity.Project.PropertyGroup | Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) { $OutputDirectory = Join-Path $root 'artifacts\release' }
$stage = Join-Path $OutputDirectory 'OmsiLaunch-current'
$zip = Join-Path $OutputDirectory 'OmsiLaunch-current.zip'
$publicZip = Join-Path $OutputDirectory ('OmsiLaunch-' + $identityProperties.OmsiLaunchProductVersion + '.zip')
$publicChecksum = $publicZip + '.sha256'

Remove-Item -LiteralPath $stage -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath $zip -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath $publicZip, $publicChecksum -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $stage, (Join-Path $stage 'plugins'), (Join-Path $stage '.omsilaunch\assets\splash'), (Join-Path $stage '.omsilaunch\docs'), (Join-Path $stage '.omsilaunch\examples') -Force | Out-Null

function Resolve-ManagedOutput([string] $ProjectName, [string] $Platform, [string] $TargetFramework) {
    $platformPath = Join-Path $root ("artifacts\bin\" + $ProjectName + "\" + $Platform + "\" + $Configuration + "\" + $TargetFramework)
    if (Test-Path -LiteralPath $platformPath) { return $platformPath }

    $legacyPath = Join-Path $root ("artifacts\bin\" + $ProjectName + "\" + $Configuration + "\" + $TargetFramework)
    if (Test-Path -LiteralPath $legacyPath) { return $legacyPath }

    return $platformPath
}

$cli = Resolve-ManagedOutput 'OmsiLaunch.Cli' 'x64' 'net6.0-windows'
$gui = Resolve-ManagedOutput 'OmsiLaunch.Gui' 'x64' 'net6.0-windows'
$guiPublished = Join-Path $root "artifacts\publish\OmsiLaunch.Gui\win-x64"
$bootstrapper = Join-Path $root "artifacts\bin\OmsiLaunch.Bootstrapper\$Configuration\OmsiLaunch.exe"
$netHost = Join-Path $root "artifacts\bin\OmsiLaunch.Bootstrapper\$Configuration\nethost.dll"
$plugin = Join-Path $root "artifacts\bin\OmsiLaunch.Plugin\x86\$Configuration\net6.0-windows"
$native = Join-Path $root "artifacts\x86\$Configuration\OmsiLaunch.Native.x86.dll"

$cliFiles = @(
    'OmsiLaunch.Controller.dll', 'OmsiLaunch.Controller.deps.json', 'OmsiLaunch.Controller.runtimeconfig.json',
    'OmsiLaunch.Api.dll', 'OmsiLaunch.Configuration.dll', 'OmsiLaunch.Content.dll',
    'OmsiLaunch.Core.dll', 'OmsiLaunch.Process.dll', 'OmsiLaunch.Builds.Omsi23004.dll'
)
$guiFiles = @(
    'OmsiLaunch.Launcher.exe'
)

if (-not (Test-Path -LiteralPath $bootstrapper)) { throw "Required controller bootstrapper missing: $bootstrapper" }
Copy-Item -LiteralPath $bootstrapper -Destination (Join-Path $stage 'OmsiLaunch.exe')
if (-not (Test-Path -LiteralPath $netHost)) { throw "Required controller nethost missing: $netHost" }
Copy-Item -LiteralPath $netHost -Destination (Join-Path $stage 'nethost.dll')
$pluginFiles = @(
    'OmsiLaunch.Plugin.opl', 'OmsiLaunch.PluginNE.dll', 'OmsiLaunch.Plugin.dll',
    'OmsiLaunch.Plugin.deps.json', 'OmsiLaunch.Plugin.runtimeconfig.json',
    'OmsiLaunch.Api.dll', 'OmsiLaunch.Builds.Omsi23004.dll', 'OmsiLaunch.Interop.dll'
)

foreach ($file in $cliFiles) {
    $source = Join-Path $cli $file
    if (-not (Test-Path -LiteralPath $source)) { throw "Required CLI artifact missing: $source" }
    Copy-Item -LiteralPath $source -Destination (Join-Path $stage $file)
}
foreach ($file in $guiFiles) {
    $publishedSource = Join-Path $guiPublished $file
    $source = if (Test-Path -LiteralPath $publishedSource) { $publishedSource } else { Join-Path $gui $file }
    if (-not (Test-Path -LiteralPath $source)) { throw "Required GUI artifact missing: $source" }
    Copy-Item -LiteralPath $source -Destination (Join-Path $stage $file)
}
foreach ($file in $pluginFiles) {
    $source = Join-Path $plugin $file
    if (-not (Test-Path -LiteralPath $source)) { throw "Required plugin artifact missing: $source" }
    Copy-Item -LiteralPath $source -Destination (Join-Path $stage "plugins\$file")
}
if (-not (Test-Path -LiteralPath $native)) { throw "Required native artifact missing: $native" }
Copy-Item -LiteralPath $native -Destination (Join-Path $stage 'plugins\OmsiLaunch.Native.x86.dll')
Copy-Item -Path (Join-Path $cli 'assets\splash\*.bmp') -Destination (Join-Path $stage '.omsilaunch\assets\splash')
Copy-Item -LiteralPath (Join-Path $root 'examples\release-session.example.json') -Destination (Join-Path $stage '.omsilaunch\examples\release-session.example.json')
Copy-Item -LiteralPath (Join-Path $root 'LICENSE') -Destination (Join-Path $stage 'LICENSE')
Copy-Item -LiteralPath (Join-Path $root 'THIRD-PARTY-NOTICES.md') -Destination (Join-Path $stage 'THIRD-PARTY-NOTICES.md')

foreach ($document in @(
    @{ Source = 'docs\getting-started\installation.md'; Destination = 'installation.md' },
    @{ Source = 'docs\getting-started\first-session.md'; Destination = 'first-session.md' },
    @{ Source = 'docs\api\local-control.md'; Destination = 'cli-basics.md' },
    @{ Source = 'docs\concepts\omsilaunch-directory.md'; Destination = 'omsilaunch-directory.md' },
    @{ Source = 'docs\reference\known-limitations.md'; Destination = 'troubleshooting-and-limitations.md' }
)) {
    Copy-Item -LiteralPath (Join-Path $root $document.Source) -Destination (Join-Path $stage ('.omsilaunch\docs\' + $document.Destination))
}

foreach ($locale in @('pt-BR', 'de-DE', 'fr-FR', 'pl-PL')) {
    $localizedSource = Join-Path $root ('docs\localized\' + $locale)
    if (Test-Path -LiteralPath $localizedSource) {
        $localizedDestination = Join-Path $stage ('.omsilaunch\docs\localized\' + $locale)
        New-Item -ItemType Directory -Path $localizedDestination -Force | Out-Null
        Copy-Item -Path (Join-Path $localizedSource '*') -Destination $localizedDestination -Recurse -Force
    }
}

$files = Get-ChildItem -LiteralPath $stage -File -Recurse | ForEach-Object {
    [ordered]@{
        path = $_.FullName.Substring($stage.Length + 1).Replace('\', '/')
        bytes = $_.Length
        sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
    }
}
$manifest = [ordered]@{
    product = $identityProperties.OmsiLaunchProductName
    product_version = $identityProperties.OmsiLaunchProductVersion
    package_alias = $identityProperties.OmsiLaunchPackageAlias
    control_protocol = '0.1'
    target_profile = 'Omsi23004_692EBFBF'
    supported_executable_hashes = @(
        '692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243',
        '7DAB063D1F62E73B3A2C7A6AC1921D7EDF5E5DB0FBC731481D117EEC8DE7D759'
    )
    configuration = $Configuration
    generated_utc = [DateTimeOffset]::UtcNow.ToString('O')
    files = $files
}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $stage 'release-manifest.json') -Encoding utf8
Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zip -CompressionLevel Optimal
Copy-Item -LiteralPath $zip -Destination $publicZip
$hash = (Get-FileHash -LiteralPath $publicZip -Algorithm SHA256).Hash
Set-Content -LiteralPath $publicChecksum -Value ($hash + '  ' + [IO.Path]::GetFileName($publicZip)) -Encoding ascii
Write-Output $publicZip
