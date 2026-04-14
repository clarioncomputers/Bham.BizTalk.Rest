using System;
using System.Collections.Generic;
using Bham.BizTalk.Rest;

// Gallagher API examples based on Gallagher API.postman_collection.json.
// This file is reference-only and is not compiled into the library.
internal static class GallagherApiSamples
{
	private const string BaseUrl = "https://its-d-cdx-01.adf.bham.ac.uk:8904/api";

	public static void PlainCSharpExamples()
	{
		var client = new GallagherApiClient(
			BaseUrl,
			new BizTalkRestClientSettings
			{
				ApiKeyHeaderName = "Authorization",
				ApiKeyHeaderValue = "2133-6820-E746-11CC-52D2-3417-CF15-2482",
				TimeoutSeconds = 100,
				Logger = entry => Console.WriteLine(
					$"[{entry.TimestampUtc:O}] [{entry.Level}] [{entry.Operation}] {entry.Message}")
			});

		var pdfFieldResponse = client.GetPersonalDataFieldsByName("ThirdPartyID");

		var cardholderResponse = client.GetCardholdersByPdfValue("IDCARD.12345");

		var accessGroupsResponse = client.GetAccessGroups();

		var accessGroupSearchResponse = client.FindAccessGroupsByName("6040-CHAMBERLAIN-B-11703");

		var accessGroupCardholdersResponse = client.GetAccessGroupCardholders("663");

		var addAccessGroupResponse = client.AddAccessGroupToCardholder("653", "663", "2026-04-01", "2026-05-01");

		var removeAccessGroupResponse = client.RemoveAccessGroupFromCardholder(
			"653",
			"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/cardholders/653/access_groups/d64caab7bc5e42e8a193bd0e8b166b0b");

		var updateAccessGroupResponse = client.UpdateAccessGroupForCardholder(
			"653",
			"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/cardholders/653/access_groups/2db46206474148c2878cca647ce74668",
			"2026-04-01T00:00:00Z",
			"2026-04-30T12:00:00Z");

		Console.WriteLine(pdfFieldResponse);
		Console.WriteLine(cardholderResponse);
		Console.WriteLine(accessGroupsResponse);
		Console.WriteLine(accessGroupSearchResponse);
		Console.WriteLine(accessGroupCardholdersResponse);
		Console.WriteLine(addAccessGroupResponse);
		Console.WriteLine(removeAccessGroupResponse);
		Console.WriteLine(updateAccessGroupResponse);
	}

	public static void BizTalkExpressionShapeExamples()
	{
		// Example orchestration variables:
		// strApiKey, strResponse, strPatchBody, strCardholderId, strAccessGroupId, strMembershipHref

		string strApiKey = null;
		string strResponse = null;
		string strPatchBody = null;
		string strCardholderId = null;
		string strAccessGroupId = null;
		string strMembershipHref = null;

		strResponse =
			Bham.BizTalk.Rest.PatchClient.GetJsonWithApiKey(
				"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/personal_data_fields?name=%22ThirdPartyID%22",
				"Authorization",
				strApiKey,
				100);

		strResponse =
			Bham.BizTalk.Rest.PatchClient.GetJsonWithApiKey(
				"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/cardholders?pdf_629=%22IDCARD.12345%22",
				"Authorization",
				strApiKey,
				100);

		strResponse =
			Bham.BizTalk.Rest.PatchClient.GetJsonWithApiKey(
				"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/access_groups",
				"Authorization",
				strApiKey,
				100);

		strResponse =
			Bham.BizTalk.Rest.PatchClient.GetJsonWithApiKey(
				"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/access_groups?name=%226040-CHAMBERLAIN-B-11703%22",
				"Authorization",
				strApiKey,
				100);

		strResponse =
			Bham.BizTalk.Rest.PatchClient.GetJsonWithApiKey(
				"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/access_groups/663/cardholders",
				"Authorization",
				strApiKey,
				100);

		strPatchBody =
			"{" +
			"\"accessGroups\":{" +
			"\"add\":[{" +
			"\"accessGroup\":{\"href\":\"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/access_groups/" + strAccessGroupId + "\"}," +
			"\"from\":\"2026-04-01\"," +
			"\"until\":\"2026-05-01\"" +
			"}]}" +
			"}";

		strResponse =
			Bham.BizTalk.Rest.PatchClient.PatchJsonWithApiKey(
				"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/cardholders/" + strCardholderId,
				strPatchBody,
				"Authorization",
				strApiKey,
				100);

		strPatchBody =
			"{" +
			"\"accessGroups\":{" +
			"\"remove\":[{" +
			"\"href\":\"" + strMembershipHref + "\"" +
			"}]}" +
			"}";

		strResponse =
			Bham.BizTalk.Rest.PatchClient.PatchJsonWithApiKey(
				"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/cardholders/" + strCardholderId,
				strPatchBody,
				"Authorization",
				strApiKey,
				100);

		strPatchBody =
			"{" +
			"\"accessGroups\":{" +
			"\"update\":[{" +
			"\"href\":\"" + strMembershipHref + "\"," +
			"\"from\":\"2026-04-01T00:00:00Z\"," +
			"\"until\":\"2026-04-30T12:00:00Z\"" +
			"}]}" +
			"}";

		strResponse =
			Bham.BizTalk.Rest.PatchClient.PatchJsonWithApiKey(
				"https://its-d-cdx-01.adf.bham.ac.uk:8904/api/cardholders/" + strCardholderId,
				strPatchBody,
				"Authorization",
				strApiKey,
				100);
	}
}