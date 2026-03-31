# Gallagher API Scenarios

This document provides code samples for common Gallagher API operations using the Bham.BizTalk.Rest library.

## Scenario 1: Add Cardholder to Access Group with Date Range

**Objective**: Given a cardholder (student ID: `IDCARD.2953599`) and an access group (`6090-MASON-114-02`), add the cardholder to the access group with from date/time `2025-09-12T05:00:00Z` and until date/time `2026-05-20T10:00:00Z`, but only if not already existing.

### Prerequisites

Define orchestration string variables:
- `strGallagherBaseUrl`
- `strApiKey`
- `strCertThumbprint`
- `strPdfFieldId`
- `strPdfFieldKey`
- `strCardholderId`
- `strAccessGroupName`
- `strAccessGroupId`
- `strFromDate`
- `strUntilDate`
- `strResponse`
- `strGallagherCardholderId`
- `strMembershipHref`

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

### Expression Shape B (Find Cardholder by PDF Value)

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
        100);
```

### Expression Shape C (Extract Gallagher Cardholder ID)

```csharp
strGallagherCardholderId =
    Bham.BizTalk.Rest.GallagherApiResponseParser.GetFirstEntityId(strResponse);
```

### Expression Shape C.5 (Resolve Access Group ID by Name)

```csharp
strAccessGroupId =
    Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupIdByName(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupName,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100);
```

### Expression Shape D (Check if Membership Already Exists)

```csharp
strMembershipHref =
    Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupMembershipHref(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupId,
        strGallagherCardholderId,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100);
```

### Decision Shape (Check if Membership Exists)

- **Rule**: `strMembershipHref != ""` (membership exists)
  - **True**: Skip adding (already exists)
  - **False**: Proceed to add membership

### Expression Shape E (Add Cardholder to Access Group - Only if Not Exists)

```csharp
strResponse =
    Bham.BizTalk.Rest.GallagherApiFacade.AddAccessGroupToCardholder(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strGallagherCardholderId,
        strAccessGroupId,
        strFromDate,
        strUntilDate,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100);
```

## Scenario 2: Remove Cardholder from Access Group

**Objective**: Find the cardholder/access group record for student ID `IDCARD.2953599` and access group `6090-MASON-114-02`, then remove it.

### Prerequisites

Use the same variables as Scenario 1, or re-initialize them.

### Expression Shape A (Initialize Variables - if not already done)

```csharp
strGallagherBaseUrl = "https://its-d-cdx-01.adf.bham.ac.uk:8904/api";
strApiKey = "YOUR_API_KEY";
strCertThumbprint = "YOUR_CERT_THUMBPRINT";
strPdfFieldId = "629";
strPdfFieldKey = "pdf_" + strPdfFieldId;
strCardholderId = "IDCARD.2953599";
strAccessGroupName = "6090-MASON-114-02";
```

### Expression Shape B (Find Cardholder by PDF Value)

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
        100);
```

### Expression Shape C (Extract Gallagher Cardholder ID)

```csharp
strGallagherCardholderId =
    Bham.BizTalk.Rest.GallagherApiResponseParser.GetFirstEntityId(strResponse);
```

### Expression Shape C.5 (Resolve Access Group ID by Name)

```csharp
strAccessGroupId =
    Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupIdByName(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupName,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100);
```

### Expression Shape D (Resolve Membership Href)

```csharp
strMembershipHref =
    Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupMembershipHref(
        strGallagherBaseUrl,
        "Authorization",
        strApiKey,
        strAccessGroupId,
        strGallagherCardholderId,
        strCertThumbprint,
        System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
        System.Security.Cryptography.X509Certificates.StoreName.My,
        100);
```

### Decision Shape (Check if Membership Exists)

- **Rule**: `strMembershipHref != ""` (membership exists)
  - **True**: Proceed to remove membership
  - **False**: Skip removing (doesn't exist)

### Expression Shape E (Remove Cardholder from Access Group)

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
        100);
```

## Notes

- Replace `YOUR_API_KEY` and `YOUR_CERT_THUMBPRINT` with actual values.
- The PDF field ID `629` is used as an example - adjust based on your configuration.
- Error handling should be added in production code.
- The `ResolveAccessGroupMembershipHref` method returns an empty string if no membership exists or if the access group does not exist (handles 404 errors internally).
- Date/time values should be in ISO 8601 format with timezone (Z for UTC).</content>
<parameter name="filePath">f:\Projects\BhamClientHelper-main\scenario.md