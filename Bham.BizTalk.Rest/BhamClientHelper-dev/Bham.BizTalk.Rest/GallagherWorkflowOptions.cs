using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Bham.BizTalk.Rest
{
    /// <summary>
    /// Holds the configuration values required to run common Gallagher workflow operations.
    /// </summary>
    public sealed class GallagherWorkflowOptions
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string Operation { get; set; }
        public string CardholderId { get; set; }
        public string GallagherCardholderId { get; set; }
        public string PdfFieldId { get; set; }
        public string PdfFieldKey { get; set; }
        public string AccessGroupName { get; set; }
        public string AccessGroupId { get; set; }
        public string MembershipHref { get; set; }
        public string From { get; set; }
        public string Until { get; set; }
        public string Thumbprint { get; set; }
        public StoreLocation StoreLocation { get; set; } = StoreLocation.LocalMachine;
        public StoreName StoreName { get; set; } = StoreName.My;
        public int TimeoutSeconds { get; set; } = 100;
    }

    /// <summary>
    /// Loads, applies, and validates workflow configuration for Gallagher smoke-test and helper flows.
    /// </summary>
    public static class GallagherWorkflowOptionsParser
    {
        /// <summary>
        /// Loads Gallagher workflow options from a JSON configuration file.
        /// </summary>
        public static GallagherWorkflowOptions LoadFromJsonFile(string configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath))
            {
                throw new ArgumentNullException(nameof(configPath));
            }

            var fullPath = Path.GetFullPath(configPath);
            var json = File.ReadAllText(fullPath);
            var dictionary = GallagherApiResponseParser.DeserializeJsonObject(json);
            var options = new GallagherWorkflowOptions();
            ApplyDictionary(options, dictionary);
            return options;
        }

        /// <summary>
        /// Applies string-based named arguments onto an existing workflow options object.
        /// </summary>
        public static void ApplyNamedArguments(GallagherWorkflowOptions options, IDictionary<string, string> namedArguments)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (namedArguments == null) return;

            ApplyValue(options, "baseUrl", GetNamedArgument(namedArguments, "baseUrl"));
            ApplyValue(options, "apiKey", GetNamedArgument(namedArguments, "apiKey"));
            ApplyValue(options, "operation", GetNamedArgument(namedArguments, "operation"));
            ApplyValue(options, "cardholderId", GetNamedArgument(namedArguments, "cardholderId"));
            ApplyValue(options, "gallagherCardholderId", GetNamedArgument(namedArguments, "gallagherCardholderId"));
            ApplyValue(options, "pdfValue", GetNamedArgument(namedArguments, "pdfValue"));
            ApplyValue(options, "pdfFieldKey", GetNamedArgument(namedArguments, "pdfFieldKey"));
            ApplyValue(options, "pdfFieldId", GetNamedArgument(namedArguments, "pdfFieldId"));
            ApplyValue(options, "accessGroupName", GetNamedArgument(namedArguments, "accessGroupName"));
            ApplyValue(options, "accessGroupId", GetNamedArgument(namedArguments, "accessGroupId"));
            ApplyValue(options, "membershipHref", GetNamedArgument(namedArguments, "membershipHref"));
            ApplyValue(options, "from", GetNamedArgument(namedArguments, "from"));
            ApplyValue(options, "until", GetNamedArgument(namedArguments, "until"));
            ApplyValue(options, "thumbprint", GetNamedArgument(namedArguments, "thumbprint"));
            ApplyValue(options, "storeLocation", GetNamedArgument(namedArguments, "storeLocation"));
            ApplyValue(options, "storeName", GetNamedArgument(namedArguments, "storeName"));
            ApplyValue(options, "timeoutSeconds", GetNamedArgument(namedArguments, "timeoutSeconds"));
        }

        /// <summary>
        /// Applies dictionary values onto an existing workflow options object.
        /// </summary>
        public static void ApplyDictionary(GallagherWorkflowOptions options, IDictionary<string, object> values)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (values == null) return;

            foreach (var pair in values)
            {
                ApplyValue(options, pair.Key, pair.Value == null ? null : Convert.ToString(pair.Value));
            }
        }

        /// <summary>
        /// Applies one key/value pair onto an existing workflow options object.
        /// </summary>
        public static void ApplyValue(GallagherWorkflowOptions options, string key, string value)
        {
            if (options == null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            switch (key.Trim().ToLowerInvariant())
            {
                case "baseurl":
                    options.BaseUrl = value;
                    break;
                case "apikey":
                    options.ApiKey = value;
                    break;
                case "operation":
                    options.Operation = value;
                    break;
                case "cardholderid":
                    options.CardholderId = value;
                    break;
                case "gallaghercardholderid":
                    options.GallagherCardholderId = value;
                    break;
                case "pdfvalue":
                    options.CardholderId = value;
                    break;
                case "pdffieldkey":
                    options.PdfFieldKey = value;
                    break;
                case "pdffieldid":
                    options.PdfFieldId = value;
                    break;
                case "accessgroupname":
                    options.AccessGroupName = value;
                    break;
                case "accessgroupid":
                    options.AccessGroupId = value;
                    break;
                case "membershiphref":
                    options.MembershipHref = value;
                    break;
                case "from":
                    options.From = value;
                    break;
                case "until":
                    options.Until = value;
                    break;
                case "thumbprint":
                    options.Thumbprint = value;
                    break;
                case "storelocation":
                    options.StoreLocation = ParseStoreLocation(value, options.StoreLocation);
                    break;
                case "storename":
                    options.StoreName = ParseStoreName(value, options.StoreName);
                    break;
                case "timeoutseconds":
                    options.TimeoutSeconds = ParseInt(value, options.TimeoutSeconds);
                    break;
            }
        }

        /// <summary>
        /// Validates the workflow options and fills in default Gallagher values where needed.
        /// </summary>
        public static void Validate(GallagherWorkflowOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            EnsureValue(options.BaseUrl, "baseUrl");
            EnsureValue(options.ApiKey, "apiKey");
            EnsureValue(options.Operation, "operation");

            var operation = options.Operation.Trim().ToLowerInvariant();
            if (operation != "add" && operation != "remove" && operation != "update")
            {
                throw new ArgumentException("Gallagher workflow operation must be add, remove, or update.");
            }

            options.Operation = operation;
            if (string.IsNullOrWhiteSpace(options.PdfFieldKey) && !string.IsNullOrWhiteSpace(options.PdfFieldId))
            {
                options.PdfFieldKey = "pdf_" + options.PdfFieldId.Trim();
            }

            if (string.IsNullOrWhiteSpace(options.PdfFieldKey))
            {
                options.PdfFieldKey = "pdf_629";
            }
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

        private static StoreLocation ParseStoreLocation(string value, StoreLocation defaultValue)
        {
            StoreLocation parsed;
            return Enum.TryParse(value, true, out parsed) ? parsed : defaultValue;
        }

        private static StoreName ParseStoreName(string value, StoreName defaultValue)
        {
            StoreName parsed;
            return Enum.TryParse(value, true, out parsed) ? parsed : defaultValue;
        }

        private static int ParseInt(string value, int defaultValue)
        {
            int parsed;
            return int.TryParse(value, out parsed) ? parsed : defaultValue;
        }
    }
}