using System;

namespace Bham.BizTalk.Rest.Tests
{
    /// <summary>
    /// Regression tests for parsing Starrez Entry XML helper values.
    /// </summary>
    internal static class StarrezHelperTests
    {
        public static void ExtractEntryId_ReturnsEntryId_WhenElementExists()
        {
            var xml = "<Results><Entry><EntryID>823543</EntryID><CategoryID>5</CategoryID></Entry></Results>";

            var result = StarrezHelper.ExtractEntryId(xml);

            AssertEqual("823543", result);
        }

        public static void ExtractEntryId_ReturnsEmpty_WhenElementMissing()
        {
            var xml = "<Results><Entry><CategoryID>5</CategoryID></Entry></Results>";

            var result = StarrezHelper.ExtractEntryId(xml);

            AssertEqual(string.Empty, result);
        }

        public static void ExtractEntryId_ReturnsEmpty_WhenXmlInvalid()
        {
            var xml = "<Results><Entry><EntryID>823543";

            var result = StarrezHelper.ExtractEntryId(xml);

            AssertEqual(string.Empty, result);
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
