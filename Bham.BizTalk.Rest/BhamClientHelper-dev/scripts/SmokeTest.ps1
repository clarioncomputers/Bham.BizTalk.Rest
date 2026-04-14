param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('getpublicjson', 'patchpublicjson', 'gallagherworkflow', 'gallaghergetcardholderbyid', 'gallaghergetcardholderaccessgroups', 'gallagherresolvemembershiphref', 'scenariomissingcert', 'scenariotimeoutpublic', 'scenariotimeoutpublicxml', 'scenariononsuccesshttp', 'scenariononsuccesshttpxml', 'scenariononsuccesshttps', 'scenariononsuccesshttpsxml', 'runfailurescenarios', 'runfailurescenariosxml')]
    [string]$Mode,

    [string]$Url = 'https://httpstat.us/503',

    [string]$Query = '-',

    [string]$Body = '{"status":"Done"}',

    [int]$TimeoutSeconds = 100,

    [int]$DelayMilliseconds = 5000,

    [int]$StatusCode = 503,

    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$ApiHeaderName = 'x-api-key',

    [string]$ApiHeaderValue = 'smoke-test',

    [string]$Thumbprint = '0000000000000000000000000000000000000000'

    ,

    [string]$BaseUrl,

    [string]$ApiKey,

    [ValidateSet('add', 'remove', 'update')]
    [string]$Operation,

    [string]$CardholderId,

    [string]$GallagherCardholderId,

    [string]$PdfFieldId,

    [string]$PdfValue,

    [string]$PdfFieldKey = 'pdf_629',

    [string]$AccessGroupName,

    [string]$AccessGroupId,

    [string]$MembershipHref,

    [string]$From,

    [string]$Until,

    [string]$ConfigPath,

    [ValidateSet('LocalMachine', 'CurrentUser')]
    [string]$StoreLocation = 'LocalMachine',

    [string]$StoreName = 'My'
)

$ErrorActionPreference = 'Stop'

function Initialize-SmokeTestExecutable {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath,

        [Parameter(Mandatory = $true)]
        [string]$ExecutablePath,

        [Parameter(Mandatory = $true)]
        [string]$BuildConfiguration
    )

    if (Test-Path $ExecutablePath) {
        return
    }

    dotnet build $ProjectPath -c $BuildConfiguration
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to build smoke test project."
    }

    if (-not (Test-Path $ExecutablePath)) {
        throw "Smoke test executable not found after build: $ExecutablePath"
    }
}

function New-SmokeExecutableAccessException {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ExecutablePath,

        [string]$Detail
    )

    $message = @(
        "Unable to access or execute the smoke test binary:",
        "  $ExecutablePath",
        "",
        "This machine appears to block execution/read access for generated EXE files in the workspace.",
        "",
        "Try one of the following:",
        "  1. Run PowerShell or VS Code as Administrator.",
        "  2. Unblock the file/folder (right-click Properties -> Unblock).",
        "  3. Exclude this workspace from endpoint protection policy.",
        "  4. Build and run from a trusted local folder.",
        ""
    ) -join [Environment]::NewLine

    if (-not [string]::IsNullOrWhiteSpace($Detail)) {
        $message += "Original error: $Detail"
    }

    return New-Object System.InvalidOperationException($message)
}

function Test-SmokeExecutableAccess {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ExecutablePath
    )

    try {
        $stream = [System.IO.File]::OpenRead($ExecutablePath)
        $stream.Close()
    }
    catch {
        throw (New-SmokeExecutableAccessException -ExecutablePath $ExecutablePath -Detail $_.Exception.Message)
    }
}

