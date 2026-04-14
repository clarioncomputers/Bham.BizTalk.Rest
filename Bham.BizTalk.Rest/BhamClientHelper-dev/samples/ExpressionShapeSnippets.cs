// BizTalk Expression Shape snippets for Bham.BizTalk.Rest.PatchClient
//
// Copy the relevant statement directly into a BizTalk Expression shape.
// Declare the corresponding orchestration variables (System.String) first.
//
// Required assembly reference in your BizTalk project:
//   Bham.BizTalk.Rest.dll
//
// Required orchestration variables (all System.String):
//   strApiKey          - your API key value
//   strCustomerId      - customer identifier
//   strOrderId         - order identifier
//   strCertThumbprint  - certificate thumbprint (when using cert overloads)
//   strGetResponse     - receives the GET response body
//   strPatchBody       - holds the JSON/XML payload to PATCH
//   strPatchResponse   - receives the PATCH response body

// ─────────────────────────────────────────────────────────────────
// GET JSON  –  no client certificate
// ─────────────────────────────────────────────────────────────────
strGetResponse =
    Bham.BizTalk.Rest.PatchClient.GetJsonWithApiKey(
        "https://api.example.com/orders?customerId=" + strCustomerId + "&status=Open",
        "x-api-key",
        strApiKey,
        100);

// ─────────────────────────────────────────────────────────────────
// GET JSON  –  client certificate, default store (LocalMachine\My)
// ─────────────────────────────────────────────────────────────────
strGetResponse =
    Bham.BizTalk.Rest.PatchClient.GetJsonWithClientCertAndApiKeyDefaultStore(
        "https://api.example.com/orders?customerId=" + strCustomerId + "&status=Open",
        "x-api-key",
        strApiKey,
        strCertThumbprint,
        100);

// ─────────────────────────────────────────────────────────────────
// GET XML  –  no client certificate
// ─────────────────────────────────────────────────────────────────
strGetResponse =
    Bham.BizTalk.Rest.PatchClient.GetXmlWithApiKey(
        "https://api.example.com/orders?customerId=" + strCustomerId + "&status=Open",
        "x-api-key",
        strApiKey,
        100);

// ─────────────────────────────────────────────────────────────────
// GET XML  –  client certificate, default store (LocalMachine\My)
// ─────────────────────────────────────────────────────────────────
strGetResponse =
    Bham.BizTalk.Rest.PatchClient.GetXmlWithClientCertAndApiKeyDefaultStore(
        "https://api.example.com/orders?customerId=" + strCustomerId + "&status=Open",
        "x-api-key",
        strApiKey,
        strCertThumbprint,
        100);

// ─────────────────────────────────────────────────────────────────
// PATCH JSON  –  no client certificate
// ─────────────────────────────────────────────────────────────────
strPatchBody =
    "{"
    + "\"status\":\"Done\","
    + "\"updatedBy\":\"BizTalk\""
    + "}";

strPatchResponse =
    Bham.BizTalk.Rest.PatchClient.PatchJsonWithApiKey(
        "https://api.example.com/orders/" + strOrderId,
        strPatchBody,
        "x-api-key",
        strApiKey,
        100);

// ─────────────────────────────────────────────────────────────────
// PATCH JSON  –  client certificate, default store (LocalMachine\My)
// ─────────────────────────────────────────────────────────────────
strPatchResponse =
    Bham.BizTalk.Rest.PatchClient.PatchJsonWithClientCertAndApiKeyDefaultStore(
        "https://api.example.com/orders/" + strOrderId,
        strPatchBody,
        "x-api-key",
        strApiKey,
        strCertThumbprint,
        100);

// ─────────────────────────────────────────────────────────────────
// PATCH XML  –  no client certificate
// ─────────────────────────────────────────────────────────────────
strPatchBody =
    "<request>"
    + "<status>Done</status>"
    + "<updatedBy>BizTalk</updatedBy>"
    + "</request>";

strPatchResponse =
    Bham.BizTalk.Rest.PatchClient.PatchXmlWithApiKey(
        "https://api.example.com/orders/" + strOrderId,
        strPatchBody,
        "x-api-key",
        strApiKey,
        100);

// ─────────────────────────────────────────────────────────────────
// PATCH XML  –  client certificate, default store (LocalMachine\My)
// ─────────────────────────────────────────────────────────────────
strPatchResponse =
    Bham.BizTalk.Rest.PatchClient.PatchXmlWithClientCertAndApiKeyDefaultStore(
        "https://api.example.com/orders/" + strOrderId,
        strPatchBody,
        "x-api-key",
        strApiKey,
        strCertThumbprint,
        100);
