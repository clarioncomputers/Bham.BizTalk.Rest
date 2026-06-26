param(
    [string]$FromTag,
    [string]$ToTag,
    [string]$OutputDir = "projectchangelogs",
    [string]$OutputName,
    [int]$FallbackCount = 100,
    [switch]$IncludeMerges
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Write-Info {
    param([string]$Message)
    Write-Host "[changelog] $Message"
}

function Test-GitRepo {
    git rev-parse --is-inside-work-tree *> $null
    return $LASTEXITCODE -eq 0
}

if (-not (Test-GitRepo)) {
    throw "Current directory is not a git repository."
}

$resolvedOutputDir = Resolve-Path -Path "." | ForEach-Object { Join-Path $_.Path $OutputDir }
New-Item -ItemType Directory -Force -Path $resolvedOutputDir | Out-Null

$logArgs = @("log")
if (-not $IncludeMerges) {
    $logArgs += "--no-merges"
}
$logArgs += "--pretty=format:- %s (%h)"

$allTags = @(git tag --sort=version:refname)

if ($FromTag -and -not $ToTag) {
    throw "If -FromTag is supplied, -ToTag must also be supplied."
}

$sourceDescription = ""

if ($FromTag -and $ToTag) {
    if (-not ($allTags -contains $FromTag)) {
        throw "Tag '$FromTag' does not exist in this repository."
    }

    if (-not ($allTags -contains $ToTag)) {
        throw "Tag '$ToTag' does not exist in this repository."
    }

    $range = "$FromTag..$ToTag"
    $logArgs += $range
    $sourceDescription = $range

    if (-not $OutputName) {
        $OutputName = "$ToTag.md"
    }
}
elseif ($ToTag -and -not $FromTag) {
    if (-not ($allTags -contains $ToTag)) {
        throw "Tag '$ToTag' does not exist in this repository."
    }

    $tagIndex = [Array]::IndexOf($allTags, $ToTag)
    if ($tagIndex -gt 0) {
        $FromTag = $allTags[$tagIndex - 1]
        $range = "$FromTag..$ToTag"
        $logArgs += $range
        $sourceDescription = $range
    }
    else {
        $logArgs += $ToTag
        $sourceDescription = $ToTag
    }

    if (-not $OutputName) {
        $OutputName = "$ToTag.md"
    }
}
elseif ($allTags.Count -ge 2) {
    $autoFrom = $allTags[-2]
    $autoTo = $allTags[-1]
    $range = "$autoFrom..$autoTo"
    $logArgs += $range
    $sourceDescription = $range

    if (-not $OutputName) {
        $OutputName = "$autoTo.md"
    }

    Write-Info "Using last two tags: $autoFrom -> $autoTo"
}
else {
    $logArgs += "-n"
    $logArgs += $FallbackCount.ToString()
    $sourceDescription = "latest $FallbackCount commits"

    if (-not $OutputName) {
        $OutputName = "unreleased.md"
    }

    Write-Info "No usable tag range found. Falling back to recent commits."
}

$outputPath = Join-Path $resolvedOutputDir $OutputName

$generatedAt = Get-Date -Format "yyyy-MM-dd HH:mm:ss K"
$header = @(
    "# Changelog",
    "",
    "Generated: $generatedAt",
    "Source: $sourceDescription",
    ""
)

$body = & git @logArgs
if ($LASTEXITCODE -ne 0) {
    throw "git log failed while generating changelog."
}

if (-not $body) {
    $body = "- No commits found for this range"
}

@($header + $body) | Set-Content -Path $outputPath -Encoding UTF8

Write-Info "Wrote changelog to: $outputPath"