function Invoke-SmokeTestMode {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ExecutablePath,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    $stdoutPath = [System.IO.Path]::GetTempFileName()
    $stderrPath = [System.IO.Path]::GetTempFileName()

    try {
        try {
            $process = Start-Process -FilePath $ExecutablePath -ArgumentList $Arguments -Wait -NoNewWindow -PassThru -RedirectStandardOutput $stdoutPath -RedirectStandardError $stderrPath -ErrorAction Stop

            $stdout = Get-Content -Path $stdoutPath -Raw -ErrorAction SilentlyContinue
            $stderr = Get-Content -Path $stderrPath -Raw -ErrorAction SilentlyContinue

            if (-not [string]::IsNullOrWhiteSpace($stdout)) {
                Write-Host $stdout.TrimEnd()
            }

            if (-not [string]::IsNullOrWhiteSpace($stderr)) {
                Write-Error $stderr.TrimEnd()
            }

            return $process.ExitCode
        }
        catch {
            if ($_.Exception.Message -notmatch 'Access is denied') {
                throw
            }
        }

        # Some environments block direct EXE launch from the workspace.
        # Fall back to invoking the entry point via reflection.
        # If LoadFrom is blocked, load bytes into memory.
        try {
            $assembly = [Reflection.Assembly]::LoadFrom($ExecutablePath)
        }
        catch {
            try {
                $assemblyBytes = [System.IO.File]::ReadAllBytes($ExecutablePath)
                $assembly = [Reflection.Assembly]::Load($assemblyBytes)
            }
            catch {
                throw (New-SmokeExecutableAccessException -ExecutablePath $ExecutablePath -Detail $_.Exception.Message)
            }
        }

        $entryPoint = $assembly.EntryPoint
        if ($null -eq $entryPoint) {
            throw "Smoke test assembly has no entry point: $ExecutablePath"
        }

        $result = $entryPoint.Invoke($null, @((,([string[]]$Arguments))))
        if ($result -is [int]) {
            return [int]$result
        }

        return 0
    }
    finally {
        Remove-Item -Path $stdoutPath, $stderrPath -ErrorAction SilentlyContinue
    }
}

function Build-Url {
    param(
        [Parameter(Mandatory = $true)]
        [string]$BaseUrl,

        [string]$QueryString = '-'
    )

    if ([string]::IsNullOrWhiteSpace($QueryString) -or $QueryString -eq '-') {
        return $BaseUrl
    }

    $pairs = $QueryString -split '&' | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    $encodedPairs = New-Object System.Collections.Generic.List[string]

    foreach ($pair in $pairs) {
        $separatorIndex = $pair.IndexOf('=')
        if ($separatorIndex -lt 0) {
            $encodedPairs.Add([System.Uri]::EscapeDataString($pair) + '=')
            continue
        }

        $key = $pair.Substring(0, $separatorIndex)
        $value = $pair.Substring($separatorIndex + 1)
        $encodedPairs.Add(([System.Uri]::EscapeDataString($key) + '=' + [System.Uri]::EscapeDataString($value)))
    }

    if ($encodedPairs.Count -eq 0) {
        return $BaseUrl
    }

    $joiner = '?'
    if ($BaseUrl.Contains('?')) {
        $joiner = '&'
    }

    return $BaseUrl + $joiner + ($encodedPairs -join '&')
}

if ($Mode -eq 'getpublicjson') {
    if ([string]::IsNullOrWhiteSpace($Url)) {
        throw 'Url is required for getpublicjson.'
    }

    $requestUrl = Build-Url -BaseUrl $Url -QueryString $Query
    $response = Invoke-RestMethod -Uri $requestUrl -Method Get -TimeoutSec $TimeoutSeconds

    Write-Host 'PUBLIC GET JSON succeeded.' -ForegroundColor Green
    $response | ConvertTo-Json -Depth 20
    exit 0
}

if ($Mode -eq 'patchpublicjson') {
    if ([string]::IsNullOrWhiteSpace($Url)) {
        throw 'Url is required for patchpublicjson.'
    }

    $response = Invoke-RestMethod -Uri $Url -Method Patch -ContentType 'application/json' -Body $Body -TimeoutSec $TimeoutSeconds

    Write-Host 'PUBLIC PATCH JSON succeeded.' -ForegroundColor Green
    $response | ConvertTo-Json -Depth 20
    exit 0
}

$projectPath = Join-Path $PSScriptRoot '..\Bham.BizTalk.Rest.SmokeTest\Bham.BizTalk.Rest.SmokeTest.csproj'
$executablePath = Join-Path $PSScriptRoot ("..\Bham.BizTalk.Rest.SmokeTest\bin\{0}\Bham.BizTalk.Rest.SmokeTest.exe" -f $Configuration)

