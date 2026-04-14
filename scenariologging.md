# Gallagher API Scenarios with NLog

This document provides BizTalk Expression-shape examples for Scenario 1 (add membership if missing) and Scenario 2 (remove membership if present), with NLog logging enabled.

## Quick Compare: Two Logging Approaches

| Approach | How it works | Best when | Trade-offs | Where to look |
| --- | --- | --- | --- | --- |
| GallagherLoggingHelper wrappers | Call helper methods like GetCardholdersByPdfValueWithNLog that already attach a logger | You want simple expression shapes and consistent logging defaults | Less per-call flexibility unless you add more wrapper methods | Scenario 1 and 2 sections above |
| GallagherApiFacade logger parameter | Call GallagherApiFacade methods directly and pass Action<BizTalkRestLogEntry> as the last parameter | You want per-call control over log formatting, routing, or enrichment | More code in orchestration expressions | Alternative section: Same Scenarios Using GallagherApiFacade Logger Parameter Directly |

## Prerequisites

- Add `NLog.config` to the BizTalk host process folder (for example from `samples/NLog.config.sample`).
- **Deployment note:** BizTalk hosts typically run from `C:\Program Files (x86)\Microsoft BizTalk Server\`. NLog resolves `NLog.config` relative to the executing assembly, so the file must be manually copied to that BizTalk host directory on the target server after deployment, or to whichever folder the BizTalk host process loads the helper assembly from.
- Ensure your helper assembly references NLog and includes `GallagherLoggingHelper`.
- Use the following orchestration string variables:
  - `strGallagherBaseUrl`
  - `strApiKey`
  - `strCertThumbprint`
  - `strPdfFieldId`
  - `strPdfFieldKey`
  - `strCardholderId`
  - `strAccessGroupName`
  - `strAccessGroupHref`
  - `strFromDate`
  - `strUntilDate`
  - `strResponse`
  - `strGallagherCardholderId`
  - `strMembershipHref`

## Extend GallagherLoggingHelper for full workflow

If your `GallagherLoggingHelper` does not already include these wrappers, add them first so all Scenario 1 and 2 calls are logged through NLog.

```csharp
public static string GetCardholdersByPdfValueWithNLog(
    string baseUrl,
    string apiKeyHeaderName,
    string apiKeyHeaderValue,
    string cardholderId,
    string pdfFieldKey,
    string certThumbprint = null,
    int timeoutSeconds = 100)
{
    return GallagherApiFacade.GetCardholdersByPdfValue(
        baseUrl,
        apiKeyHeaderName,
        apiKeyHeaderValue,
        cardholderId,
        pdfFieldKey,
        certThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        timeoutSeconds,
        CreateNLogLogger("GetCardholdersByPdfValue"));
}

public static string ResolveAccessGroupHrefByNameWithNLog(
    string baseUrl,
    string apiKeyHeaderName,
    string apiKeyHeaderValue,
    string accessGroupName,
    string certThumbprint = null,
    int timeoutSeconds = 100)
{
    return GallagherApiFacade.ResolveAccessGroupHrefByName(
        baseUrl,
        apiKeyHeaderName,
        apiKeyHeaderValue,
        accessGroupName,
        certThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        timeoutSeconds,
        CreateNLogLogger("ResolveAccessGroupHrefByName"));
}

public static string ResolveAccessGroupMembershipHrefWithNLog(
    string baseUrl,
    string apiKeyHeaderName,
    string apiKeyHeaderValue,
    string accessGroupHref,
    string cardholderId,
    string certThumbprint = null,
    int timeoutSeconds = 100)
{
    return GallagherApiFacade.ResolveAccessGroupMembershipHref(
        baseUrl,
        apiKeyHeaderName,
        apiKeyHeaderValue,
        accessGroupHref,
        cardholderId,
        certThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        timeoutSeconds,
        CreateNLogLogger("ResolveAccessGroupMembershipHref"));
}

public static string RemoveCardholderFromAccessGroupWithNLog(
    string baseUrl,
    string apiKeyHeaderName,
    string apiKeyHeaderValue,
    string cardholderId,
    string membershipHref,
    string certThumbprint = null,
    int timeoutSeconds = 100)
{
    return GallagherApiFacade.RemoveCardholderFromAccessGroup(
        baseUrl,
        apiKeyHeaderName,
        apiKeyHeaderValue,
        cardholderId,
        membershipHref,
        certThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        timeoutSeconds,
        CreateNLogLogger("RemoveCardholderFromAccessGroup"));
}

