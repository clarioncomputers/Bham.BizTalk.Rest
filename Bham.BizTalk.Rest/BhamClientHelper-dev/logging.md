# Logging Capabilities Summary

## Overview

Logging in this codebase is implemented as an optional callback pipeline rather than a built-in logging framework provider.

## Core Diagnostics Types

Defined in `Bham.BizTalk.Rest/BizTalkRestDiagnostics.cs`:

- `BizTalkRestLogLevel`
  - Values: `Debug`, `Information`, `Warning`, `Error`
- `BizTalkRestLogEntry`
  - `TimestampUtc`
  - `Level`
  - `Operation`
  - `Url`
  - `Message`
  - `StatusCode` (nullable)
  - `Exception` (nullable)
- `BizTalkRestClientException`
  - Carries operation, URL, status code, and response body for failed REST calls

## Logger Hook and Propagation

- `BizTalkRestClientSettings.Logger` is an `Action<BizTalkRestLogEntry>` callback.
- `BizTalkRestClient` forwards this callback into `PatchClient` request execution.
- `GallagherApiFacade` methods expose overloads that can pass a logger through to the underlying client creation path.

## Central Write Behavior

`BizTalkRestLogging.Write(...)` centralizes emission of log events.

Behavior:

- If logger is `null`, logging is skipped.
- Log entry is stamped with UTC timestamp at write time.
- If the logger callback throws, that exception is swallowed to avoid breaking request flow.

## What Is Currently Logged

Primary emissions in `Bham.BizTalk.Rest/PatchClient.cs`:

- `Information`: request start
  - Includes HTTP method, URL, timeout
- `Information`: successful response
  - Includes status code and response length
- `Error`: non-success HTTP response
  - Includes status code and wrapped `BizTalkRestClientException`
- `Error`: timeout (`TaskCanceledException` wrapped as `BizTalkRestClientException`)
- `Error`: pre-response failures (generic wrapped `BizTalkRestClientException`)
- `Debug`: `HttpClient` creation details
  - With certificate: masked thumbprint + cert subject + store location/name
  - Without certificate: explicit no-cert mode

## Console Sink Example

`Bham.BizTalk.Rest.SmokeTest/Program.cs` contains a `WriteLog(BizTalkRestLogEntry entry)` example sink:

- Writes `Error` level entries to stderr
- Writes other levels to stdout
- Prints exception details when present
- Includes timestamp, level, operation, optional status, optional URL, and message

## Current Limits / Notes

- `Warning` exists in the enum but is not currently emitted by the library.
- Logging is synchronous callback invocation only.
- No built-in sink providers (file/event log/structured backend).
- No correlation ID/request ID field in `BizTalkRestLogEntry`.
- Response bodies may appear in exception text for HTTP failures, so downstream sinks should be careful with sensitive content.

## How To Wire A Logger In BizTalk

Use the logger-enabled overloads when you are in regular .NET helper code. For BizTalk Expression shapes, prefer no-logger facade overloads unless you call through a custom helper class.

### Option 1: Regular .NET helper (recommended if you want logs in BizTalk)

Create a small static helper in a separate class library and call it from BizTalk:

```csharp
using System;
using Bham.BizTalk.Rest;

public static class GallagherLoggingHelper
{
  public static string GetCardholdersWithLogging(
    string baseUrl,
    string apiKeyHeaderName,
    string apiKeyHeaderValue,
    string certThumbprint)
  {
    Action<BizTalkRestLogEntry> logger = entry =>
    {
      if (entry == null) return;

      var line = string.Format(
        "[{0:O}] [{1}] [{2}] status={3} url={4} {5}",
        entry.TimestampUtc,
        entry.Level,
        entry.Operation ?? "n/a",
        entry.StatusCode.HasValue ? entry.StatusCode.Value.ToString() : "-",
        string.IsNullOrWhiteSpace(entry.Url) ? "-" : entry.Url,
        entry.Message ?? string.Empty);

      // Replace with your sink (Event Log, file, SIEM, etc.)
      Console.WriteLine(line);
      if (entry.Exception != null)
      {
        Console.WriteLine(entry.Exception.ToString());
      }
    };

    return GallagherApiFacade.GetCardholders(
      baseUrl,
      apiKeyHeaderName,
      apiKeyHeaderValue,
      certThumbprint,
      System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
      System.Security.Cryptography.X509Certificates.StoreName.My,
      100,
      logger);
  }
}
```

### Option 2: Direct BizTalk Expression shape usage (no custom helper)

Use no-logger overloads from `GallagherApiFacade` for maximum binding compatibility:

```csharp
strResponse = Bham.BizTalk.Rest.GallagherApiFacade.GetCardholders(
  strGallagherBaseUrl,
  "Authorization",
  strApiKey,
  strCertThumbprint,
  System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
  System.Security.Cryptography.X509Certificates.StoreName.My,
  100);
```

If you need logs from orchestration calls, route through a custom static helper (Option 1) and keep the orchestration expression itself simple.

Ready-made sample class:

- `samples/GallagherLoggingHelper.cs` contains a copy/paste static helper you can add to your BizTalk helper project.
- The same sample now includes NLog file variants:
  - `GetCardholdersWithNLog(...)`
  - `ResolveGallagherCardholderIdWithNLog(...)`
  - `AddAccessGroupToCardholderWithNLog(...)`
- The same sample now includes log4net file variants:
  - `GetCardholdersWithLog4Net(...)`
  - `ResolveGallagherCardholderIdWithLog4Net(...)`
  - `AddAccessGroupToCardholderWithLog4Net(...)`
