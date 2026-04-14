using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bham.BizTalk.Rest;

internal static class Program
{
	private static int Main(string[] args)
	{
		if (args == null || args.Length == 0)
		{
			PrintUsage();
			return 1;
		}

		try
		{
			var mode = args[0].Trim().ToLowerInvariant();
			if (mode == "get" || mode == "getjson")
			{
				return RunGet(args, isXml: false);
			}

			if (mode == "getxml")
			{
				return RunGet(args, isXml: true);
			}

			if (mode == "patch" || mode == "patchjson")
			{
				return RunPatch(args, isXml: false);
			}

			if (mode == "patchxml")
			{
				return RunPatch(args, isXml: true);
			}

			if (mode == "getpublic" || mode == "getpublicjson")
			{
				return RunGetPublic(args, isXml: false);
			}

			if (mode == "gallaghergetpdfid")
			{
				return RunGallagherGetPdfId(args);
			}

			if (mode == "gallaghergetcardholderid")
			{
				return RunGallagherGetCardholderId(args);
			}

			if (mode == "gallaghergetcardholderbyid")
			{
				return RunGallagherGetCardholderById(args);
			}

			if (mode == "gallaghergetcardholderaccessgroups")
			{
				return RunGallagherGetCardholderAccessGroups(args);
			}

			if (mode == "gallagherresolvemembershiphref")
			{
				return RunGallagherResolveMembershipHref(args);
			}

			if (mode == "gallaghergetaccessgroups")
			{
				return RunGallagherGetAccessGroups(args);
			}

			if (mode == "gallaghersearchaccessgroup")
			{
				return RunGallagherSearchAccessGroup(args);
			}

			if (mode == "gallaghergetaccessgroupcardholders")
			{
				return RunGallagherGetAccessGroupCardholders(args);
			}

			if (mode == "gallagheraddaccessgroup")
			{
				return RunGallagherAddAccessGroup(args);
			}

			if (mode == "gallagherremoveaccessgroup")
			{
				return RunGallagherRemoveAccessGroup(args);
			}

			if (mode == "gallagherupdateaccessgroup")
			{
				return RunGallagherUpdateAccessGroup(args);
			}

			if (mode == "gallagherworkflow")
			{
				return RunGallagherWorkflow(args);
			}

			if (mode == "getpublicxml")
			{
				return RunGetPublic(args, isXml: true);
			}

			if (mode == "patchpublic" || mode == "patchpublicjson")
			{
				return RunPatchPublic(args, isXml: false);
			}

			if (mode == "patchpublicxml")
			{
				return RunPatchPublic(args, isXml: true);
			}

			if (mode == "scenariomissingcert")
			{
				return RunScenarioMissingCert(args);
			}

			if (mode == "scenariotimeoutpublic")
			{
				return RunScenarioTimeoutPublic(args, isXml: false);
			}

			if (mode == "scenariotimeoutpublicxml")
			{
				return RunScenarioTimeoutPublic(args, isXml: true);
			}

			if (mode == "scenariononsuccesshttp")
			{
				return RunScenarioNonSuccessHttp(args, isXml: false);
			}

			if (mode == "scenariononsuccesshttpxml")
			{
				return RunScenarioNonSuccessHttp(args, isXml: true);
			}

			if (mode == "scenariononsuccesshttps")
			{
				return RunScenarioNonSuccessHttps(args, isXml: false);
			}

			if (mode == "scenariononsuccesshttpsxml")
			{
				return RunScenarioNonSuccessHttps(args, isXml: true);
			}

			if (mode == "runfailurescenarios")
			{
				return RunFailureScenarios(args, isXml: false);
			}

			if (mode == "runfailurescenariosxml")
			{
				return RunFailureScenarios(args, isXml: true);
			}

			Console.Error.WriteLine("Unknown mode: " + args[0]);
			PrintUsage();
			return 1;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Smoke test failed:");
			Console.Error.WriteLine(ex.ToString());
			return 2;
		}
	}