public static string GetPersonalDataFieldsByNameWithNLog(
    string baseUrl,
    string apiKeyHeaderName,
    string apiKeyHeaderValue,
    string fieldName,
    string certThumbprint = null,
    int timeoutSeconds = 100)
{
    return GallagherApiFacade.GetPersonalDataFieldsByName(
        baseUrl,
        apiKeyHeaderName,
        apiKeyHeaderValue,
        fieldName,
        certThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        timeoutSeconds,
        CreateNLogLogger("GetPersonalDataFieldsByName"));
}
```

## Scenario 1: Add Cardholder to Access Group with Date Range (with NLog)

### Expression Shape A (Initialize Variables)

```csharp
strGallagherBaseUrl = "https://its-d-cdx-01.adf.bham.ac.uk:8904/api";
strApiKey = "YOUR_API_KEY";
strCertThumbprint = "YOUR_CERT_THUMBPRINT";
strPdfFieldId = "629";
strPdfFieldKey = "pdf_" + strPdfFieldId;
strCardholderId = "IDCARD.2953599";
strAccessGroupName = "6090-MASON-114-02";
strFromDate = "2025-09-12T05:00:00Z";
strUntilDate = "2026-05-20T10:00:00Z";
```

### Expression Shape A.5 (Resolve PDF Field ID by Name with NLog)

Use this shape to look up the PDF field ID dynamically from Gallagher instead of hardcoding it in Shape A.

```csharp
strResponse =
    GallagherLoggingHelper.GetPersonalDataFieldsByNameWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        "ThirdPartyID",
        strCertThumbprint,
        100);
strPdfFieldId = Bham.BizTalk.Rest.GallagherApiResponseParser.GetFirstEntityId(strResponse);
strPdfFieldKey = "pdf_" + strPdfFieldId;
```

### Expression Shape B (Find Cardholder by PDF Value with NLog)

```csharp
strResponse =
    GallagherLoggingHelper.GetCardholdersByPdfValueWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strCardholderId,
        strPdfFieldKey,
        strCertThumbprint,
        100);
```

### Expression Shape C (Extract Gallagher Cardholder ID)

```csharp
strGallagherCardholderId =
    Bham.BizTalk.Rest.GallagherApiResponseParser.GetFirstEntityId(strResponse);
```

### Expression Shape C.5 (Resolve Access Group href by Name with NLog)

```csharp
strAccessGroupHref =
    GallagherLoggingHelper.ResolveAccessGroupHrefByNameWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupName,
        strCertThumbprint,
        100);
```

### Expression Shape D (Check if Membership Already Exists with NLog)

```csharp
strMembershipHref =
    GallagherLoggingHelper.ResolveAccessGroupMembershipHrefWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupHref,
        strGallagherCardholderId,
        strCertThumbprint,
        100);
```

### Decision Shape (Check if Membership Exists)

- Rule: `strMembershipHref != ""`
- True: Skip adding (already exists)
- False: Proceed to add membership

### Expression Shape E (Add Cardholder to Access Group with NLog)

```csharp
strResponse =
    GallagherLoggingHelper.AddAccessGroupToCardholderWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strGallagherCardholderId,
        strAccessGroupHref,
        strFromDate,
        strUntilDate,
        strCertThumbprint,
        100);
```

## Scenario 2: Remove Cardholder from Access Group (with NLog)

### Expression Shape A (Initialize Variables)

```csharp
strGallagherBaseUrl = "https://its-d-cdx-01.adf.bham.ac.uk:8904/api";
strApiKey = "YOUR_API_KEY";
strCertThumbprint = "YOUR_CERT_THUMBPRINT";
strPdfFieldId = "629";
strPdfFieldKey = "pdf_" + strPdfFieldId;
strCardholderId = "IDCARD.2953599";
strAccessGroupName = "6090-MASON-114-02";
```

### Expression Shape A.5 (Resolve PDF Field ID by Name with NLog)

Use this shape to look up the PDF field ID dynamically from Gallagher instead of hardcoding it in Shape A.

```csharp
strResponse =
    GallagherLoggingHelper.GetPersonalDataFieldsByNameWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        "ThirdPartyID",
        strCertThumbprint,
        100);
strPdfFieldId = Bham.BizTalk.Rest.GallagherApiResponseParser.GetFirstEntityId(strResponse);
strPdfFieldKey = "pdf_" + strPdfFieldId;
```

### Expression Shape B (Find Cardholder by PDF Value with NLog)

```csharp
strResponse =
    GallagherLoggingHelper.GetCardholdersByPdfValueWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strCardholderId,
        strPdfFieldKey,
        strCertThumbprint,
        100);
```

### Expression Shape C (Extract Gallagher Cardholder ID)

```csharp
strGallagherCardholderId =
    Bham.BizTalk.Rest.GallagherApiResponseParser.GetFirstEntityId(strResponse);
```

### Expression Shape C.5 (Resolve Access Group href by Name with NLog)

```csharp
strAccessGroupHref =
    GallagherLoggingHelper.ResolveAccessGroupHrefByNameWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupName,
        strCertThumbprint,
        100);
```

### Expression Shape D (Resolve Membership Href with NLog)

```csharp
strMembershipHref =
    GallagherLoggingHelper.ResolveAccessGroupMembershipHrefWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupHref,
        strGallagherCardholderId,
        strCertThumbprint,
        100);
