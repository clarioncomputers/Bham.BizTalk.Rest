param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [switch]$SkipRestore,

    [switch]$SkipBuild,

    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

function Invoke-Step {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [scriptblock]$Action
    )

    Write-Host "==> [NON-BIZTALK] $Name" -ForegroundColor Cyan
    & $Action

    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE."
    }
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$restProject = Join-Path $repoRoot 'Bham.BizTalk.Rest\Bham.BizTalk.Rest.csproj'
$testProject = Join-Path $repoRoot 'Bham.BizTalk.Rest.Tests\Bham.BizTalk.Rest.Tests.csproj'
$smokeProject = Join-Path $repoRoot 'Bham.BizTalk.Rest.SmokeTest\Bham.BizTalk.Rest.SmokeTest.csproj'
$testAssemblyPath = Join-Path $repoRoot ("Bham.BizTalk.Rest.Tests\\bin\\{0}\\Bham.BizTalk.Rest.Tests.dll" -f $Configuration)

foreach ($path in @($restProject, $testProject, $smokeProject)) {
    if (-not (Test-Path $path)) {
        throw "Project file not found: $path"
    }
}

Push-Location $repoRoot
try {
    Write-Host '==> NON-BIZTALK build/test mode: BizTalk .btproj is intentionally excluded.' -ForegroundColor Yellow

    if (-not $SkipRestore) {
        foreach ($project in @($restProject, $testProject, $smokeProject)) {
            Invoke-Step -Name ("dotnet restore {0}" -f [System.IO.Path]::GetFileName($project)) -Action {
                dotnet restore $project
            }
        }
    }

    if (-not $SkipBuild) {
        foreach ($project in @($restProject, $testProject, $smokeProject)) {
            Invoke-Step -Name ("dotnet build {0}" -f [System.IO.Path]::GetFileName($project)) -Action {
                dotnet build $project -c $Configuration --no-restore
            }
        }
    }

    if (-not $SkipTests) {
        if (-not (Test-Path $testAssemblyPath)) {
            throw "Test assembly not found: $testAssemblyPath"
        }

        Write-Host '==> [NON-BIZTALK] Running tests' -ForegroundColor Cyan
        [Reflection.Assembly]::LoadFrom($testAssemblyPath) | Out-Null
        [Bham.BizTalk.Rest.Tests.TestRunner]::RunAll()
    }

    Write-Host "NON-BIZTALK build and tests completed successfully for configuration $Configuration." -ForegroundColor Green
}
finally {
    Pop-Location
}