	private static int RunGet(string[] args, bool isXml)
	{
		// getjson/getxml <baseUrl> <queryStringOrDash> <apiHeaderName> <apiHeaderValue> <thumbprint> [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 6)
		{
			Console.Error.WriteLine("GET requires at least 5 arguments after mode.");
			PrintUsage();
			return 1;
		}

		var baseUrl = args[1];
		var queryParameters = ParseQueryParameters(args[2]);
		var settings = ParseSettings(args, 3);
		var client = new BizTalkRestClient(settings);

		var result = isXml
			? client.GetXml(baseUrl, queryParameters)
			: client.GetJson(baseUrl, queryParameters);

		Console.WriteLine(isXml ? "GET XML succeeded." : "GET JSON succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunPatch(string[] args, bool isXml)
	{
		// patchjson/patchxml <url> <body> <apiHeaderName> <apiHeaderValue> <thumbprint> [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 6)
		{
			Console.Error.WriteLine("PATCH requires at least 5 arguments after mode.");
			PrintUsage();
			return 1;
		}

		var url = args[1];
		var body = args[2];
		var settings = ParseSettings(args, 3);
		var client = new BizTalkRestClient(settings);

		var result = isXml
			? client.PatchXml(url, body)
			: client.PatchJson(url, body);

		Console.WriteLine(isXml ? "PATCH XML succeeded." : "PATCH JSON succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGetPublic(string[] args, bool isXml)
	{
		// getpublicjson/getpublicxml <baseUrl> <queryStringOrDash> [timeoutSeconds]
		if (args.Length < 3)
		{
			Console.Error.WriteLine("Public GET requires at least 2 arguments after mode.");
			PrintUsage();
			return 1;
		}

		var baseUrl = args[1];
		var queryParameters = ParseQueryParameters(args[2]);
		var timeout = ParseInt(args, 3, 100);
		var url = BizTalkRestClient.BuildUrl(baseUrl, queryParameters);
		var responseText = ExecutePublicRequest(HttpMethod.Get, url, null, isXml ? "application/xml" : "application/json", timeout);

		Console.WriteLine(isXml ? "PUBLIC GET XML succeeded." : "PUBLIC GET JSON succeeded.");
		Console.WriteLine(responseText);
		return 0;
	}

	private static int RunPatchPublic(string[] args, bool isXml)
	{
		// patchpublicjson/patchpublicxml <url> <body> [timeoutSeconds]
		if (args.Length < 3)
		{
			Console.Error.WriteLine("Public PATCH requires at least 2 arguments after mode.");
			PrintUsage();
			return 1;
		}

		var url = args[1];
		var body = args[2];
		var timeout = ParseInt(args, 3, 100);
		var mediaType = isXml ? "application/xml" : "application/json";
		var responseText = ExecutePublicRequest(new HttpMethod("PATCH"), url, body, mediaType, timeout);

		Console.WriteLine(isXml ? "PUBLIC PATCH XML succeeded." : "PUBLIC PATCH JSON succeeded.");
		Console.WriteLine(responseText);
		return 0;
	}

	private static int RunGallagherGetPdfId(string[] args)
	{
		// gallaghergetpdfid <baseUrl> <apiKey> <fieldName> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 4)
		{
			Console.Error.WriteLine("Gallagher PDF ID lookup requires <baseUrl> <apiKey> <fieldName>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 4);
		var result = client.GetPersonalDataFieldsByName(args[3]);

		Console.WriteLine("Gallagher PDF ID lookup succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherGetCardholderId(string[] args)
	{
		// gallaghergetcardholderid <baseUrl> <apiKey> <externalCardholderId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 4)
		{
			Console.Error.WriteLine("Gallagher cardholder lookup requires <baseUrl> <apiKey> <externalCardholderId>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 4);
		var result = client.GetCardholdersByPdfValue(args[3]);

		Console.WriteLine("Gallagher cardholder lookup succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherGetCardholderById(string[] args)
	{
		// gallaghergetcardholderbyid <baseUrl> <apiKey> <gallagherCardholderId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 4)
		{
			Console.Error.WriteLine("Gallagher cardholder-by-id lookup requires <baseUrl> <apiKey> <gallagherCardholderId>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 4);
		var result = client.GetCardholderById(args[3]);

		Console.WriteLine("Gallagher cardholder-by-id lookup succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherGetCardholderAccessGroups(string[] args)
	{
		// gallaghergetcardholderaccessgroups <baseUrl> <apiKey> <gallagherCardholderId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 4)
		{
			Console.Error.WriteLine("Gallagher cardholder access-groups lookup requires <baseUrl> <apiKey> <gallagherCardholderId>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 4);
		var result = client.GetCardholderAccessGroups(args[3]);

		Console.WriteLine("Gallagher cardholder access-groups lookup succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherResolveMembershipHref(string[] args)
	{
		// gallagherresolvemembershiphref <baseUrl> <apiKey> <accessGroupId> <gallagherCardholderId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 5)
		{
			Console.Error.WriteLine("Gallagher membership href lookup requires <baseUrl> <apiKey> <accessGroupId> <gallagherCardholderId>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 5);
		var result = client.ResolveAccessGroupMembershipHref(args[3], args[4]);

		Console.WriteLine("Gallagher membership href lookup succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherGetAccessGroups(string[] args)
	{
		// gallaghergetaccessgroups <baseUrl> <apiKey> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 3)
		{
			Console.Error.WriteLine("Gallagher access-group list requires <baseUrl> <apiKey>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 3);
		var result = client.GetAccessGroups();

		Console.WriteLine("Gallagher access-group list succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherSearchAccessGroup(string[] args)
	{
		// gallaghersearchaccessgroup <baseUrl> <apiKey> <groupName> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 4)
		{
			Console.Error.WriteLine("Gallagher access-group search requires <baseUrl> <apiKey> <groupName>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 4);
		var result = client.FindAccessGroupsByName(args[3]);

		Console.WriteLine("Gallagher access-group search succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherGetAccessGroupCardholders(string[] args)
	{
		// gallaghergetaccessgroupcardholders <baseUrl> <apiKey> <accessGroupId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 4)
		{
			Console.Error.WriteLine("Gallagher access-group cardholders lookup requires <baseUrl> <apiKey> <accessGroupId>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 4);
		var result = client.GetAccessGroupCardholders(args[3]);

		Console.WriteLine("Gallagher access-group cardholders lookup succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherAddAccessGroup(string[] args)
	{
		// gallagheraddaccessgroup <baseUrl> <apiKey> <cardholderId> <accessGroupId> <fromDate> <untilDate> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 7)
		{
			Console.Error.WriteLine("Gallagher add-access-group requires <baseUrl> <apiKey> <cardholderId> <accessGroupId> <fromDate> <untilDate>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 7);
		var result = client.AddAccessGroupToCardholder(args[3], args[4], args[5], args[6]);

		Console.WriteLine("Gallagher add access-group succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherRemoveAccessGroup(string[] args)
	{
		// gallagherremoveaccessgroup <baseUrl> <apiKey> <cardholderId> <membershipHref> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 5)
		{
			Console.Error.WriteLine("Gallagher remove-access-group requires <baseUrl> <apiKey> <cardholderId> <membershipHref>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 5);
		var result = client.RemoveAccessGroupFromCardholder(args[3], args[4]);

		Console.WriteLine("Gallagher remove access-group succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherUpdateAccessGroup(string[] args)
	{
		// gallagherupdateaccessgroup <baseUrl> <apiKey> <cardholderId> <membershipHref> <fromUtc> <untilUtc> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]
		if (args.Length < 7)
		{
			Console.Error.WriteLine("Gallagher update-access-group requires <baseUrl> <apiKey> <cardholderId> <membershipHref> <fromUtc> <untilUtc>.");
			PrintUsage();
			return 1;
		}

		var client = CreateGallagherClient(args, 1, 2, 7);
		var result = client.UpdateAccessGroupForCardholder(args[3], args[4], args[5], args[6]);

		Console.WriteLine("Gallagher update access-group succeeded.");
		Console.WriteLine(result);
		return 0;
	}

	private static int RunGallagherWorkflow(string[] args)
	{
		var options = ParseGallagherWorkflowOptions(args);
		var client = CreateGallagherClientFromWorkflowOptions(options);

		var cardholderId = ResolveCardholderId(options, client);
		var accessGroupId = ResolveAccessGroupId(options, client);
		var membershipHref = ResolveMembershipHref(options, client, accessGroupId, cardholderId);
		string result;

		switch (options.Operation)
		{
			case "add":
				EnsureValue(cardholderId, "gallagherCardholderId or cardholderId");
				EnsureValue(accessGroupId, "accessGroupId or accessGroupName");
				EnsureValue(options.From, "from");
				EnsureValue(options.Until, "until");
				result = client.AddAccessGroupToCardholder(cardholderId, accessGroupId, options.From, options.Until);
				Console.WriteLine("Gallagher workflow add succeeded.");
				break;
			case "remove":
				EnsureValue(cardholderId, "gallagherCardholderId or cardholderId");
				EnsureValue(membershipHref, "membershipHref or accessGroupId/accessGroupName for membership lookup");
				result = client.RemoveAccessGroupFromCardholder(cardholderId, membershipHref);
				Console.WriteLine("Gallagher workflow remove succeeded.");
				break;
			case "update":
				EnsureValue(cardholderId, "gallagherCardholderId or cardholderId");
				EnsureValue(membershipHref, "membershipHref or accessGroupId/accessGroupName for membership lookup");
				EnsureValue(options.From, "from");
				EnsureValue(options.Until, "until");
				result = client.UpdateAccessGroupForCardholder(cardholderId, membershipHref, options.From, options.Until);
				Console.WriteLine("Gallagher workflow update succeeded.");
				break;
			default:
				throw new ArgumentException("Gallagher workflow operation must be add, remove, or update.");
		}

		Console.WriteLine("Resolved cardholderId=" + (cardholderId ?? "<none>"));
		Console.WriteLine("Resolved accessGroupId=" + (accessGroupId ?? "<none>"));
		Console.WriteLine("Resolved membershipHref=" + (membershipHref ?? "<none>"));
		Console.WriteLine(result);
		return 0;
	}

	private static int RunScenarioMissingCert(string[] args)
	{
		var url = args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]) ? args[1] : "https://localhost/missing-cert";
		var headerName = args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "x-api-key";
		var headerValue = args.Length > 3 && !string.IsNullOrWhiteSpace(args[3]) ? args[3] : "smoke-test";
		var thumbprint = args.Length > 4 && !string.IsNullOrWhiteSpace(args[4]) ? args[4] : "0000000000000000000000000000000000000000";
		var storeLocation = ParseStoreLocation(args, 5, StoreLocation.LocalMachine);
		var storeName = ParseStoreName(args, 6, StoreName.My);
		var timeoutSeconds = ParseInt(args, 7, 30);

		var client = new BizTalkRestClient(
			new BizTalkRestClientSettings
			{
				ApiKeyHeaderName = headerName,
				ApiKeyHeaderValue = headerValue,
				CertThumbprint = thumbprint,
				StoreLocation = storeLocation,
				StoreName = storeName,
				TimeoutSeconds = timeoutSeconds,
				Logger = WriteLog
			});

		try
		{
			client.GetJson(url);
			Console.Error.WriteLine("Missing certificate scenario failed: expected BizTalkRestClientException.");
			return 1;
		}
		catch (BizTalkRestClientException ex)
		{
			if (!(ex.InnerException is InvalidOperationException) || ex.InnerException.Message.IndexOf("Certificate not found", StringComparison.OrdinalIgnoreCase) < 0)
			{
				Console.Error.WriteLine("Missing certificate scenario failed with an unexpected exception shape.");
				Console.Error.WriteLine(ex.ToString());
				return 1;
			}

			Console.WriteLine("MISSING CERTIFICATE scenario passed.");
			Console.WriteLine(ex.InnerException.Message);
			return 0;
		}
	}

	private static int RunScenarioTimeoutPublic(string[] args, bool isXml)
	{
		var delayMilliseconds = ParseInt(args, 1, 5000);
		var timeoutSeconds = ParseInt(args, 2, 1);
		var mediaType = isXml ? "application/xml" : "application/json";

		using (var server = new LocalHttpScenarioServer(200, delayMilliseconds, "{\"status\":\"delayed\"}"))
		{
			try
			{
				ExecutePublicRequest(HttpMethod.Get, server.Url, null, mediaType, timeoutSeconds);
				Console.Error.WriteLine("Timeout scenario failed: expected request timeout.");
				return 1;
			}
			catch (TaskCanceledException ex)
			{
				Console.WriteLine(isXml ? "TIMEOUT XML scenario passed." : "TIMEOUT scenario passed.");
				Console.WriteLine(ex.Message);
				return 0;
			}
		}
	}

	private static int RunScenarioNonSuccessHttp(string[] args, bool isXml)
	{
		var statusCode = ParseInt(args, 1, 503);
		var timeoutSeconds = ParseInt(args, 2, 30);
		var responseBody = args.Length > 3 && !string.IsNullOrWhiteSpace(args[3]) ? args[3] : "{\"error\":\"forced-http-failure\"}";
		var mediaType = isXml ? "application/xml" : "application/json";

		using (var server = new LocalHttpScenarioServer(statusCode, 0, responseBody))
		{
			try
			{
				ExecutePublicRequest(HttpMethod.Get, server.Url, null, mediaType, timeoutSeconds);
				Console.Error.WriteLine("HTTP non-success scenario failed: expected non-success response.");
				return 1;
			}
			catch (HttpRequestException ex)
			{
				if (!ex.Message.StartsWith("Public GET failed:", StringComparison.Ordinal))
				{
					Console.Error.WriteLine("HTTP non-success scenario failed with an unexpected exception shape.");
					Console.Error.WriteLine(ex.ToString());
					return 1;
				}

				Console.WriteLine(isXml ? "HTTP NON-SUCCESS XML scenario passed." : "HTTP NON-SUCCESS scenario passed.");
				Console.WriteLine(ex.Message);
				return 0;
			}
		}
	}

	private static int RunScenarioNonSuccessHttps(string[] args, bool isXml)
	{
		var url = args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]) ? args[1] : "https://httpstat.us/503";
		var timeoutSeconds = ParseInt(args, 2, 30);
		var mediaType = isXml ? "application/xml" : "application/json";

		if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
		{
			Console.Error.WriteLine("HTTPS non-success scenario requires an https:// URL.");
			return 1;
		}

		try
		{
			ExecutePublicRequest(HttpMethod.Get, url, null, mediaType, timeoutSeconds);
			Console.Error.WriteLine("HTTPS non-success scenario failed: expected non-success response.");
			return 1;
		}
		catch (HttpRequestException ex)
		{
			if (!ex.Message.StartsWith("Public GET failed:", StringComparison.Ordinal))
			{
				Console.Error.WriteLine("HTTPS non-success scenario failed before an HTTP response was received.");
				Console.Error.WriteLine(ex.ToString());
				return 1;
			}

			Console.WriteLine(isXml ? "HTTPS NON-SUCCESS XML scenario passed." : "HTTPS NON-SUCCESS scenario passed.");
			Console.WriteLine(ex.Message);
			return 0;
		}
	}

	private static int RunFailureScenarios(string[] args, bool isXml)
	{
		var httpsUrl = args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]) ? args[1] : "https://httpstat.us/503";
		var timeoutMode = isXml ? "scenariotimeoutpublicxml" : "scenariotimeoutpublic";
		var httpMode = isXml ? "scenariononsuccesshttpxml" : "scenariononsuccesshttp";
		var httpsMode = isXml ? "scenariononsuccesshttpsxml" : "scenariononsuccesshttps";
		var timeoutScenario = RunScenarioTimeoutPublic(new[] { timeoutMode, "5000", "1" }, isXml);
		var missingCertScenario = RunScenarioMissingCert(new[] { "scenariomissingcert" });
		var httpScenario = RunScenarioNonSuccessHttp(new[] { httpMode, "503", "30" }, isXml);
		var httpsScenario = RunScenarioNonSuccessHttps(new[] { httpsMode, httpsUrl, "30" }, isXml);

		if (timeoutScenario == 0 && missingCertScenario == 0 && httpScenario == 0 && httpsScenario == 0)
		{
			Console.WriteLine(isXml ? "All XML failure scenarios passed." : "All failure scenarios passed.");
			return 0;
		}

		Console.Error.WriteLine("One or more failure scenarios failed.");
		return 1;
	}

	private static string ExecutePublicRequest(HttpMethod method, string url, string body, string mediaType, int timeoutSeconds)
	{
		ValidateUrl(url);
		using (var client = new HttpClient())
		using (var request = new HttpRequestMessage(method, url))
		{
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
			if (method != HttpMethod.Get)
			{
				request.Content = new StringContent(body ?? string.Empty, Encoding.UTF8, mediaType);
			}

			client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

			using (var response = client.SendAsync(request).GetAwaiter().GetResult())
			{
				var responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
						string.Format(
							"Public {0} failed: {1} ({2}). Response: {3}",
							method.Method,
							(int)response.StatusCode,
							response.StatusCode,
							responseText));
				}

				return responseText;
			}
		}
	}

	private static void ValidateUrl(string url)
	{
		Uri parsed;
		if (!Uri.TryCreate(url, UriKind.Absolute, out parsed) ||
			(parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
		{
			throw new ArgumentException("URL must be an absolute http or https URI.");
		}
	}

	private static BizTalkRestClientSettings ParseSettings(string[] args, int startIndex)
	{
		if (args.Length <= startIndex + 2)
		{
			throw new ArgumentException("Missing API header or certificate settings.");
		}

		return new BizTalkRestClientSettings
		{
			ApiKeyHeaderName = args[startIndex],
			ApiKeyHeaderValue = args[startIndex + 1],
			CertThumbprint = args[startIndex + 2],
			StoreLocation = ParseStoreLocation(args, startIndex + 3, StoreLocation.LocalMachine),
			StoreName = ParseStoreName(args, startIndex + 4, StoreName.My),
			TimeoutSeconds = ParseInt(args, startIndex + 5, 100),
			Logger = WriteLog
		};
	}

	private static GallagherApiClient CreateGallagherClient(string[] args, int baseUrlIndex, int apiKeyIndex, int optionalSettingsIndex)
	{
		if (args.Length <= baseUrlIndex || args.Length <= apiKeyIndex)
		{
			throw new ArgumentException("Missing Gallagher base URL or API key.");
		}

		var settings = new BizTalkRestClientSettings
		{
			ApiKeyHeaderName = "Authorization",
			ApiKeyHeaderValue = args[apiKeyIndex],
			CertThumbprint = ParseOptionalString(args, optionalSettingsIndex),
			StoreLocation = ParseStoreLocation(args, optionalSettingsIndex + 1, StoreLocation.LocalMachine),
			StoreName = ParseStoreName(args, optionalSettingsIndex + 2, StoreName.My),
			TimeoutSeconds = ParseInt(args, optionalSettingsIndex + 3, 100),
			Logger = WriteLog
		};

		return new GallagherApiClient(args[baseUrlIndex], settings);
	}

	private static GallagherApiClient CreateGallagherClientFromWorkflowOptions(GallagherWorkflowOptions options)
	{
		return new GallagherApiClient(
			options.BaseUrl,
			new BizTalkRestClientSettings
			{
				ApiKeyHeaderName = "Authorization",
				ApiKeyHeaderValue = options.ApiKey,
				CertThumbprint = options.Thumbprint,
				StoreLocation = options.StoreLocation,
				StoreName = options.StoreName,
				TimeoutSeconds = options.TimeoutSeconds,
				Logger = WriteLog
			});
	}

	private static GallagherWorkflowOptions ParseGallagherWorkflowOptions(string[] args)
	{
		var namedArguments = ParseNamedArguments(args, 1);
		var configPath = GetNamedArgument(namedArguments, "config");
		if (string.IsNullOrWhiteSpace(configPath) && args.Length == 2 && File.Exists(args[1]))
		{
			configPath = args[1];
		}

		var options = !string.IsNullOrWhiteSpace(configPath)
			? LoadGallagherWorkflowOptionsFromFile(configPath)
			: new GallagherWorkflowOptions();

		ApplyNamedArguments(options, namedArguments);
		ValidateGallagherWorkflowOptions(options);
		return options;
	}

	private static Dictionary<string, string> ParseNamedArguments(string[] args, int startIndex)
	{
		var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		for (var i = startIndex; i < args.Length; i++)
		{
			var current = args[i];
			if (string.IsNullOrWhiteSpace(current) || !current.StartsWith("--", StringComparison.Ordinal))
			{
				continue;
			}

			var key = current.Substring(2);
			if (string.IsNullOrWhiteSpace(key))
			{
				continue;
			}

			var value = i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal)
				? args[++i]
				: "true";

			result[key] = value;
		}

		return result;
	}

	private static GallagherWorkflowOptions LoadGallagherWorkflowOptionsFromFile(string configPath)
	{
		return GallagherWorkflowOptionsParser.LoadFromJsonFile(configPath);
	}

	private static void ApplyNamedArguments(GallagherWorkflowOptions options, IDictionary<string, string> namedArguments)
	{
		GallagherWorkflowOptionsParser.ApplyNamedArguments(options, namedArguments);
	}

	private static void ApplyDictionary(GallagherWorkflowOptions options, IDictionary<string, object> values)
	{
		GallagherWorkflowOptionsParser.ApplyDictionary(options, values);
	}

	private static void ApplyValue(GallagherWorkflowOptions options, string key, string value)
	{
		GallagherWorkflowOptionsParser.ApplyValue(options, key, value);
	}

	private static string ResolveCardholderId(GallagherWorkflowOptions options, GallagherApiClient client)
	{
		if (!string.IsNullOrWhiteSpace(options.GallagherCardholderId))
		{
			return options.GallagherCardholderId;
		}

		if (!string.IsNullOrWhiteSpace(options.CardholderId))
		{
			return client.ResolveGallagherCardholderId(options.CardholderId, options.PdfFieldKey);
		}

		return null;
	}

	private static string ResolveAccessGroupId(GallagherWorkflowOptions options, GallagherApiClient client)
	{
		if (!string.IsNullOrWhiteSpace(options.AccessGroupId))
		{
			return options.AccessGroupId;
		}

		if (!string.IsNullOrWhiteSpace(options.AccessGroupName))
		{
			return client.ResolveAccessGroupIdByName(options.AccessGroupName);
		}

		return null;
	}

	private static string ResolveMembershipHref(GallagherWorkflowOptions options, GallagherApiClient client, string accessGroupId, string cardholderId)
	{
		if (!string.IsNullOrWhiteSpace(options.MembershipHref))
		{
			return options.MembershipHref;
		}

		if (!string.IsNullOrWhiteSpace(accessGroupId) && !string.IsNullOrWhiteSpace(cardholderId))
		{
			return client.ResolveAccessGroupMembershipHref(accessGroupId, cardholderId);
		}

		return null;
	}

	private static void ValidateGallagherWorkflowOptions(GallagherWorkflowOptions options)
	{
		GallagherWorkflowOptionsParser.Validate(options);
	}

	private static string GetNamedArgument(IDictionary<string, string> namedArguments, string key)
	{
		string value;
		return namedArguments != null && namedArguments.TryGetValue(key, out value) ? value : null;
	}

	private static void EnsureValue(string value, string name)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException("Missing required Gallagher workflow value: " + name + ".");
		}
	}

	private static void WriteLog(BizTalkRestLogEntry entry)
	{
		if (entry == null)
		{
			return;
		}

		var statusText = entry.StatusCode.HasValue ? " status=" + entry.StatusCode.Value : string.Empty;
		var urlText = string.IsNullOrWhiteSpace(entry.Url) ? string.Empty : " url=" + entry.Url;
		var prefix = string.Format("[{0:O}] [{1}] [{2}]", entry.TimestampUtc, entry.Level, entry.Operation ?? "n/a");
		var line = prefix + statusText + urlText + " " + entry.Message;

		if (entry.Level == BizTalkRestLogLevel.Error)
		{
			Console.Error.WriteLine(line);
			if (entry.Exception != null)
			{
				Console.Error.WriteLine(entry.Exception.ToString());
			}
			return;
		}

		Console.WriteLine(line);
	}

	private static IDictionary<string, string> ParseQueryParameters(string queryArg)
	{
		var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		if (string.IsNullOrWhiteSpace(queryArg) || queryArg == "-")
		{
			return result;
		}

		var pairs = queryArg.Split('&');
		for (var i = 0; i < pairs.Length; i++)
		{
			if (string.IsNullOrWhiteSpace(pairs[i])) continue;

			var separatorIndex = pairs[i].IndexOf('=');
			if (separatorIndex < 0)
			{
				result[pairs[i]] = string.Empty;
				continue;
			}

			var key = pairs[i].Substring(0, separatorIndex);
			var value = pairs[i].Substring(separatorIndex + 1);
			result[key] = value;
		}

		return result;
	}

	private static StoreLocation ParseStoreLocation(string[] args, int index, StoreLocation fallback)
	{
		if (args.Length <= index || string.IsNullOrWhiteSpace(args[index]))
		{
			return fallback;
		}

		StoreLocation value;
		return Enum.TryParse(args[index], true, out value) ? value : fallback;
	}

	private static StoreName ParseStoreName(string[] args, int index, StoreName fallback)
	{
		if (args.Length <= index || string.IsNullOrWhiteSpace(args[index]))
		{
			return fallback;
		}

		StoreName value;
		return Enum.TryParse(args[index], true, out value) ? value : fallback;
	}

	private static int ParseInt(string[] args, int index, int fallback)
	{
		if (args.Length <= index || string.IsNullOrWhiteSpace(args[index]))
		{
			return fallback;
		}

		int value;
		return int.TryParse(args[index], out value) ? value : fallback;
	}

	private static string ParseOptionalString(string[] args, int index)
	{
		if (args.Length <= index || string.IsNullOrWhiteSpace(args[index]) || args[index] == "-")
		{
			return null;
		}

		return args[index];
	}

	private static void PrintUsage()
	{
		Console.WriteLine("Usage:");
		Console.WriteLine("  getjson <baseUrl> <queryStringOrDash> <apiHeaderName> <apiHeaderValue> <thumbprint> [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  getxml <baseUrl> <queryStringOrDash> <apiHeaderName> <apiHeaderValue> <thumbprint> [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  patchjson <url> <jsonBody> <apiHeaderName> <apiHeaderValue> <thumbprint> [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  patchxml <url> <xmlBody> <apiHeaderName> <apiHeaderValue> <thumbprint> [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  getpublicjson <baseUrl> <queryStringOrDash> [timeoutSeconds]");
		Console.WriteLine("  getpublicxml <baseUrl> <queryStringOrDash> [timeoutSeconds]");
		Console.WriteLine("  patchpublicjson <url> <jsonBody> [timeoutSeconds]");
		Console.WriteLine("  patchpublicxml <url> <xmlBody> [timeoutSeconds]");
		Console.WriteLine("  gallaghergetpdfid <baseUrl> <apiKey> <fieldName> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallaghergetcardholderid <baseUrl> <apiKey> <externalCardholderId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallaghergetcardholderbyid <baseUrl> <apiKey> <gallagherCardholderId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallaghergetcardholderaccessgroups <baseUrl> <apiKey> <gallagherCardholderId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallagherresolvemembershiphref <baseUrl> <apiKey> <accessGroupId> <gallagherCardholderId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallaghergetaccessgroups <baseUrl> <apiKey> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallaghersearchaccessgroup <baseUrl> <apiKey> <groupName> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallaghergetaccessgroupcardholders <baseUrl> <apiKey> <accessGroupId> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallagheraddaccessgroup <baseUrl> <apiKey> <cardholderId> <accessGroupId> <fromDate> <untilDate> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallagherremoveaccessgroup <baseUrl> <apiKey> <cardholderId> <membershipHref> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallagherupdateaccessgroup <baseUrl> <apiKey> <cardholderId> <membershipHref> <fromUtc> <untilUtc> [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  gallagherworkflow <configPath>");
		Console.WriteLine("  gallagherworkflow --baseUrl <url> --apiKey <key> --operation <add|remove|update> [--cardholderId <value>] [--gallagherCardholderId <id>] [--pdfFieldId <id>] [--pdfFieldKey <field>] [--accessGroupName <name>] [--accessGroupId <id>] [--membershipHref <href>] [--from <date>] [--until <date>] [--thumbprint <thumbprint>] [--storeLocation <location>] [--storeName <name>] [--timeoutSeconds <seconds>]");
		Console.WriteLine("  scenariomissingcert [url] [apiHeaderName] [apiHeaderValue] [thumbprint] [storeLocation] [storeName] [timeoutSeconds]");
		Console.WriteLine("  scenariotimeoutpublic [delayMilliseconds] [timeoutSeconds]");
		Console.WriteLine("  scenariotimeoutpublicxml [delayMilliseconds] [timeoutSeconds]");
		Console.WriteLine("  scenariononsuccesshttp [statusCode] [timeoutSeconds] [responseBody]");
		Console.WriteLine("  scenariononsuccesshttpxml [statusCode] [timeoutSeconds] [responseBody]");
		Console.WriteLine("  scenariononsuccesshttps [httpsUrl] [timeoutSeconds]");
		Console.WriteLine("  scenariononsuccesshttpsxml [httpsUrl] [timeoutSeconds]");
		Console.WriteLine("  runfailurescenarios [httpsNonSuccessUrl]");
		Console.WriteLine("  runfailurescenariosxml [httpsNonSuccessUrl]");
		Console.WriteLine();
		Console.WriteLine("Examples:");
		Console.WriteLine("  getjson https://api.example.com/orders \"customerId=C123&status=Open\" x-api-key abc123 001122... LocalMachine My 100");
		Console.WriteLine("  getxml https://api.example.com/orders - x-api-key abc123 001122... LocalMachine My 100");
		Console.WriteLine("  patchjson https://api.example.com/items/{id} \"{\"\"status\"\":\"\"Done\"\"}\" x-api-key abc123 001122... LocalMachine My 100");
		Console.WriteLine("  patchxml https://api.example.com/items/{id} \"<request><status>Done</status></request>\" x-api-key abc123 001122... LocalMachine My 100");
		Console.WriteLine("  getpublicjson https://jsonplaceholder.typicode.com/posts \"userId=1\" 100");
		Console.WriteLine("  patchpublicjson https://httpbin.org/patch \"{\"\"status\"\":\"\"Done\"\"}\" 100");
		Console.WriteLine("  gallaghergetpdfid https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 ThirdPartyID");
		Console.WriteLine("  gallaghergetcardholderid https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 IDCARD.12345");
		Console.WriteLine("  gallaghergetcardholderbyid https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 653");
		Console.WriteLine("  gallaghergetcardholderaccessgroups https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 653");
		Console.WriteLine("  gallagherresolvemembershiphref https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 663 653");
		Console.WriteLine("  gallaghersearchaccessgroup https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 6040-CHAMBERLAIN-B-11703");
		Console.WriteLine("  gallagheraddaccessgroup https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 653 663 2026-04-01 2026-05-01");
		Console.WriteLine("  gallagherremoveaccessgroup https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 653 https://its-d-cdx-01.adf.bham.ac.uk:8904/api/cardholders/653/access_groups/d64caab7bc5e42e8a193bd0e8b166b0b");
		Console.WriteLine("  gallagherupdateaccessgroup https://its-d-cdx-01.adf.bham.ac.uk:8904/api 2133-6820-E746-11CC-52D2-3417-CF15-2482 653 https://its-d-cdx-01.adf.bham.ac.uk:8904/api/cardholders/653/access_groups/2db46206474148c2878cca647ce74668 2026-04-01T00:00:00Z 2026-04-30T12:00:00Z");
		Console.WriteLine("  gallagherworkflow --baseUrl https://its-d-cdx-01.adf.bham.ac.uk:8904/api --apiKey 2133-6820-E746-11CC-52D2-3417-CF15-2482 --operation add --cardholderId IDCARD.12345 --pdfFieldId 629 --accessGroupName 6040-CHAMBERLAIN-B-11703 --from 2026-04-01 --until 2026-05-01");
		Console.WriteLine("  gallagherworkflow .\\samples\\gallagher-workflow.sample.json");
		Console.WriteLine("  scenariomissingcert");
		Console.WriteLine("  scenariotimeoutpublic 5000 1");
		Console.WriteLine("  scenariotimeoutpublicxml 5000 1");
		Console.WriteLine("  scenariononsuccesshttp 503 30 \"{\"\"error\"\":\"\"forced-http-failure\"\"}\"");
		Console.WriteLine("  scenariononsuccesshttpxml 503 30 \"<error>forced-http-failure</error>\"");
		Console.WriteLine("  scenariononsuccesshttps https://httpstat.us/503 30");
		Console.WriteLine("  scenariononsuccesshttpsxml https://httpstat.us/503 30");
		Console.WriteLine("  runfailurescenarios https://httpstat.us/503");
		Console.WriteLine("  runfailurescenariosxml https://httpstat.us/503");
	}

	private sealed class LocalHttpScenarioServer : IDisposable
	{
		private readonly HttpListener _listener;
		private readonly Task _requestTask;

		public LocalHttpScenarioServer(int statusCode, int delayMilliseconds, string responseBody)
		{
			if (statusCode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(statusCode));
			}

			var port = GetAvailablePort();
			Url = string.Format("http://localhost:{0}/", port);
			_listener = new HttpListener();
			_listener.Prefixes.Add(Url);
			_listener.Start();
			_requestTask = Task.Run(() => HandleSingleRequest(statusCode, delayMilliseconds, responseBody));
		}

		public string Url { get; }

		public void Dispose()
		{
			try
			{
				if (_listener.IsListening)
				{
					_listener.Stop();
				}
			}
			catch
			{
			}

			_listener.Close();

			try
			{
				_requestTask.Wait(TimeSpan.FromSeconds(2));
			}
			catch
			{
			}
		}

		private void HandleSingleRequest(int statusCode, int delayMilliseconds, string responseBody)
		{
			try
			{
				var context = _listener.GetContext();
				if (delayMilliseconds > 0)
				{
					Thread.Sleep(delayMilliseconds);
				}

				var bytes = Encoding.UTF8.GetBytes(responseBody ?? string.Empty);
				context.Response.StatusCode = statusCode;
				context.Response.ContentType = "application/json";
				context.Response.ContentLength64 = bytes.Length;
				if (bytes.Length > 0)
				{
					using (var output = context.Response.OutputStream)
					{
						output.Write(bytes, 0, bytes.Length);
					}
				}
				else
				{
					context.Response.Close();
				}
			}
			catch (HttpListenerException)
			{
			}
			catch (ObjectDisposedException)
			{
			}
			catch (IOException)
			{
			}
		}

		private static int GetAvailablePort()
		{
			var listener = new TcpListener(IPAddress.Loopback, 0);
			listener.Start();
			try
			{
				return ((IPEndPoint)listener.LocalEndpoint).Port;
			}
			finally
			{
				listener.Stop();
			}
		}
	}

}
