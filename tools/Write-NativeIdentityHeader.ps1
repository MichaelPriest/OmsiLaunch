param(
    [Parameter(Mandatory = $true)][string] $IdentityFile,
    [Parameter(Mandatory = $true)][string] $OutputPath
)

$ErrorActionPreference = 'Stop'
[xml] $identity = Get-Content -LiteralPath $IdentityFile -Raw -Encoding utf8
$properties = $identity.Project.PropertyGroup | Select-Object -First 1
function Escape-CString([string] $value) { $value.Replace('\', '\\').Replace('"', '\"') }

$lines = @(
    '#pragma once',
    ('#define OMSILAUNCH_FILE_VERSION_COMMAS {0}' -f $properties.OmsiLaunchFileVersionCommas),
    ('#define OMSILAUNCH_FILE_VERSION "{0}"' -f (Escape-CString $properties.OmsiLaunchFileVersion)),
    ('#define OMSILAUNCH_PRODUCT_VERSION "{0}"' -f (Escape-CString $properties.OmsiLaunchProductVersion)),
    ('#define OMSILAUNCH_PRODUCT_NAME "{0}"' -f (Escape-CString $properties.OmsiLaunchProductName)),
    ('#define OMSILAUNCH_COMPANY_NAME "{0}"' -f (Escape-CString $properties.OmsiLaunchCompanyName)),
    ('#define OMSILAUNCH_LEGAL_COPYRIGHT "{0}"' -f (Escape-CString $properties.OmsiLaunchLegalCopyright)),
    ('#define OMSILAUNCH_PRODUCT_COMMENTS "{0}"' -f (Escape-CString $properties.OmsiLaunchProductComments))
)
New-Item -ItemType Directory -Path (Split-Path -Parent $OutputPath) -Force | Out-Null
# Resource strings include the product copyright symbol; emit UTF-8 without a BOM
# and let each .rc file opt into code page 65001.
[System.IO.File]::WriteAllLines($OutputPath, $lines, [System.Text.UTF8Encoding]::new($false))
