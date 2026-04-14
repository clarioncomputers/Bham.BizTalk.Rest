using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Bham.BizTalk.Rest
{
    /// <summary>
    /// Holds the API key, certificate, timeout, and logger settings used by BizTalk REST calls.
    /// </summary>
    public sealed class BizTalkRestClientSettings
    {
        public string ApiKeyHeaderName { get; set; }

        public string ApiKeyHeaderValue { get; set; }

        public string CertThumbprint { get; set; }

        public StoreLocation StoreLocation { get; set; } = StoreLocation.LocalMachine;

        public StoreName StoreName { get; set; } = StoreName.My;

        public int TimeoutSeconds { get; set; } = 100;

        public Action<BizTalkRestLogEntry> Logger { get; set; }
    }

    /// <summary>
    /// Thin BizTalk-facing wrapper that centralizes URL/query composition and
    /// common JSON/XML GET and PATCH calls.
    /// </summary>
    public sealed class BizTalkRestClient
    {
        private readonly BizTalkRestClientSettings _settings;

        /// <summary>
        /// Creates a REST client from a settings object containing authentication, certificate, and timeout values.
        /// </summary>
        public BizTalkRestClient(BizTalkRestClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
            if (string.IsNullOrWhiteSpace(_settings.ApiKeyHeaderName)) throw new ArgumentNullException(nameof(settings.ApiKeyHeaderName));
            if (string.IsNullOrWhiteSpace(_settings.ApiKeyHeaderValue)) throw new ArgumentNullException(nameof(settings.ApiKeyHeaderValue));
            if (_settings.TimeoutSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(settings.TimeoutSeconds));
        }

        /// <summary>
        /// Sends a GET request and returns the response body as JSON text.
        /// </summary>
        public string GetJson(string baseUrl, IDictionary<string, string> queryParameters = null)
        {
            var url = BuildUrl(baseUrl, queryParameters);
            return PatchClient.GetWithClientCertAndApiKey(
                url,
                _settings.ApiKeyHeaderName,
                _settings.ApiKeyHeaderValue,
                _settings.CertThumbprint,
                "application/json",
                _settings.StoreLocation,
                _settings.StoreName,
                _settings.TimeoutSeconds,
                _settings.Logger);
        }

        /// <summary>
        /// Sends a GET request and returns the response body as XML text.
        /// </summary>
        public string GetXml(string baseUrl, IDictionary<string, string> queryParameters = null)
        {
            var url = BuildUrl(baseUrl, queryParameters);
            return PatchClient.GetWithClientCertAndApiKey(
                url,
                _settings.ApiKeyHeaderName,
                _settings.ApiKeyHeaderValue,
                _settings.CertThumbprint,
                "application/xml",
                _settings.StoreLocation,
                _settings.StoreName,
                _settings.TimeoutSeconds,
                _settings.Logger);
        }

        /// <summary>
        /// Sends a PATCH request with a JSON payload and returns the response body.
        /// </summary>
        public string PatchJson(string url, string jsonBody)
        {
            return PatchClient.PatchWithClientCertAndApiKey(
                url,
                jsonBody,
                _settings.ApiKeyHeaderName,
                _settings.ApiKeyHeaderValue,
                _settings.CertThumbprint,
                "application/json",
                "application/json",
                _settings.StoreLocation,
                _settings.StoreName,
                _settings.TimeoutSeconds,
                _settings.Logger);
        }

        /// <summary>
        /// Sends a PATCH request with an XML payload and returns the response body.
        /// </summary>
        public string PatchXml(string url, string xmlBody)
        {
            return PatchClient.PatchWithClientCertAndApiKey(
                url,
                xmlBody,
                _settings.ApiKeyHeaderName,
                _settings.ApiKeyHeaderValue,
                _settings.CertThumbprint,
                "application/xml",
                "application/xml",
                _settings.StoreLocation,
                _settings.StoreName,
                _settings.TimeoutSeconds,
                _settings.Logger);
        }

        /// <summary>
        /// Builds a URL with escaped query-string parameters appended.
        /// </summary>
        public static string BuildUrl(string baseUrl, IDictionary<string, string> queryParameters)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));
            if (queryParameters == null || queryParameters.Count == 0) return baseUrl;

            var query = string.Join(
                "&",
                queryParameters
                    .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key))
                    .Select(kvp =>
                        Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value ?? string.Empty)));

            if (string.IsNullOrWhiteSpace(query)) return baseUrl;
            return baseUrl.Contains("?") ? baseUrl + "&" + query : baseUrl + "?" + query;
        }
    }
}
