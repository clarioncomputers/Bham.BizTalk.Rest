using System;

namespace Bham.BizTalk.Rest.Tests
{
    /// <summary>
    /// Regression tests for extracting ids and membership hrefs from Gallagher JSON responses.
    /// </summary>
    internal static class GallagherApiResponseParserTests
    {
        public static void GetFirstEntityId_FindsFirstNestedId()
        {
            var responseJson = "{\"results\":[{\"id\":\"629\",\"name\":\"ThirdPartyID\"}]}";

            var result = GallagherApiResponseParser.GetFirstEntityId(responseJson);

            AssertEqual("629", result);
        }

        public static void GetEntityIdByName_FindsMatchingName()
        {
            var responseJson = "{\"results\":[{\"id\":\"662\",\"name\":\"Other\"},{\"id\":\"663\",\"name\":\"6040-CHAMBERLAIN-B-11703\"}]}";

            var result = GallagherApiResponseParser.GetEntityIdByName(responseJson, "6040-CHAMBERLAIN-B-11703");

            AssertEqual("663", result);
        }

        public static void GetAccessGroupMembershipHrefForCardholder_FindsMembershipHref()
        {
            var responseJson = "{\"results\":[{\"href\":\"https://host/api/cardholders/653/access_groups/abc123\",\"cardholder\":{\"id\":\"653\",\"href\":\"https://host/api/cardholders/653\"}}]}";

            var result = GallagherApiResponseParser.GetAccessGroupMembershipHrefForCardholder(responseJson, "653");

            AssertEqual("https://host/api/cardholders/653/access_groups/abc123", result);
        }

        public static void GetAccessGroupMembershipHrefByNameAndDates_FindsMatchingRecord()
        {
            var responseJson = "{\"results\":["
                + "{\"href\":\"https://host/api/cardholders/111/access_groups/old\",\"cardholder\":{\"name\":\"Jane Smith\"},\"from\":\"2026-01-01\",\"until\":\"2026-01-31\"},"
                + "{\"href\":\"https://host/api/cardholders/653/access_groups/abc123\",\"cardholder\":{\"name\":\"John Smith\"},\"from\":\"2026-04-01T00:00:00Z\",\"until\":\"2026-05-01T00:00:00Z\"}"
                + "]}";

            var result = GallagherApiResponseParser.GetAccessGroupMembershipHrefByNameAndDates(
                responseJson,
                "John Smith",
                "2026-04-01",
                "2026-05-01");

            AssertEqual("https://host/api/cardholders/653/access_groups/abc123", result);
        }

        public static void TryGetAccessGroupMembershipHrefByNameAndDates_AllowsNameOnlyFilter()
        {
            var responseJson = "{\"results\":[{\"href\":\"https://host/api/cardholders/777/access_groups/xyz\",\"cardholder\":{\"name\":\"Alice Brown\"},\"from\":\"2026-06-01\",\"until\":\"2026-06-30\"}]}";

            string result;
            var found = GallagherApiResponseParser.TryGetAccessGroupMembershipHrefByNameAndDates(
                responseJson,
                "Alice Brown",
                null,
                null,
                out result);

            AssertEqual("True", found.ToString());
            AssertEqual("https://host/api/cardholders/777/access_groups/xyz", result);
        }

        private static void AssertEqual(string expected, string actual)
        {
            if (!string.Equals(expected, actual, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    string.Format("Assertion failed. Expected: '{0}' Actual: '{1}'", expected, actual));
            }
        }
    }
}