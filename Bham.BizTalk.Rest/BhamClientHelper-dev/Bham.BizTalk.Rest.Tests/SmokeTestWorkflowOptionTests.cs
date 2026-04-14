using System;
using System.IO;

namespace Bham.BizTalk.Rest.Tests
{
    /// <summary>
    /// Regression tests for workflow option parsing, alias handling, and default value normalization.
    /// </summary>
    internal static class SmokeTestWorkflowOptionTests
    {
        public static void Validate_BuildsPdfFieldKeyFromPdfFieldId()
        {
            var options = new GallagherWorkflowOptions();
            var arguments = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "baseUrl", "https://example/api" },
                { "apiKey", "test-key" },
                { "operation", "add" },
                { "cardholderId", "IDCARD.12345" },
                { "pdfFieldId", "629" },
                { "accessGroupName", "Group A" },
                { "from", "2026-04-01" },
                { "until", "2026-05-01" }
            };

            GallagherWorkflowOptionsParser.ApplyNamedArguments(options, arguments);
            GallagherWorkflowOptionsParser.Validate(options);

            AssertEqual("IDCARD.12345", options.CardholderId);
            AssertEqual("629", options.PdfFieldId);
            AssertEqual("pdf_629", options.PdfFieldKey);
        }

        public static void ApplyNamedArguments_AcceptsLegacyPdfValueAlias()
        {
            var options = new GallagherWorkflowOptions();
            var arguments = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "baseUrl", "https://example/api" },
                { "apiKey", "test-key" },
                { "operation", "add" },
                { "pdfValue", "IDCARD.12345" },
                { "accessGroupName", "Group A" },
                { "from", "2026-04-01" },
                { "until", "2026-05-01" }
            };

            GallagherWorkflowOptionsParser.ApplyNamedArguments(options, arguments);
            GallagherWorkflowOptionsParser.Validate(options);

            AssertEqual("IDCARD.12345", options.CardholderId);
            AssertEqual("pdf_629", options.PdfFieldKey);
        }

        public static void ApplyNamedArguments_PreservesGallagherCardholderId_WhenProvided()
        {
            var options = new GallagherWorkflowOptions();
            var arguments = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "baseUrl", "https://example/api" },
                { "apiKey", "test-key" },
                { "operation", "remove" },
                { "gallagherCardholderId", "653" },
                { "membershipHref", "https://example/api/cardholders/653/access_groups/abc" }
            };

            GallagherWorkflowOptionsParser.ApplyNamedArguments(options, arguments);
            GallagherWorkflowOptionsParser.Validate(options);

            AssertEqual("653", options.GallagherCardholderId);
        }

        public static void LoadFromJsonFile_ReadsGallagherCardholderIdSample()
        {
            var assemblyDirectory = Path.GetDirectoryName(typeof(SmokeTestWorkflowOptionTests).Assembly.Location);
            var samplePath = Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "..", "samples", "gallagher-workflow-direct-gallagher-cardholder-id.sample.json"));
            if (!File.Exists(samplePath))
            {
                samplePath = Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "..", "..", "samples", "gallagher-workflow-direct-gallagher-cardholder-id.sample.json"));
            }

            var options = GallagherWorkflowOptionsParser.LoadFromJsonFile(samplePath);
            GallagherWorkflowOptionsParser.Validate(options);

            AssertEqual("653", options.GallagherCardholderId);
            AssertEqual("663", options.AccessGroupId);
            AssertEqual("add", options.Operation);
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