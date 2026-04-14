param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [switch]$SkipRestore,

    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'

function Invoke-Step {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [scriptblock]$Action
    )

    Write-Host "==> $Name" -ForegroundColor Cyan
    & $Action

    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE."
    }
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$solutionPath = Join-Path $repoRoot 'Bham.HelperClient.sln'
$testAssemblyPath = Join-Path $repoRoot ("Bham.BizTalk.Rest.Tests\\bin\\{0}\\Bham.BizTalk.Rest.Tests.dll" -f $Configuration)

if (-not (Test-Path $solutionPath)) {
    throw "Solution file not found: $solutionPath"
}

Push-Location $repoRoot
try {
    if (-not $SkipRestore) {
        $nuget = Get-Command nuget -ErrorAction SilentlyContinue
        if ($nuget) {
            Invoke-Step -Name 'NuGet restore' -Action {
                nuget restore Bham.HelperClient.sln
            }
        }
        else {
            Write-Warning 'nuget command not found. Falling back to dotnet restore.'
            Invoke-Step -Name 'dotnet restore' -Action {
                dotnet restore Bham.HelperClient.sln
            }
        }
    }

    if (-not $SkipBuild) {
        $msbuild = Get-Command msbuild -ErrorAction SilentlyContinue
        if ($msbuild) {
            Invoke-Step -Name 'MSBuild' -Action {
                msbuild Bham.HelperClient.sln /p:Configuration=$Configuration /p:Platform="Any CPU" /m
            }
        }
        else {
            Write-Warning 'msbuild command not found. Falling back to dotnet build.'
            Invoke-Step -Name 'dotnet build' -Action {
                dotnet build Bham.HelperClient.sln -c $Configuration
            }
        }
    }

    if (-not (Test-Path $testAssemblyPath)) {
        throw "Test assembly not found: $testAssemblyPath"
    }

    Write-Host '==> Running tests' -ForegroundColor Cyan
    [Reflection.Assembly]::LoadFrom($testAssemblyPath) | Out-Null
    [Bham.BizTalk.Rest.Tests.TestRunner]::RunAll()

    Write-Host "Build and tests completed successfully for configuration $Configuration." -ForegroundColor Green
}
finally {
    Pop-Location
}
