# Bham.HelperClient

[![CI Pipeline](https://dev.azure.com/YOUR_ORG/YOUR_PROJECT/_apis/build/status/YOUR_PIPELINE_NAME?branchName=dev)](https://dev.azure.com/YOUR_ORG/YOUR_PROJECT/_build/latest?definitionId=YOUR_DEFINITION_ID&branchName=refs/heads/dev)

BizTalk 2016 helper library for outbound REST GET and PATCH calls, with Gallagher-specific wrappers.

## 1. What this library gives a BizTalk developer

- Static HTTP helper methods for GET and PATCH.
- Optional client certificate (thumbprint lookup in Windows certificate store).
- API key header support.
- Typed Gallagher wrapper logic for cardholders and access groups.
- BizTalk-safe static Gallagher facade so you can use wrapper logic without holding non-serializable objects in orchestration state.
- JSON parsing helpers for common Gallagher response extraction.

Target framework:
- .NET Framework 4.6 (net46)

Main output:
- Bham.BizTalk.Rest.dll

## Azure DevOps CI/CD

This repository now includes Azure DevOps YAML at azure-pipelines.yml.

Badge setup:
- Replace `YOUR_ORG`, `YOUR_PROJECT`, `YOUR_PIPELINE_NAME`, and `YOUR_DEFINITION_ID` in the badge URL above.
- Keep `branchName=dev` if `dev` is your active integration branch.

Default pipeline behavior:
- Runs on a Microsoft-hosted Windows agent.
- Executes scripts/build-and-test-non-biztalk.ps1 in Release mode.
- Validates restore, build, and tests for the non-BizTalk projects.

Optional BizTalk stage:
- Stage name: BizTalk.
- Disabled by default with variable RunBizTalkBuild=false.
- Requires a self-hosted Windows agent pool with BizTalk Developer Tools installed.
- Uses variable BizTalkPoolName to select that pool.

Recommended usage:
1. Create a new Azure DevOps pipeline from this repo and select Existing Azure Pipelines YAML file.
2. Choose azure-pipelines.yml at repository root.
3. Run as-is to validate non-BizTalk CI.
4. When your self-hosted BizTalk agent is ready, set RunBizTalkBuild=true and BizTalkPoolName to your pool name.

Suggested governance:
1. Set the pipeline default branch to `dev`.
2. Add branch policy so pull requests require this pipeline to pass.
3. Add a nightly schedule to detect toolchain or dependency drift.

## 2. Quick start for BizTalk (recommended path)

Use the static facade class in orchestration Expression shapes:
- Bham.BizTalk.Rest.GallagherApiFacade

This avoids the BizTalk serialization problem you get if you declare GallagherApiClient as an orchestration variable.

Postman alignment:
- The wrapper/facade now includes alias methods whose names read closer to the imported Postman requests.
- Existing methods are still available.
- Use whichever naming is clearer for your team.

### Minimal orchestration setup

Terminology used in this README:
- `pdfFieldId` = Gallagher Personal Data Field numeric id, for example `629`
- `pdfFieldKey` = Gallagher query key built from the field id, for example `pdf_629`
- `cardholderId` = the value stored in that field and used for lookup, for example `IDCARD.12345`
- `gallagherCardholderId` = Gallagher's own cardholder record id, for example `653`

Define orchestration string variables (example):
- strGallagherBaseUrl
- strApiKey
- strCertThumbprint
- strPdfFieldId
- strPdfFieldKey
- strCardholderId
- strResponse
- strGallagherCardholderId
- strAccessGroupId
- strMembershipHref

Expression shape A (initialize once):

    strGallagherBaseUrl = "https://its-d-cdx-01.adf.bham.ac.uk:8904/api";
    strApiKey = "YOUR_API_KEY";
    strCertThumbprint = "YOUR_CERT_THUMBPRINT";
    strPdfFieldId = "629";
    strPdfFieldKey = "pdf_" + strPdfFieldId;
    strCardholderId = "IDCARD.12345";

Expression shape B (find cardholder by PDF value via facade):

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

Expression shape C (resolve Gallagher numeric cardholder id from the JSON response):

    strGallagherCardholderId =
        Bham.BizTalk.Rest.GallagherApiResponseParser.GetFirstEntityId(strResponse);

Expression shape D (resolve membership href via facade):

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

## 3. Which API surface to use

Use this decision guide:

1. Use GallagherApiFacade when:
- You are in BizTalk orchestration code.
- You want Gallagher-specific methods (cardholders, access groups, membership href resolution).
- You want to avoid non-serializable wrapper instances.

2. Use PatchClient when:
- You need a raw URL call and do not need Gallagher wrapper behavior.
- You want the simplest direct static GET/PATCH calls.

3. Use GallagherApiClient directly only when:
- You are in regular .NET code (or inside BizTalk Atomic scope).
- You are comfortable creating wrapper instances.

## 4. BizTalk serialization rule (important)

BizTalk persists orchestration state. Non-serializable CLR object variables cannot be stored in normal orchestration scope.

This will fail in normal scope:
- Bham.BizTalk.Rest.GallagherApiClient
- Bham.BizTalk.Rest.BizTalkRestClientSettings

Your options:

1. Preferred for orchestrations: use static GallagherApiFacade methods.
2. Alternative: use GallagherApiClient only inside an Atomic Scope.
3. Alternative: wrap logic in external static helper methods and call those.

## 5. Certificate thumbprint behavior

- certThumbprint is optional.
- If provided, the certificate is looked up and attached from StoreLocation + StoreName.
- If omitted/null, request is sent without client certificate.

BizTalk-friendly overloads:
- PatchClient.GetJsonWithClientCertAndApiKeyDefaultStore
- PatchClient.GetXmlWithClientCertAndApiKeyDefaultStore
- PatchClient.PatchJsonWithClientCertAndApiKeyDefaultStore
- PatchClient.PatchXmlWithClientCertAndApiKeyDefaultStore

No-certificate overloads:
- PatchClient.GetJsonWithApiKey
- PatchClient.GetXmlWithApiKey
- PatchClient.PatchJsonWithApiKey
- PatchClient.PatchXmlWithApiKey

## 6. Gallagher facade methods

Important naming note:
- Methods such as `GetCardholdersByPdfValue(...)` still keep the original method name for backward compatibility.
- The lookup argument now represents the external `cardholderId` value stored in the Personal Data Field, for example `IDCARD.12345`.
- That is different from the Gallagher numeric cardholder id, for example `653`.

Common methods on GallagherApiFacade:

Read/query:
- GetPersonalDataFields
- GetPersonalDataFieldsByName
- GetPersonalDataFieldById
- GetCardholders
- GetCardholdersByPdfValue
- GetCardholderById
- GetAccessGroups
- GetAccessGroupById
- FindAccessGroupsByName
- GetAccessGroupCardholders
- GetCardholderAccessGroups

Resolve IDs/hrefs:
- ResolvePersonalDataFieldId
- ResolveCardholderIdByPdfValue
- ResolveAccessGroupIdByName
- ResolveAccessGroupMembershipHref

Update membership:
- AddAccessGroupToCardholder
- RemoveAccessGroupFromCardholder
- UpdateAccessGroupForCardholder

Postman-aligned aliases:
- ResolvePdfFieldId
- ResolveGallagherCardholderId
- SearchAccessGroupsByName
- RemoveCardholderFromAccessGroup
- UpdateCardholderAccessGroup

### Postman request to method mapping

Imported Postman request names map to these facade methods:

1. `PDF ID`
- `GallagherApiFacade.ResolvePdfFieldId(...)`
- or `GallagherApiFacade.ResolvePersonalDataFieldId(...)`

2. `Cardholder ID`
- `GallagherApiFacade.ResolveGallagherCardholderId(...)`
- or `GallagherApiFacade.ResolveCardholderIdByPdfValue(...)`

3. `Access Group ID`
- `GallagherApiFacade.GetAccessGroups(...)`

4. `Access Group ID Search`
- `GallagherApiFacade.SearchAccessGroupsByName(...)`
- or `GallagherApiFacade.FindAccessGroupsByName(...)`

5. `Access Group Cardholders`
- `GallagherApiFacade.GetAccessGroupCardholders(...)`

6. `Add Access Group to Cardholder`
- `GallagherApiFacade.AddAccessGroupToCardholder(...)`

7. `Remove Cardholder from Access Group`
- `GallagherApiFacade.RemoveCardholderFromAccessGroup(...)`
- or `GallagherApiFacade.RemoveAccessGroupFromCardholder(...)`

8. `Update Cardholder Access Group`
- `GallagherApiFacade.UpdateCardholderAccessGroup(...)`
- or `GallagherApiFacade.UpdateAccessGroupForCardholder(...)`

### End-to-end BizTalk examples for each common Gallagher step

These examples assume the following orchestration string variables already exist:
- strGallagherBaseUrl
- strApiKey
- strCertThumbprint
- strPdfFieldId
- strPdfFieldKey
- strCardholderId
- strGallagherCardholderId
- strAccessGroupName
- strAccessGroupId
- strMembershipHref
- strResponse
- strFromDate
- strUntilDate
- strFromUtc
- strUntilUtc

Example setup:

    strGallagherBaseUrl = "https://its-d-cdx-01.adf.bham.ac.uk:8904/api";
    strApiKey = "YOUR_API_KEY";
    strCertThumbprint = "YOUR_CERT_THUMBPRINT";
    strPdfFieldId = "629";
    strPdfFieldKey = "pdf_" + strPdfFieldId;
    strCardholderId = "IDCARD.12345";
    strAccessGroupName = "6040-CHAMBERLAIN-B-11703";
    strFromDate = "2026-04-01";
    strUntilDate = "2026-05-01";
    strFromUtc = "2026-04-01T00:00:00Z";
    strUntilUtc = "2026-04-30T12:00:00Z";

#### 1. Get PDF field id by field name

If you do not already know the Personal Data Field id for the environment, resolve it first.

    strPdfFieldId =
        Bham.BizTalk.Rest.GallagherApiFacade.ResolvePersonalDataFieldId(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            "ThirdPartyID",
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

    strPdfFieldKey = "pdf_" + strPdfFieldId;

#### 2. Get cardholder JSON by external cardholder id

This performs the query:
- GET /api/cardholders?pdf_629="IDCARD.12345"

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

#### 3. Resolve Gallagher numeric cardholder id from the JSON

    strGallagherCardholderId =
        Bham.BizTalk.Rest.GallagherApiResponseParser.GetFirstEntityId(strResponse);

#### 4. Resolve access group id by name

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

#### 5. Add an access group to a cardholder

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

#### 6. Get membership href for an existing access-group assignment

You need the membership href before update or remove.

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

#### 7. Update an existing access-group assignment

Use UTC timestamps for update operations.

    strResponse =
        Bham.BizTalk.Rest.GallagherApiFacade.UpdateAccessGroupForCardholder(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            strGallagherCardholderId,
            strMembershipHref,
            strFromUtc,
            strUntilUtc,
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

#### 8. Remove an access-group assignment

    strResponse =
        Bham.BizTalk.Rest.GallagherApiFacade.RemoveAccessGroupFromCardholder(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            strGallagherCardholderId,
            strMembershipHref,
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

### Compact production flows

#### Add flow

    strPdfFieldId = Bham.BizTalk.Rest.GallagherApiFacade.ResolvePdfFieldId(strGallagherBaseUrl, "Authorization", strApiKey, "ThirdPartyID", strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strPdfFieldKey = "pdf_" + strPdfFieldId;
    strGallagherCardholderId = Bham.BizTalk.Rest.GallagherApiFacade.ResolveGallagherCardholderId(strGallagherBaseUrl, "Authorization", strApiKey, strCardholderId, strPdfFieldKey, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strAccessGroupId = Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupIdByName(strGallagherBaseUrl, "Authorization", strApiKey, strAccessGroupName, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strResponse = Bham.BizTalk.Rest.GallagherApiFacade.AddAccessGroupToCardholder(strGallagherBaseUrl, "Authorization", strApiKey, strGallagherCardholderId, strAccessGroupId, strFromDate, strUntilDate, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);

#### Update flow

    strPdfFieldId = Bham.BizTalk.Rest.GallagherApiFacade.ResolvePdfFieldId(strGallagherBaseUrl, "Authorization", strApiKey, "ThirdPartyID", strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strPdfFieldKey = "pdf_" + strPdfFieldId;
    strGallagherCardholderId = Bham.BizTalk.Rest.GallagherApiFacade.ResolveGallagherCardholderId(strGallagherBaseUrl, "Authorization", strApiKey, strCardholderId, strPdfFieldKey, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strAccessGroupId = Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupIdByName(strGallagherBaseUrl, "Authorization", strApiKey, strAccessGroupName, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strMembershipHref = Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupMembershipHref(strGallagherBaseUrl, "Authorization", strApiKey, strAccessGroupId, strGallagherCardholderId, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strResponse = Bham.BizTalk.Rest.GallagherApiFacade.UpdateCardholderAccessGroup(strGallagherBaseUrl, "Authorization", strApiKey, strGallagherCardholderId, strMembershipHref, strFromUtc, strUntilUtc, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);

#### Remove flow

    strPdfFieldId = Bham.BizTalk.Rest.GallagherApiFacade.ResolvePdfFieldId(strGallagherBaseUrl, "Authorization", strApiKey, "ThirdPartyID", strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strPdfFieldKey = "pdf_" + strPdfFieldId;
    strGallagherCardholderId = Bham.BizTalk.Rest.GallagherApiFacade.ResolveGallagherCardholderId(strGallagherBaseUrl, "Authorization", strApiKey, strCardholderId, strPdfFieldKey, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strAccessGroupId = Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupIdByName(strGallagherBaseUrl, "Authorization", strApiKey, strAccessGroupName, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strMembershipHref = Bham.BizTalk.Rest.GallagherApiFacade.ResolveAccessGroupMembershipHref(strGallagherBaseUrl, "Authorization", strApiKey, strAccessGroupId, strGallagherCardholderId, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);
    strResponse = Bham.BizTalk.Rest.GallagherApiFacade.RemoveCardholderFromAccessGroup(strGallagherBaseUrl, "Authorization", strApiKey, strGallagherCardholderId, strMembershipHref, strCertThumbprint, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, 100);

#### 9. Optional raw reads for troubleshooting

Get all personal data fields:

    strResponse =
        Bham.BizTalk.Rest.GallagherApiFacade.GetPersonalDataFields(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

Get one personal data field by id:

    strResponse =
        Bham.BizTalk.Rest.GallagherApiFacade.GetPersonalDataFieldById(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            strPdfFieldId,
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

Get all access groups:

    strResponse =
        Bham.BizTalk.Rest.GallagherApiFacade.GetAccessGroups(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

Get one access group by id:

    strResponse =
        Bham.BizTalk.Rest.GallagherApiFacade.GetAccessGroupById(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            strAccessGroupId,
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

Get access-group cardholders:

    strResponse =
        Bham.BizTalk.Rest.GallagherApiFacade.GetAccessGroupCardholders(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            strAccessGroupId,
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

Get cardholder access groups:

    strResponse =
        Bham.BizTalk.Rest.GallagherApiFacade.GetCardholderAccessGroups(
            strGallagherBaseUrl,
            "Authorization",
            strApiKey,
            strGallagherCardholderId,
            strCertThumbprint,
            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            System.Security.Cryptography.X509Certificates.StoreName.My,
            100);

## 7. JSON parser helpers (for multi-record responses)

Use GallagherApiResponseParser when you already have response JSON and need extraction logic.

Example: find membership href by cardholder name and optional dates:

    string strMembershipHref = "";

    if (Bham.BizTalk.Rest.GallagherApiResponseParser.TryGetAccessGroupMembershipHrefByNameAndDates(
        strMembershipJson,
        strCardholderName,
        strFromDate,
        strUntilDate,
        out strMembershipHref))
    {
        // Match found
    }
    else
    {
        // No match found
    }

Available parser methods:
- GetFirstEntityId(json)
- GetEntityIdByName(json, name)
- GetAccessGroupMembershipHrefForCardholder(json, cardholderId)
- GetAccessGroupMembershipHrefByNameAndDates(json, cardholderName, fromDate, untilDate)
- TryGetAccessGroupMembershipHrefByNameAndDates(json, cardholderName, fromDate, untilDate, out href)

## 8. Error handling and logging

- Library throws BizTalkRestClientException for HTTP failures.
- Exception includes HTTP method, URL, status code (if available), and response body (if available).
- Optional logger hook: BizTalkRestClientSettings.Logger (Action<BizTalkRestLogEntry>).

### NLog and GallagherLoggingHelper

For production BizTalk orchestrations, use `GallagherLoggingHelper` to integrate NLog logging.

- Wrapper methods are available for all common Gallagher API operations with automatic NLog integration.
- NLog logger name: `Bham.BizTalk.Rest.Gallagher`
- Log properties include timestamp, operation, URL, status code, and context.

**Complete end-to-end tutorials** for both add and remove scenarios with NLog enabled:
- See [scenariologging.md](scenariologging.md) for Scenario 1 (add cardholder to access group) and Scenario 2 (remove cardholder from access group).

Example helper methods available:
- `GetCardholdersByPdfValueWithNLog(...)`
- `ResolveAccessGroupHrefByNameWithNLog(...)`
- `ResolveAccessGroupMembershipHrefWithNLog(...)`
- `RemoveCardholderFromAccessGroupWithNLog(...)`
- `AddAccessGroupToCardholderWithNLog(...)`

## 9. Build and test

Build solution:

    dotnet build .\Bham.HelperClient.sln -c Release

Build + test script:

    .\scripts\build-and-test.ps1 -Configuration Release

Run tests directly:

    [Reflection.Assembly]::LoadFrom("$PWD\Bham.BizTalk.Rest.Tests\bin\Release\Bham.BizTalk.Rest.Tests.dll") | Out-Null
    [Bham.BizTalk.Rest.Tests.TestRunner]::RunAll()

## 10. Smoke test commands

Gallagher workflow sample:

    .\scripts\SmokeTest.ps1 -Mode gallagherworkflow -ConfigPath .\samples\gallagher-workflow.sample.json

Gallagher workflow sample using a known Gallagher numeric cardholder id directly:

    .\scripts\SmokeTest.ps1 -Mode gallagherworkflow -ConfigPath .\samples\gallagher-workflow-direct-gallagher-cardholder-id.sample.json

Direct Gallagher cardholder lookup by Gallagher numeric id:

    .\scripts\SmokeTest.ps1 -Mode gallaghergetcardholderbyid -BaseUrl https://its-d-cdx-01.adf.bham.ac.uk:8904/api -ApiKey YOUR_API_KEY -GallagherCardholderId 653

Direct Gallagher cardholder access-groups lookup by Gallagher numeric id:

    .\scripts\SmokeTest.ps1 -Mode gallaghergetcardholderaccessgroups -BaseUrl https://its-d-cdx-01.adf.bham.ac.uk:8904/api -ApiKey YOUR_API_KEY -GallagherCardholderId 653

Direct Gallagher membership href lookup by access group id and Gallagher cardholder id:

    .\scripts\SmokeTest.ps1 -Mode gallagherresolvemembershiphref -BaseUrl https://its-d-cdx-01.adf.bham.ac.uk:8904/api -ApiKey YOUR_API_KEY -AccessGroupId 663 -GallagherCardholderId 653

Direct console invocation for the same lookup:

    dotnet run --project .\Bham.BizTalk.Rest.SmokeTest\Bham.BizTalk.Rest.SmokeTest.csproj -- gallaghergetcardholderbyid https://its-d-cdx-01.adf.bham.ac.uk:8904/api YOUR_API_KEY 653

Direct console invocation for cardholder access-groups:

    dotnet run --project .\Bham.BizTalk.Rest.SmokeTest\Bham.BizTalk.Rest.SmokeTest.csproj -- gallaghergetcardholderaccessgroups https://its-d-cdx-01.adf.bham.ac.uk:8904/api YOUR_API_KEY 653

Direct console invocation for membership href resolution:

    dotnet run --project .\Bham.BizTalk.Rest.SmokeTest\Bham.BizTalk.Rest.SmokeTest.csproj -- gallagherresolvemembershiphref https://its-d-cdx-01.adf.bham.ac.uk:8904/api YOUR_API_KEY 663 653

Failure scenarios:

    .\scripts\SmokeTest.ps1 -Mode scenariomissingcert
    .\scripts\SmokeTest.ps1 -Mode scenariotimeoutpublic -DelayMilliseconds 5000 -TimeoutSeconds 1
    .\scripts\SmokeTest.ps1 -Mode scenariononsuccesshttp -StatusCode 503 -TimeoutSeconds 30

## 11. Deployment notes for BizTalk

- Deploy Bham.BizTalk.Rest.dll where BizTalk host can load it (or GAC, per your standards).
- Ensure certificate is in expected store and BizTalk host account has private key access.
- Keep AssemblyVersion stable for non-breaking updates to minimize binding churn.
- Increment AssemblyFileVersion each release.

Current version metadata is declared in:
- Bham.BizTalk.Rest/Properties/AssemblyInfo.cs

## 12. Strong name and GAC (optional)

Create key:

    sn -k .\Bham.BizTalk.Rest\Bham.BizTalk.Rest.snk

Build:

    dotnet build .\Bham.HelperClient.sln -c Release

Verify strong name:

    sn -vf .\Bham.BizTalk.Rest\bin\Release\Bham.BizTalk.Rest.dll

## 13. Related sample files

- samples/ExpressionShapeSnippets.cs
- samples/GallagherApiSamples.cs
- samples/gallagher-workflow.sample.json
- samples/gallagher-workflow-direct-gallagher-cardholder-id.sample.json
- Gallagher API.postman_collection.json

## 14. Class overview

This section gives a high-level explanation of each public class in the library and what role it plays.

### BizTalkRestClientSettings

Purpose:
- Holds the connection settings used by `BizTalkRestClient` and the Gallagher wrapper classes.

What it contains:
- `ApiKeyHeaderName`: HTTP header name used for API key or token authentication.
- `ApiKeyHeaderValue`: HTTP header value sent with each request.
- `CertThumbprint`: optional client certificate thumbprint.
- `StoreLocation`: Windows certificate store location to search.
- `StoreName`: Windows certificate store name to search.
- `TimeoutSeconds`: request timeout.
- `Logger`: optional callback for request and error diagnostics.

### BizTalkRestClient

Purpose:
- Low-level reusable REST client for GET and PATCH calls.

When to use it:
- Use this in regular .NET code when you want direct control over URLs and payloads.
- In BizTalk orchestration code, prefer `GallagherApiFacade` or `PatchClient`.

### BizTalkRestLogLevel

Purpose:
- Enum that labels diagnostic messages by severity.

### BizTalkRestLogEntry

Purpose:
- Represents one diagnostic event written through the optional logger callback.

What it contains:
- Timestamp, severity, operation name, URL, message, HTTP status code, and any related exception.

### BizTalkRestClientException

Purpose:
- Exception type thrown when a REST call fails.

Why it matters:
- It carries the HTTP operation, URL, status code, and response body so failures are easier to troubleshoot from BizTalk or smoke tests.

### PatchClient

Purpose:
- Static convenience wrapper for raw GET and PATCH calls.

When to use it:
- Use this when you do not need Gallagher-specific logic and just want a direct static helper.

### GallagherApiClient

Purpose:
- Gallagher-specific wrapper over `BizTalkRestClient`.

What it does:
- Knows Gallagher endpoint paths.
- Builds Gallagher query strings.
- Creates PATCH request bodies for access-group membership operations.

When to use it:
- Use it in regular .NET code, or inside BizTalk Atomic scopes.

### GallagherApiFacade

Purpose:
- Static BizTalk-safe entry point for Gallagher operations.

Why it exists:
- BizTalk orchestration code should avoid holding non-serializable client instances in orchestration state.
- This facade lets you call Gallagher operations using only primitive arguments.
- The facade now includes explicit no-logger overloads for BizTalk Expression shape compatibility.

### GallagherApiResponseParser

Purpose:
- Extracts common values from Gallagher JSON responses.

Typical uses:
- Get the first entity id from a list response.
- Resolve an id by display name.
- Find an access-group membership href for update or remove operations.

### GallagherWorkflowOptions

Purpose:
- Configuration object used by the smoke-test and workflow helper path.

What it contains:
- All the inputs needed for common Gallagher flows, including base URL, API key, ids, dates, certificate settings, and timeout.

### GallagherWorkflowOptionsParser

Purpose:
- Loads and validates `GallagherWorkflowOptions` from JSON files or dictionaries.

When to use it:
- Use this in automation, smoke tests, or helper tooling that reads workflow configuration from files or command-line arguments.

## 15. Method reference

This section gives a simple explanation for each public method. For methods that have both a BizTalk-friendly overload and a logger-enabled overload, the explanation is the same.

### BizTalkRestClient methods

- `BizTalkRestClient(settings)`
    Creates a REST client using API key, certificate, timeout, and optional logger settings.

- `GetJson(baseUrl, queryParameters)`
    Sends an HTTP GET request and returns the response body as JSON text.

- `GetXml(baseUrl, queryParameters)`
    Sends an HTTP GET request and returns the response body as XML text.

- `PatchJson(url, jsonBody)`
    Sends an HTTP PATCH request with a JSON body and returns the response body.

- `PatchXml(url, xmlBody)`
    Sends an HTTP PATCH request with an XML body and returns the response body.

- `BuildUrl(baseUrl, queryParameters)`
    Builds a URL with query-string parameters appended and correctly escaped.

### BizTalkRestLogging methods

- `Write(logger, level, operation, url, message,exception, statusCode)`
    Sends a diagnostic event to the configured logger callback if one is present.

### PatchClient methods

- `GetJsonWithClientCertAndApiKey(...)`
    Performs a JSON GET request using an API key and an explicit certificate store location and store name.

- `GetJsonWithClientCertAndApiKeyDefaultStore(...)`
    Performs a JSON GET request using an API key and the default certificate store settings.

- `GetJsonWithApiKey(...)`
    Performs a JSON GET request using only an API key and no client certificate.

- `GetXmlWithClientCertAndApiKey(...)`
    Performs an XML GET request using an API key and an explicit certificate store location and store name.

- `GetXmlWithClientCertAndApiKeyDefaultStore(...)`
    Performs an XML GET request using an API key and the default certificate store settings.

- `GetXmlWithApiKey(...)`
    Performs an XML GET request using only an API key and no client certificate.

- `PatchJsonWithClientCertAndApiKey(...)`
    Performs a JSON PATCH request using an API key and an explicit certificate store location and store name.

- `PatchJsonWithClientCertAndApiKeyDefaultStore(...)`
    Performs a JSON PATCH request using an API key and the default certificate store settings.

- `PatchJsonWithApiKey(...)`
    Performs a JSON PATCH request using only an API key and no client certificate.

- `PatchXmlWithClientCertAndApiKey(...)`
    Performs an XML PATCH request using an API key and an explicit certificate store location and store name.

- `PatchXmlWithClientCertAndApiKeyDefaultStore(...)`
    Performs an XML PATCH request using an API key and the default certificate store settings.

- `PatchXmlWithApiKey(...)`
    Performs an XML PATCH request using only an API key and no client certificate.

- `GetWithClientCertAndApiKey(...)`
    Performs a raw GET request where you control the response content type and certificate settings.

- `PatchWithClientCertAndApiKey(...)`
    Performs a raw PATCH request where you control the request content type, response content type, and certificate settings.

### GallagherApiClient methods

- `GallagherApiClient(baseUrl, settings)`
    Creates a Gallagher client from a base URL and a settings object.

- `GallagherApiClient(baseUrl, client)`
    Creates a Gallagher client from a base URL and an already-configured low-level REST client.

- `GetPersonalDataFieldsByName(fieldName)`
    Gets Gallagher personal data field records filtered by field name.

- `GetPersonalDataFields()`
    Gets all Gallagher personal data fields.

- `GetPersonalDataFieldById(fieldId)`
    Gets one Gallagher personal data field by its numeric id.

- `GetCardholdersByPdfValue(cardholderId, pdfFieldKey)`
    Searches Gallagher cardholders where a Personal Data Field key matches the supplied external cardholder id value.

- `GetCardholders()`
    Gets all cardholders from Gallagher.

- `GetCardholderById(cardholderId)`
    Gets a single Gallagher cardholder by Gallagher's own cardholder id.

- `GetAccessGroups()`
    Gets all access groups from Gallagher.

- `GetAccessGroupById(accessGroupId)`
    Gets a single access group by its Gallagher id.

- `FindAccessGroupsByName(accessGroupName)`
    Searches access groups by name.

- `GetAccessGroupCardholders(accessGroupId)`
    Gets the cardholders currently assigned to one access group.

- `GetCardholderAccessGroups(cardholderId)`
    Gets the access groups currently assigned to one cardholder.

- `ResolvePersonalDataFieldId(fieldName)`
    Finds a personal data field by name and returns its id.

- `ResolvePdfFieldId(fieldName)`
    Alias for resolving a personal data field id when the field is used as a PDF lookup field.

- `ResolveCardholderIdByPdfValue(cardholderId, pdfFieldKey)`
    Finds a Gallagher cardholder id by searching a Personal Data Field value.

- `ResolveGallagherCardholderId(cardholderId, pdfFieldKey)`
    Alias for resolving Gallagher's own cardholder id from an external id value.

- `ResolveAccessGroupIdByName(accessGroupName)`
    Finds an access group id by its name.

- `SearchAccessGroupsByName(accessGroupName)`
    Alias for searching access groups by name.

- `ResolveAccessGroupMembershipHref(accessGroupId, cardholderId)`
    Finds the membership href that links a specific cardholder to a specific access group.

- `AddAccessGroupToCardholder(cardholderId, accessGroupId, fromDate, untilDate)`
    Adds an access group membership to a cardholder for the supplied date range.

- `RemoveAccessGroupFromCardholder(cardholderId, membershipHref)`
    Removes an access group membership from a cardholder using the membership href.

- `RemoveCardholderFromAccessGroup(cardholderId, membershipHref)`
    Alias for removing an access group membership from a cardholder.

- `UpdateAccessGroupForCardholder(cardholderId, membershipHref, fromUtc, untilUtc)`
    Updates the dates on an existing access group membership.

- `UpdateCardholderAccessGroup(cardholderId, membershipHref, fromUtc, untilUtc)`
    Alias for updating an existing access group membership.

- `BuildQuotedQueryValue(value)`
    Wraps a query value in quotes so it matches Gallagher's expected filter format.

- `BuildAddAccessGroupPatchBody(accessGroupHref, fromDate, untilDate)`
    Builds the JSON PATCH body used to add an access group membership.

- `BuildRemoveAccessGroupPatchBody(membershipHref)`
    Builds the JSON PATCH body used to remove an access group membership.

- `BuildUpdateAccessGroupPatchBody(membershipHref, fromUtc, untilUtc)`
    Builds the JSON PATCH body used to update an access group membership.

### GallagherApiFacade methods

Note:
- Many facade methods now have multiple overloads.
- The short explanation below applies to all overloads of the same method name.
- For BizTalk Expression shapes, use the overloads that do not include a logger parameter.

- `GetPersonalDataFields(...)`
    Gets all Gallagher personal data fields.

- `GetPersonalDataFieldsByName(...)`
    Gets Gallagher personal data fields filtered by field name.

- `GetPersonalDataFieldById(...)`
    Gets one personal data field by id.

- `GetCardholders(...)`
    Gets all Gallagher cardholders.

- `GetCardholdersByPdfValue(...)`
    Searches cardholders by a Personal Data Field value such as `pdf_629="IDCARD.12345"`.

- `GetCardholderById(...)`
    Gets one Gallagher cardholder by Gallagher's own id.

- `GetAccessGroups(...)`
    Gets all Gallagher access groups.

- `GetAccessGroupById(...)`
    Gets one access group by id.

- `FindAccessGroupsByName(...)`
    Searches access groups by name.

- `GetAccessGroupCardholders(...)`
    Gets the cardholders assigned to one access group.

- `GetCardholderAccessGroups(...)`
    Gets the access groups assigned to one cardholder.

- `ResolvePersonalDataFieldId(...)`
    Resolves a personal data field name to its id.

- `ResolvePdfFieldId(...)`
    Alias for resolving a personal data field id when used as a PDF field.

- `ResolveCardholderIdByPdfValue(...)`
    Resolves Gallagher's cardholder id by searching a Personal Data Field value.

- `ResolveGallagherCardholderId(...)`
    Alias for resolving Gallagher's cardholder id from an external id value.

- `ResolveAccessGroupIdByName(...)`
    Resolves an access group name to its id.

- `SearchAccessGroupsByName(...)`
    Alias for searching access groups by name.

- `ResolveAccessGroupMembershipHref(...)`
    Resolves the href for the membership record linking one cardholder to one access group.

- `AddAccessGroupToCardholder(...)`
    Adds an access group to a cardholder for a given date range.

- `RemoveAccessGroupFromCardholder(...)`
    Removes an access group assignment from a cardholder.

- `RemoveCardholderFromAccessGroup(...)`
    Alias for removing an access group assignment from a cardholder.

- `UpdateAccessGroupForCardholder(...)`
    Updates the dates on an existing access group assignment.

- `UpdateCardholderAccessGroup(...)`
    Alias for updating an existing access group assignment.

### GallagherApiResponseParser methods

- `DeserializeJsonObject(json)`
    Parses JSON text into a dictionary/object structure that the helper methods can inspect.

- `GetFirstEntityId(responseJson)`
    Returns the id of the first entity in a Gallagher response.

- `TryGetFirstEntityId(responseJson, out id)`
    Tries to return the id of the first entity without throwing if no id is found.

- `GetEntityIdByName(responseJson, name)`
    Returns the id of the entity whose `name` matches the supplied value.

- `TryGetEntityIdByName(responseJson, name, out id)`
    Tries to return the id of the entity whose `name` matches the supplied value.

- `GetAccessGroupMembershipHrefForCardholder(responseJson, cardholderId)`
    Returns the membership href for the specified cardholder within an access-group membership response.

- `TryGetAccessGroupMembershipHrefForCardholder(responseJson, cardholderId, out href)`
    Tries to return the membership href for the specified cardholder without throwing if no match is found.

- `GetAccessGroupMembershipHrefByNameAndDates(responseJson, cardholderName, fromDate, untilDate)`
    Returns the membership href for a cardholder name, optionally matching the supplied date range as well.

- `TryGetAccessGroupMembershipHrefByNameAndDates(responseJson, cardholderName, fromDate, untilDate, out href)`
    Tries to return the membership href for a cardholder name and optional dates without throwing if no match is found.

### GallagherWorkflowOptionsParser methods

- `LoadFromJsonFile(configPath)`
    Loads workflow options from a JSON configuration file.

- `ApplyNamedArguments(options, namedArguments)`
    Applies string-based named arguments onto an existing workflow options object.

- `ApplyDictionary(options, values)`
    Applies dictionary values onto an existing workflow options object.

- `ApplyValue(options, key, value)`
    Applies one key/value pair onto an existing workflow options object.

- `Validate(options)`
    Checks that the workflow options contain the required values for the requested operation.

## 16. BizTalk expression cheat sheet

Use these overloads in BizTalk Expression shapes. They avoid the trailing logger parameter and are the safest choice for orchestration binding.

| Task | Method | Recommended BizTalk call shape |
| --- | --- | --- |
| Get a Gallagher cardholder by Gallagher id | `GetCardholderById` | `baseUrl, headerName, apiKey, gallagherCardholderId, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Find cardholders by Personal Data Field value | `GetCardholdersByPdfValue` | `baseUrl, headerName, apiKey, cardholderId, pdfFieldKey, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Resolve Gallagher cardholder id from external id | `ResolveGallagherCardholderId` | `baseUrl, headerName, apiKey, cardholderId, pdfFieldKey, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Resolve Personal Data Field id by name | `ResolvePersonalDataFieldId` | `baseUrl, headerName, apiKey, fieldName, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Resolve PDF field id by name | `ResolvePdfFieldId` | `baseUrl, headerName, apiKey, fieldName, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Get all access groups | `GetAccessGroups` | `baseUrl, headerName, apiKey, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Resolve access group id by name | `ResolveAccessGroupIdByName` | `baseUrl, headerName, apiKey, accessGroupName, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Search access groups by name | `SearchAccessGroupsByName` | `baseUrl, headerName, apiKey, accessGroupName, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Get one access group by id | `GetAccessGroupById` | `baseUrl, headerName, apiKey, accessGroupId, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Get access-group cardholders | `GetAccessGroupCardholders` | `baseUrl, headerName, apiKey, accessGroupId, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Get cardholder access groups | `GetCardholderAccessGroups` | `baseUrl, headerName, apiKey, gallagherCardholderId, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Resolve membership href | `ResolveAccessGroupMembershipHref` | `baseUrl, headerName, apiKey, accessGroupId, gallagherCardholderId, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Add access group to cardholder | `AddAccessGroupToCardholder` | `baseUrl, headerName, apiKey, gallagherCardholderId, accessGroupId, fromDate, untilDate, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Remove cardholder from access group | `RemoveCardholderFromAccessGroup` | `baseUrl, headerName, apiKey, gallagherCardholderId, membershipHref, certThumbprint, storeLocation, storeName, timeoutSeconds` |
| Update cardholder access group | `UpdateCardholderAccessGroup` | `baseUrl, headerName, apiKey, gallagherCardholderId, membershipHref, fromUtc, untilUtc, certThumbprint, storeLocation, storeName, timeoutSeconds` |

Rules:
- Do not pass `null` as a final logger argument in BizTalk expressions.
- Prefer the explicit no-logger overloads listed above.
- If you already have Gallagher's numeric cardholder id, use `GetCardholderById` directly.