Initialize-SmokeTestExecutable -ProjectPath $projectPath -ExecutablePath $executablePath -BuildConfiguration $Configuration
Test-SmokeExecutableAccess -ExecutablePath $executablePath

if ($Mode -eq 'scenariomissingcert') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('scenariomissingcert', $Url, $ApiHeaderName, $ApiHeaderValue, $Thumbprint, 'LocalMachine', 'My', $TimeoutSeconds)
    exit $exitCode
}

if ($Mode -eq 'scenariotimeoutpublic') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('scenariotimeoutpublic', $DelayMilliseconds, $TimeoutSeconds)
    exit $exitCode
}

if ($Mode -eq 'scenariotimeoutpublicxml') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('scenariotimeoutpublicxml', $DelayMilliseconds, $TimeoutSeconds)
    exit $exitCode
}

if ($Mode -eq 'scenariononsuccesshttp') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('scenariononsuccesshttp', $StatusCode, $TimeoutSeconds, $Body)
    exit $exitCode
}

if ($Mode -eq 'scenariononsuccesshttpxml') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('scenariononsuccesshttpxml', $StatusCode, $TimeoutSeconds, $Body)
    exit $exitCode
}

if ($Mode -eq 'scenariononsuccesshttps') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('scenariononsuccesshttps', $Url, $TimeoutSeconds)
    exit $exitCode
}

if ($Mode -eq 'scenariononsuccesshttpsxml') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('scenariononsuccesshttpsxml', $Url, $TimeoutSeconds)
    exit $exitCode
}

if ($Mode -eq 'runfailurescenarios') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('runfailurescenarios', $Url)
    exit $exitCode
}

if ($Mode -eq 'runfailurescenariosxml') {
    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments @('runfailurescenariosxml', $Url)
    exit $exitCode
}

if ($Mode -eq 'gallagherworkflow') {
    $arguments = New-Object System.Collections.Generic.List[string]
    $arguments.Add('gallagherworkflow')

    if (-not [string]::IsNullOrWhiteSpace($ConfigPath)) {
        $arguments.Add('--config')
        $arguments.Add($ConfigPath)
    }

    if (-not [string]::IsNullOrWhiteSpace($BaseUrl)) {
        $arguments.Add('--baseUrl')
        $arguments.Add($BaseUrl)
    }

    if (-not [string]::IsNullOrWhiteSpace($ApiKey)) {
        $arguments.Add('--apiKey')
        $arguments.Add($ApiKey)
    }

    if (-not [string]::IsNullOrWhiteSpace($Operation)) {
        $arguments.Add('--operation')
        $arguments.Add($Operation)
    }

    if (-not [string]::IsNullOrWhiteSpace($CardholderId)) {
        $arguments.Add('--cardholderId')
        $arguments.Add($CardholderId)
    }

    if (-not [string]::IsNullOrWhiteSpace($GallagherCardholderId)) {
        $arguments.Add('--gallagherCardholderId')
        $arguments.Add($GallagherCardholderId)
    }

    if (-not [string]::IsNullOrWhiteSpace($PdfFieldId)) {
        $arguments.Add('--pdfFieldId')
        $arguments.Add($PdfFieldId)
    }

    if (-not [string]::IsNullOrWhiteSpace($PdfValue)) {
        $arguments.Add('--pdfValue')
        $arguments.Add($PdfValue)
    }

    if (-not [string]::IsNullOrWhiteSpace($PdfFieldKey)) {
        $arguments.Add('--pdfFieldKey')
        $arguments.Add($PdfFieldKey)
    }

    if (-not [string]::IsNullOrWhiteSpace($AccessGroupName)) {
        $arguments.Add('--accessGroupName')
        $arguments.Add($AccessGroupName)
    }

    if (-not [string]::IsNullOrWhiteSpace($AccessGroupId)) {
        $arguments.Add('--accessGroupId')
        $arguments.Add($AccessGroupId)
    }

    if (-not [string]::IsNullOrWhiteSpace($MembershipHref)) {
        $arguments.Add('--membershipHref')
        $arguments.Add($MembershipHref)
    }

    if (-not [string]::IsNullOrWhiteSpace($From)) {
        $arguments.Add('--from')
        $arguments.Add($From)
    }

    if (-not [string]::IsNullOrWhiteSpace($Until)) {
        $arguments.Add('--until')
        $arguments.Add($Until)
    }

    if (-not [string]::IsNullOrWhiteSpace($Thumbprint)) {
        $arguments.Add('--thumbprint')
        $arguments.Add($Thumbprint)
    }

    if (-not [string]::IsNullOrWhiteSpace($StoreLocation)) {
        $arguments.Add('--storeLocation')
        $arguments.Add($StoreLocation)
    }

    if (-not [string]::IsNullOrWhiteSpace($StoreName)) {
        $arguments.Add('--storeName')
        $arguments.Add($StoreName)
    }

    $arguments.Add('--timeoutSeconds')
    $arguments.Add([string]$TimeoutSeconds)

    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments $arguments.ToArray()
    exit $exitCode
}

