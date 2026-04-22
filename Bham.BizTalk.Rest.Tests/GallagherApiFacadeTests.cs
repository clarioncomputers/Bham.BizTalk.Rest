using System;

namespace Bham.BizTalk.Rest.Tests
{
    /// <summary>
    /// Regression tests for facade-level argument validation and BizTalk-safe entry points.
    /// </summary>
    internal static class GallagherApiFacadeTests
    {
        public static void GetCardholderById_ThrowsArgumentNullException_WhenIdMissing()
        {
            var ex = ExpectThrows<ArgumentNullException>(() => GallagherApiFacade.GetCardholderById(
                "https://example/api",
                "Authorization",
                "test",
                "   "));

            AssertContains(ex.ParamName, "cardholderId");
        }

        public static void GetCardholders_ThrowsArgumentException_WhenBaseUrlIsNotHttpOrHttps()
        {
            var ex = ExpectThrows<ArgumentException>(() => GallagherApiFacade.GetCardholders(
                "file:///c:/temp/local-only",
                "Authorization",
                "test"));

            AssertContains(ex.Message, "URL must be an absolute http or https URI");
        }

        public static void GetCardholders_ThrowsArgumentNullException_WhenHeaderNameMissing()
        {
            var ex = ExpectThrows<ArgumentNullException>(() => GallagherApiFacade.GetCardholders(
                "https://example/api",
                "  ",
                "test"));

            AssertContains(ex.ParamName, "ApiKeyHeaderName");
        }

        public static void ConvertUkDateAndTimeToUtcIso8601_ConvertsBstToUtc()
        {
            var result = GallagherApiFacade.ConvertUkDateAndTimeToUtcIso8601("17/04/26", "10:00");

            AssertEqual("2026-04-17T09:00:00Z", result);
        }

        public static void ConvertUkDateAndTimeToUtcIso8601_ConvertsWinterTimeToUtc()
        {
            var result = GallagherApiFacade.ConvertUkDateAndTimeToUtcIso8601("17/12/26", "10:00");

            AssertEqual("2026-12-17T10:00:00Z", result);
        }

        public static void ConvertUkDateAndTimeToUtcIso8601_ThrowsFormatException_WhenDateIsInvalid()
        {
            ExpectThrows<FormatException>(() => GallagherApiFacade.ConvertUkDateAndTimeToUtcIso8601("2026-04-17", "10:00"));
        }

        public static void ConvertUkDateAndTimeToUtcIso8601_ThrowsArgumentNullException_WhenTimeMissing()
        {
            var ex = ExpectThrows<ArgumentNullException>(() => GallagherApiFacade.ConvertUkDateAndTimeToUtcIso8601("17/04/26", "  "));

            AssertContains(ex.ParamName, "ukTime");
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
