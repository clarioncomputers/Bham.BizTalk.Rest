using System;

namespace Bham.BizTalk.Rest.Tests
{
    /// <summary>
    /// Regression tests for Gallagher client argument validation and PATCH body generation helpers.
    /// </summary>
    internal static class GallagherApiClientTests
    {
        public static void BuildQuotedQueryValue_WrapsAndTrimsValue()
        {
            var result = GallagherApiClient.BuildQuotedQueryValue(" ThirdPartyID ");

            AssertEqual("\"ThirdPartyID\"", result);
        }

        public static void Constructor_ThrowsArgumentException_WhenBaseUrlIsNotHttpOrHttps()
        {
            var ex = ExpectThrows<ArgumentException>(
                () => new GallagherApiClient(
                    "file:///c:/temp/local-only",
                    new BizTalkRestClientSettings
                    {
                        ApiKeyHeaderName = "Authorization",
                        ApiKeyHeaderValue = "test",
                        TimeoutSeconds = 5
                    }));

            AssertContains(ex.Message, "URL must be an absolute http or https URI");
        }

        public static void GetCardholderById_ThrowsArgumentNullException_WhenIdMissing()
        {
            var client = CreateClient();

            var ex = ExpectThrows<ArgumentNullException>(() => client.GetCardholderById("  "));

            AssertContains(ex.ParamName, "cardholderId");
        }

        public static void GetAccessGroupById_ThrowsArgumentNullException_WhenIdMissing()
        {
            var client = CreateClient();

            var ex = ExpectThrows<ArgumentNullException>(() => client.GetAccessGroupById(string.Empty));

            AssertContains(ex.ParamName, "accessGroupId");
        }

        public static void GetPersonalDataFieldById_ThrowsArgumentNullException_WhenIdMissing()
        {
            var client = CreateClient();

            var ex = ExpectThrows<ArgumentNullException>(() => client.GetPersonalDataFieldById(null));

            AssertContains(ex.ParamName, "fieldId");
        }

        public static void GetCardholderAccessGroups_ThrowsArgumentNullException_WhenIdMissing()
        {
            var client = CreateClient();

            var ex = ExpectThrows<ArgumentNullException>(() => client.GetCardholderAccessGroups("\t"));

            AssertContains(ex.ParamName, "cardholderId");
        }

        public static void BuildAddAccessGroupPatchBody_MatchesGallagherShape()
        {
            var result = GallagherApiClient.BuildAddAccessGroupPatchBody(
                "https://example/api/access_groups/663",
                "2026-04-01",
                "2026-05-01");

            AssertEqual(
                "{\"accessGroups\":{\"add\":[{\"accessGroup\":{\"href\":\"https://example/api/access_groups/663\"},\"from\":\"2026-04-01\",\"until\":\"2026-05-01\"}]}}",
                result);
        }

        public static void BuildRemoveAccessGroupPatchBody_MatchesGallagherShape()
        {
            var result = GallagherApiClient.BuildRemoveAccessGroupPatchBody(
                "https://example/api/cardholders/653/access_groups/abc123");

            AssertEqual(
                "{\"accessGroups\":{\"remove\":[{\"href\":\"https://example/api/cardholders/653/access_groups/abc123\"}]}}",
                result);
        }

        public static void BuildUpdateAccessGroupPatchBody_MatchesGallagherShape()
        {
            var result = GallagherApiClient.BuildUpdateAccessGroupPatchBody(
                "https://example/api/cardholders/653/access_groups/abc123",
                "2026-04-01T00:00:00Z",
                "2026-04-30T12:00:00Z");

            AssertEqual(
                "{\"accessGroups\":{\"update\":[{\"href\":\"https://example/api/cardholders/653/access_groups/abc123\",\"from\":\"2026-04-01T00:00:00Z\",\"until\":\"2026-04-30T12:00:00Z\"}]}}",
                result);
        }

        private static void AssertEqual(string expected, string actual)
        {
            if (!string.Equals(expected, actual, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    string.Format("Assertion failed. Expected: '{0}' Actual: '{1}'", expected, actual));
            }
        }

        private static GallagherApiClient CreateClient()
        {
            return new GallagherApiClient(
                "https://example/api",
                new BizTalkRestClientSettings
                {
                    ApiKeyHeaderName = "Authorization",
                    ApiKeyHeaderValue = "test",
                    TimeoutSeconds = 5
                });
        }

        private static void AssertContains(string actual, string expectedFragment)
        {
            if (actual == null || actual.IndexOf(expectedFragment, StringComparison.Ordinal) < 0)
            {
                throw new InvalidOperationException(
                    string.Format("Assertion failed. Text '{0}' was not found in '{1}'.", expectedFragment, actual ?? "<null>"));
            }
        }

        private static TException ExpectThrows<TException>(Action action) where TException : Exception
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            try
            {
                action();
            }
            catch (TException ex)
            {
                return ex;
            }

            throw new InvalidOperationException("Expected exception was not thrown: " + typeof(TException).FullName);
        }
    }
}