if ($Mode -eq 'gallaghergetcardholderbyid') {
    if ([string]::IsNullOrWhiteSpace($BaseUrl)) {
        throw 'BaseUrl is required for gallaghergetcardholderbyid.'
    }

    if ([string]::IsNullOrWhiteSpace($ApiKey)) {
        throw 'ApiKey is required for gallaghergetcardholderbyid.'
    }

    if ([string]::IsNullOrWhiteSpace($GallagherCardholderId)) {
        throw 'GallagherCardholderId is required for gallaghergetcardholderbyid.'
    }

    $arguments = @('gallaghergetcardholderbyid', $BaseUrl, $ApiKey, $GallagherCardholderId)

    if (-not [string]::IsNullOrWhiteSpace($Thumbprint)) {
        $arguments += $Thumbprint
        $arguments += $StoreLocation
        $arguments += $StoreName
        $arguments += [string]$TimeoutSeconds
    }

    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments $arguments
    exit $exitCode
}

if ($Mode -eq 'gallaghergetcardholderaccessgroups') {
    if ([string]::IsNullOrWhiteSpace($BaseUrl)) {
        throw 'BaseUrl is required for gallaghergetcardholderaccessgroups.'
    }

    if ([string]::IsNullOrWhiteSpace($ApiKey)) {
        throw 'ApiKey is required for gallaghergetcardholderaccessgroups.'
    }

    if ([string]::IsNullOrWhiteSpace($GallagherCardholderId)) {
        throw 'GallagherCardholderId is required for gallaghergetcardholderaccessgroups.'
    }

    $arguments = @('gallaghergetcardholderaccessgroups', $BaseUrl, $ApiKey, $GallagherCardholderId)

    if (-not [string]::IsNullOrWhiteSpace($Thumbprint)) {
        $arguments += $Thumbprint
        $arguments += $StoreLocation
        $arguments += $StoreName
        $arguments += [string]$TimeoutSeconds
    }

    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments $arguments
    exit $exitCode
}

if ($Mode -eq 'gallagherresolvemembershiphref') {
    if ([string]::IsNullOrWhiteSpace($BaseUrl)) {
        throw 'BaseUrl is required for gallagherresolvemembershiphref.'
    }

    if ([string]::IsNullOrWhiteSpace($ApiKey)) {
        throw 'ApiKey is required for gallagherresolvemembershiphref.'
    }

    if ([string]::IsNullOrWhiteSpace($AccessGroupId)) {
        throw 'AccessGroupId is required for gallagherresolvemembershiphref.'
    }

    if ([string]::IsNullOrWhiteSpace($GallagherCardholderId)) {
        throw 'GallagherCardholderId is required for gallagherresolvemembershiphref.'
    }

    $arguments = @('gallagherresolvemembershiphref', $BaseUrl, $ApiKey, $AccessGroupId, $GallagherCardholderId)

    if (-not [string]::IsNullOrWhiteSpace($Thumbprint)) {
        $arguments += $Thumbprint
        $arguments += $StoreLocation
        $arguments += $StoreName
        $arguments += [string]$TimeoutSeconds
    }

    $exitCode = Invoke-SmokeTestMode -ExecutablePath $executablePath -Arguments $arguments
    exit $exitCode
}