- The same sample now includes Event Log variants:
  - `GetCardholdersWithEventLog(...)`
  - `ResolveGallagherCardholderIdWithEventLog(...)`
  - `AddAccessGroupToCardholderWithEventLog(...)`
- `samples/NLog.config.sample` provides a rolling file config template.
- `samples/log4net.config.sample` provides a rolling file config template.

### NLog file logger setup (BizTalk helper project)

1. Add NuGet packages to your BizTalk helper project:

```powershell
Install-Package NLog -Version 4.7.15
Install-Package NLog.Config -Version 4.7.15
```

2. Copy `samples/GallagherLoggingHelper.cs` into your helper project.

3. Copy `samples/NLog.config.sample` as `NLog.config` into your helper project.

4. Ensure `NLog.config` is copied to output:
  - Build Action: `Content`
  - Copy to Output Directory: `Copy if newer`

5. Update file path in config to your server location, for example:
  - `D:/BizTalkLogs/HelperClient/helperclient.${shortdate}.log`

6. Grant BizTalk host service account Modify permission on the log folder.

7. Call one of the `...WithNLog(...)` helper methods from orchestration expressions (via your static helper class).

### log4net file logger setup (BizTalk helper project)

1. Add NuGet package to your BizTalk helper project:

```powershell
Install-Package log4net -Version 2.0.17
```

2. Copy `samples/GallagherLoggingHelper.cs` into your helper project.

3. Copy `samples/log4net.config.sample` as `log4net.config` into your helper project.

4. Ensure `log4net.config` is copied to output:
  - Build Action: `Content`
  - Copy to Output Directory: `Copy if newer`

5. Update file path in config to your server location, for example:
  - `D:/BizTalkLogs/HelperClient/helperclient.log`

6. Grant BizTalk host service account Modify permission on the log folder.

7. Call one of the `...WithLog4Net(...)` helper methods from orchestration expressions (via your static helper class).

8. The helper auto-loads `log4net.config` from the BizTalk host process base directory and falls back to `BasicConfigurator` when config is missing.

### BizTalk Expression shape example (log4net helper)

In an Expression shape, call your copied helper class directly and assign the response to a string variable:

```csharp
strResponse = YourCompany.BizTalk.Helpers.GallagherLoggingHelper.GetCardholdersWithLog4Net(
    strGallagherBaseUrl,
    "Authorization",
    strApiKey,
    strCertThumbprint,
    100);
```

Expected orchestration variables:

- `strGallagherBaseUrl` (System.String)
- `strApiKey` (System.String)
- `strCertThumbprint` (System.String, optional)
- `strResponse` (System.String)

Notes:

- Replace `YourCompany.BizTalk.Helpers` with the namespace where you place `GallagherLoggingHelper`.
- Keep the helper static so BizTalk expressions can call it without instance state.

### BizTalk Expression shape example (resolve Gallagher cardholder id with log4net)

```csharp
strGallagherCardholderId = YourCompany.BizTalk.Helpers.GallagherLoggingHelper.ResolveGallagherCardholderIdWithLog4Net(
  strGallagherBaseUrl,
  "Authorization",
  strApiKey,
  strExternalCardholderId,
  strPdfFieldKey,
  strCertThumbprint,
  100);
```

Expected orchestration variables:

- `strGallagherBaseUrl` (System.String)
- `strApiKey` (System.String)
- `strExternalCardholderId` (System.String)
- `strPdfFieldKey` (System.String, for example `pdf_629`)
- `strCertThumbprint` (System.String, optional)
- `strGallagherCardholderId` (System.String)

### BizTalk Expression shape example (add access group with log4net)

```csharp
strResponse = YourCompany.BizTalk.Helpers.GallagherLoggingHelper.AddAccessGroupToCardholderWithLog4Net(
  strGallagherBaseUrl,
  "Authorization",
  strApiKey,
  strGallagherCardholderId,
  strAccessGroupId,
  strFromDate,
  strUntilDate,
  strCertThumbprint,
  100);
```

Expected orchestration variables:

- `strGallagherBaseUrl` (System.String)
- `strApiKey` (System.String)
- `strGallagherCardholderId` (System.String)
- `strAccessGroupId` (System.String)
- `strFromDate` (System.String, for example `2026-04-01`)
- `strUntilDate` (System.String, for example `2026-05-01`)
- `strCertThumbprint` (System.String, optional)
- `strResponse` (System.String)
- Event Log sink behavior:
  - Writes to source `Bham.BizTalk.Rest` in `Application` log.
  - Attempts source creation if missing.
  - Falls back to `Trace` when source creation/write permissions are unavailable.

## BizTalk Deployment Note (Event Log Source)

To avoid runtime permission problems in BizTalk host instances, pre-create the Event Log source during server provisioning (run once as administrator):

```powershell
$source = "Bham.BizTalk.Rest"
$logName = "Application"

if (-not [System.Diagnostics.EventLog]::SourceExists($source)) {
    [System.Diagnostics.EventLog]::CreateEventSource($source, $logName)
}
```

Verify source registration:

```powershell
[System.Diagnostics.EventLog]::SourceExists("Bham.BizTalk.Rest")
```

Operational notes:

- Event source creation requires elevated/admin privileges.
- Writing entries generally does not require admin once the source exists.
- If source creation is blocked at runtime, the helper falls back to `Trace` output.