```

### Decision Shape (Check if Membership Exists)

- Rule: `strMembershipHref != ""`
- True: Proceed to remove membership
- False: Skip removing (does not exist)

### Expression Shape E (Remove Cardholder from Access Group with NLog)

```csharp
strResponse =
    GallagherLoggingHelper.RemoveCardholderFromAccessGroupWithNLog(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strGallagherCardholderId,
        strMembershipHref,
        strCertThumbprint,
        100);
```

## Alternative: Same Scenarios Using GallagherApiFacade Logger Parameter Directly

This section shows the same Scenario 1 and 2 flow as `scenario.md`, but instead of wrapper methods it passes the optional last parameter (`Action<BizTalkRestLogEntry> logger`) directly into each facade call.

### Option setup (logger callback)

You can pass an inline callback directly in each call, or create one reusable callback variable.

```csharp
var nlog = NLog.LogManager.GetLogger("Bham.BizTalk.Rest.Gallagher");
Action<Bham.BizTalk.Rest.BizTalkRestLogEntry> logCallback = entry =>
{
    if (entry == null)
    {
        return;
    }

    var level = entry.Level == Bham.BizTalk.Rest.BizTalkRestLogLevel.Error
        ? NLog.LogLevel.Error
        : entry.Level == Bham.BizTalk.Rest.BizTalkRestLogLevel.Warning
            ? NLog.LogLevel.Warn
            : entry.Level == Bham.BizTalk.Rest.BizTalkRestLogLevel.Debug
                ? NLog.LogLevel.Debug
                : NLog.LogLevel.Info;

    var evt = new NLog.LogEventInfo(level, nlog.Name, entry.Message ?? string.Empty)
    {
        Exception = entry.Exception
    };

    evt.Properties["operation"] = entry.Operation ?? string.Empty;
    evt.Properties["url"] = entry.Url ?? string.Empty;
    evt.Properties["statusCode"] = entry.StatusCode.HasValue ? entry.StatusCode.Value : 0;
    nlog.Log(evt);
};
```

### Scenario 1 direct facade calls (add if missing)

Use the same Shapes A, A.5, C and Decision from above. Replace the API call shapes with the direct facade calls below.

#### Expression Shape B (Find Cardholder by PDF Value with logger parameter)

```csharp
strResponse =
    Bham.BizTalk.Rest.GallagherApiFacade.GetCardholdersByPdfValue(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strCardholderId,
        strPdfFieldKey,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100,
        logCallback);
```

#### Expression Shape C.5 (Resolve Access Group href by Name with logger parameter)

```csharp
strAccessGroupHref =
    Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupHrefByName(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupName,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100,
        logCallback);
```

#### Expression Shape D (Resolve Membership Href with logger parameter)

```csharp
strMembershipHref =
    Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupMembershipHref(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupHref,
        strGallagherCardholderId,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100,
        logCallback);
```

#### Expression Shape E (Add Cardholder to Access Group with logger parameter)

```csharp
strResponse =
    Bham.BizTalk.Rest.GallagherApiFacade.AddAccessGroupToCardholder(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strGallagherCardholderId,
        strAccessGroupHref,
        strFromDate,
        strUntilDate,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100,
        logCallback);
```

### Scenario 2 direct facade calls (remove if present)

Use the same Shapes A, A.5, C and Decision from above. Replace the API call shapes with the direct facade calls below.

#### Expression Shape B (Find Cardholder by PDF Value with logger parameter)

```csharp
strResponse =
    Bham.BizTalk.Rest.GallagherApiFacade.GetCardholdersByPdfValue(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strCardholderId,
        strPdfFieldKey,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100,
        logCallback);
```

#### Expression Shape C.5 (Resolve Access Group href by Name with logger parameter)

```csharp
strAccessGroupHref =
    Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupHrefByName(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupName,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100,
        logCallback);
```

#### Expression Shape D (Resolve Membership Href with logger parameter)

```csharp
strMembershipHref =
    Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupMembershipHref(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupHref,
        strGallagherCardholderId,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100,
        logCallback);
```

#### Expression Shape E (Remove Cardholder from Access Group with logger parameter)

```csharp
strResponse =
    Bham.BizTalk.Rest.GallagherApiFacade.RemoveCardholderFromAccessGroup(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strGallagherCardholderId,
        strMembershipHref,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100,
        logCallback);
```

### When to use each logging approach

- Use `GallagherLoggingHelper` wrappers when you want simpler expression shapes and a standard logging format.
- Use direct facade logger parameters when you want per-call control over logging behavior without adding new wrapper methods.
- If your orchestration expression constraints make inline callback creation awkward, keep using wrapper methods and centralize callback creation in helper code.

## Logging Notes

- `ResolveAccessGroupMembershipHref` returns an empty string when no membership exists, and also when access group lookup returns 404.
- `strAccessGroupHref` should contain the full Gallagher href (for example `https://its-d-cdx-01.adf.bham.ac.uk:8904/api/access_groups/659`).
- NLog file target and retention can be configured via `NLog.config`.
