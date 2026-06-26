param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [string]$OutputDir = "projectchangelogs",
    [string]$Remote = "origin",
    [string]$TagMessage,
    [int]$FallbackCount = 100,
    [switch]$Annotated,
    [switch]$IncludeMerges,
    [switch]$Push,
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Write-Info {
    param([string]$Message)
    Write-Host "[release] $Message"
}

function Test-GitRepo {
    git rev-parse --is-inside-work-tree *> $null
    return $LASTEXITCODE -eq 0
}

if (-not (Test-GitRepo)) {
    throw "Current directory is not a git repository."
}

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$generatorPath = Join-Path $scriptPath "generate-changelog.ps1"
if (-not (Test-Path -Path $generatorPath)) {
    throw "Could not find generate-changelog.ps1 at '$generatorPath'."
}

$tagName = if ($Version.StartsWith("v")) { $Version } else { "v$Version" }
if (-not ($tagName -match '^v\d+\.\d+\.\d+([-.][0-9A-Za-z.-]+)?$')) {
    throw "Version must be semver-like, for example 1.2.3 or v1.2.3."
}

$existingTags = @(git tag --sort=version:refname)
$tagExists = $existingTags -contains $tagName

if ($tagExists -and -not $Force) {
    throw "Tag '$tagName' already exists. Use -Force to recreate it at HEAD."
}

if ($tagExists -and $Force) {
    Write-Info "Recreating existing tag '$tagName' at current HEAD."
}

$createTagArgs = @("tag")
if ($Force) {
    $createTagArgs += "-f"
}
if ($Annotated -or $TagMessage) {
    $createTagArgs += "-a"
    $createTagArgs += $tagName
    $createTagArgs += "-m"
    if ($TagMessage) {
        $createTagArgs += $TagMessage
    }
    else {
        $createTagArgs += "Release $tagName"
    }
}
else {
    $createTagArgs += $tagName
}

& git @createTagArgs
if ($LASTEXITCODE -ne 0) {
    throw "Failed to create tag '$tagName'."
}

Write-Info "Created tag '$tagName'."

if ($Push) {
    if ($Force) {
        & git push $Remote "refs/tags/$tagName" --force
    }
    else {
        & git push $Remote $tagName
    }

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to push tag '$tagName' to '$Remote'."
    }

    Write-Info "Pushed tag '$tagName' to '$Remote'."
}

$updatedTags = @(git tag --sort=version:refname)
$tagIndex = [Array]::IndexOf($updatedTags, $tagName)

$generatorArgs = @{
    ToTag = $tagName
    OutputDir = $OutputDir
    OutputName = "$tagName.md"
    FallbackCount = $FallbackCount
}

if ($IncludeMerges) {
    $generatorArgs["IncludeMerges"] = $true
}

if ($tagIndex -gt 0) {
    $generatorArgs["FromTag"] = $updatedTags[$tagIndex - 1]
}

& $generatorPath @generatorArgs
if ($LASTEXITCODE -ne 0) {
    throw "Changelog generation failed after creating tag '$tagName'."
}

Write-Info "Release changelog generated for '$tagName'."
