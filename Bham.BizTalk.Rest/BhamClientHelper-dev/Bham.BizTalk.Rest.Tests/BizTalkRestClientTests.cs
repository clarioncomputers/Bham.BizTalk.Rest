using System;
using System.Collections.Generic;

namespace Bham.BizTalk.Rest.Tests
{
    /// <summary>
    /// Regression tests for URL construction and basic client argument validation.
    /// </summary>
    internal static class BizTalkRestClientTests
    {
        public static void BuildUrl_AppendsEncodedQueryParameters()
        {
            var query = new Dictionary<string, string>
            {
                { "customer id", "C 123" },
                { "status", "Open/Closed" }
            };

            var result = BizTalkRestClient.BuildUrl("https://api.example.com/orders", query);

            AssertEqual("https://api.example.com/orders?customer%20id=C%20123&status=Open%2FClosed", result);
        }

        public static void BuildUrl_AppendsWithAmpersand_WhenBaseAlreadyHasQuery()
        {
            var query = new Dictionary<string, string>
            {
                { "status", "Open" }
            };

            var result = BizTalkRestClient.BuildUrl("https://api.example.com/orders?customerId=C123", query);

            AssertEqual("https://api.example.com/orders?customerId=C123&status=Open", result);
        }

        public static void GetJson_ThrowsArgumentException_WhenUrlIsNotHttpOrHttps()
        {
            var client = new BizTalkRestClient(new BizTalkRestClientSettings
            {
                ApiKeyHeaderName = "x-api-key",
                ApiKeyHeaderValue = "test",
                CertThumbprint = "0011223344556677889900112233445566778899",
                TimeoutSeconds = 5
            });

            var ex = ExpectThrows<ArgumentException>(() => client.GetJson("file:///windows/system32/drivers/etc/hosts"));

            AssertContains(ex.Message, "URL must be an absolute http or https URI");
        }

        private static void AssertEqual(string expected, string actual)
        {
            if (!string.Equals(expected, actual, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    string.Format("Assertion failed. Expected: '{0}' Actual: '{1}'", expected, actual));
            }
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
