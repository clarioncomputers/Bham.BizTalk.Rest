using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Bham.BizTalk.Rest
{
    /// <summary>
    /// Static helper for raw GET and PATCH calls with optional API key and client certificate support.
    /// </summary>
    public static class PatchClient
    {
        // Cache HttpClient instances by certificate + store + timeout so sockets are reused safely.
        private static readonly ConcurrentDictionary<string, Lazy<HttpClient>> ClientCache =
            new ConcurrentDictionary<string, Lazy<HttpClient>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Sends a GET request with client certificate and API key header.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string GetJsonWithClientCertAndApiKey(
            string url,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return GetWithClientCertAndApiKey(
                url,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                "application/json",
                storeLocation,
                storeName,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a GET request with client certificate and API key header,
        /// using the default certificate store (LocalMachine\My).
        /// </summary>
        public static string GetJsonWithClientCertAndApiKeyDefaultStore(
            string url,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return GetWithClientCertAndApiKey(
                url,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                "application/json",
                StoreLocation.LocalMachine,
                StoreName.My,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a GET request with API key header and no client certificate.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string GetJsonWithApiKey(
            string url,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return GetWithClientCertAndApiKey(
                url,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                null,
                "application/json",
                StoreLocation.LocalMachine,
                StoreName.My,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a GET request with XML accept header, client certificate, and API key header.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string GetXmlWithClientCertAndApiKey(
            string url,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return GetWithClientCertAndApiKey(
                url,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                "application/xml",
                storeLocation,
                storeName,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a GET request with XML accept header, client certificate, and API key header,
        /// using the default certificate store (LocalMachine\My).
        /// </summary>
        public static string GetXmlWithClientCertAndApiKeyDefaultStore(
            string url,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return GetWithClientCertAndApiKey(
                url,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                "application/xml",
                StoreLocation.LocalMachine,
                StoreName.My,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a GET request with XML accept header, API key header, and no client certificate.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string GetXmlWithApiKey(
            string url,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return GetWithClientCertAndApiKey(
                url,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                null,
                "application/xml",
                StoreLocation.LocalMachine,
                StoreName.My,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a PATCH request with JSON body, client certificate, and API key header.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string PatchJsonWithClientCertAndApiKey(
            string url,
            string jsonBody,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return PatchWithClientCertAndApiKey(
                url,
                jsonBody,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                "application/json",
                "application/json",
                storeLocation,
                storeName,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a PATCH request with JSON body, client certificate, and API key header,
        /// using the default certificate store (LocalMachine\My).
        /// </summary>
        public static string PatchJsonWithClientCertAndApiKeyDefaultStore(
            string url,
            string jsonBody,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return PatchWithClientCertAndApiKey(
                url,
                jsonBody,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                "application/json",
                "application/json",
                StoreLocation.LocalMachine,
                StoreName.My,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a PATCH request with JSON body and API key header, without a client certificate.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string PatchJsonWithApiKey(
            string url,
            string jsonBody,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return PatchWithClientCertAndApiKey(
                url,
                jsonBody,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                null,
                "application/json",
                "application/json",
                StoreLocation.LocalMachine,
                StoreName.My,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a PATCH request with XML body, XML accept header, client certificate, and API key header.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string PatchXmlWithClientCertAndApiKey(
            string url,
            string xmlBody,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return PatchWithClientCertAndApiKey(
                url,
                xmlBody,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                "application/xml",
                "application/xml",
                storeLocation,
                storeName,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a PATCH request with XML body, client certificate, and API key header,
        /// using the default certificate store (LocalMachine\My).
        /// </summary>
        public static string PatchXmlWithClientCertAndApiKeyDefaultStore(
            string url,
            string xmlBody,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return PatchWithClientCertAndApiKey(
                url,
                xmlBody,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                "application/xml",
                "application/xml",
                StoreLocation.LocalMachine,
                StoreName.My,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a PATCH request with XML body and API key header, without a client certificate.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string PatchXmlWithApiKey(
            string url,
            string xmlBody,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return PatchWithClientCertAndApiKey(
                url,
                xmlBody,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                null,
                "application/xml",
                "application/xml",
                StoreLocation.LocalMachine,
                StoreName.My,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a GET request with client certificate and API key header.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string GetWithClientCertAndApiKey(
            string url,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            string acceptMediaType,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return ExecuteRequest(
                HttpMethod.Get,
                url,
                null,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                null,
                acceptMediaType,
                storeLocation,
                storeName,
                timeoutSeconds,
                logger);
        }

        /// <summary>
        /// Sends a PATCH request with configurable body and accept media types,
        /// client certificate, and API key header.
        /// Returns response body as string; throws on non-success HTTP codes.
        /// </summary>
        public static string PatchWithClientCertAndApiKey(
            string url,
            string body,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            string contentMediaType,
            string acceptMediaType,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return ExecuteRequest(
                new HttpMethod("PATCH"),
                url,
                body,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                contentMediaType,
                acceptMediaType,
                storeLocation,
                storeName,
                timeoutSeconds,
                logger);
        }

        private static string ExecuteRequest(
            HttpMethod method,
            string url,
            string body,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            string contentMediaType,
            string acceptMediaType,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds,
            Action<BizTalkRestLogEntry> logger)
        {
            ValidateRequestArguments(
                method,
                url,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                contentMediaType,
                acceptMediaType,
                timeoutSeconds);

            try
            {
                var client = GetOrCreateClient(certThumbprint, storeLocation, storeName, timeoutSeconds, logger);

                using (var request = new HttpRequestMessage(method, url))
                {
                    if (body != null)
                    {
                        request.Content = new StringContent(body, Encoding.UTF8, contentMediaType);
                    }

                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptMediaType));
                    request.Headers.Add(apiKeyHeaderName, apiKeyHeaderValue);

                    BizTalkRestLogging.Write(
                        logger,
                        BizTalkRestLogLevel.Information,
                        method.Method,
                        url,
                        string.Format("Sending {0} request. Timeout={1}s.", method.Method, timeoutSeconds));

                    using (var response = client.SendAsync(request).GetAwaiter().GetResult())
                    {
                        var responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (!response.IsSuccessStatusCode)
                        {
                            var httpException = CreateHttpFailure(method.Method, url, response.StatusCode, responseText);
                            BizTalkRestLogging.Write(
                                logger,
                                BizTalkRestLogLevel.Error,
                                method.Method,
                                url,
                                httpException.Message,
                                httpException,
                                (int)response.StatusCode);
                            throw httpException;
                        }

                        BizTalkRestLogging.Write(
                            logger,
                            BizTalkRestLogLevel.Information,
                            method.Method,
                            url,
                            string.Format(
                                "{0} request succeeded with status {1}. ResponseLength={2}.",
                                method.Method,
                                (int)response.StatusCode,
                                responseText == null ? 0 : responseText.Length),
                            statusCode: (int)response.StatusCode);

                        return responseText;
                    }
                }
            }
            catch (BizTalkRestClientException)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                var timeoutException = new BizTalkRestClientException(
                    method.Method,
                    url,
                    string.Format("{0} request timed out after {1} seconds.", method.Method, timeoutSeconds),
                    ex);
                BizTalkRestLogging.Write(
                    logger,
                    BizTalkRestLogLevel.Error,
                    method.Method,
                    url,
                    timeoutException.Message,
                    timeoutException);
                throw timeoutException;
            }
            catch (Exception ex)
            {
                var wrappedException = new BizTalkRestClientException(
                    method.Method,
                    url,
                    string.Format("{0} request failed before a successful response was received.", method.Method),
                    ex);
                BizTalkRestLogging.Write(
                    logger,
                    BizTalkRestLogLevel.Error,
                    method.Method,
                    url,
                    wrappedException.Message,
                    wrappedException);
                throw wrappedException;
            }
        }

        private static void ValidateRequestArguments(
            HttpMethod method,
            string url,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            string contentMediaType,
            string acceptMediaType,
            int timeoutSeconds)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            ValidateUrlScheme(url);
            if (string.IsNullOrWhiteSpace(apiKeyHeaderName)) throw new ArgumentNullException(nameof(apiKeyHeaderName));
            if (string.IsNullOrWhiteSpace(apiKeyHeaderValue)) throw new ArgumentNullException(nameof(apiKeyHeaderValue));
            if (string.IsNullOrWhiteSpace(acceptMediaType)) throw new ArgumentNullException(nameof(acceptMediaType));
            if (method != HttpMethod.Get && string.IsNullOrWhiteSpace(contentMediaType)) throw new ArgumentNullException(nameof(contentMediaType));
            if (timeoutSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(timeoutSeconds));
        }

        private static void ValidateUrlScheme(string url)
        {
            Uri parsed;
            if (!Uri.TryCreate(url, UriKind.Absolute, out parsed) ||
                (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("URL must be an absolute http or https URI.", "url");
            }
        }

        private static BizTalkRestClientException CreateHttpFailure(
            string operation,
            string url,
            HttpStatusCode statusCode,
            string responseText)
        {
            return new BizTalkRestClientException(
                operation,
                url,
                string.Format(
                    "{0} failed with status {1} ({2}). Response: {3}",
                    operation,
                    (int)statusCode,
                    statusCode,
                    responseText),
                statusCode: (int)statusCode,
                responseBody: responseText);
        }

        private static HttpClient GetOrCreateClient(
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds,
            Action<BizTalkRestLogEntry> logger)
        {
            var normalizedThumbprint = NormalizeThumbprint(certThumbprint);
            var thumbprintCacheKeyToken = string.IsNullOrWhiteSpace(normalizedThumbprint)
                ? "NOCERT"
                : normalizedThumbprint;
            var key = string.Format(
                "{0}|{1}|{2}|{3}",
                thumbprintCacheKeyToken,
                storeLocation,
                storeName,
                timeoutSeconds);

            var lazyClient = ClientCache.GetOrAdd(
                key,
                _ => new Lazy<HttpClient>(
                    () => CreateClient(normalizedThumbprint, storeLocation, storeName, timeoutSeconds, logger),
                    true));

            try
            {
                return lazyClient.Value;
            }
            catch
            {
                Lazy<HttpClient> removedClient;
                ClientCache.TryRemove(key, out removedClient);
                throw;
            }
        }

        private static HttpClient CreateClient(
            string thumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds,
            Action<BizTalkRestLogEntry> logger)
        {
            var handler = new HttpClientHandler();
            if (!string.IsNullOrWhiteSpace(thumbprint))
            {
                var cert = FindCertificateByThumbprint(thumbprint, storeLocation, storeName);

                BizTalkRestLogging.Write(
                    logger,
                    BizTalkRestLogLevel.Debug,
                    "HttpClient",
                    null,
                    string.Format(
                        "Creating HttpClient using certificate {0} (subject: {1}) from {2}\\{3}.",
                        MaskThumbprint(cert.Thumbprint),
                        cert.Subject,
                        storeLocation,
                        storeName));

                if (!TryAttachClientCertificate(handler, cert))
                {
                    throw new InvalidOperationException(
                        "The current System.Net.Http reference does not expose client certificate APIs on HttpClientHandler. " +
                        "Use the full .NET Framework System.Net.Http reference in the BizTalk project.");
                }
            }
            else
            {
                BizTalkRestLogging.Write(
                    logger,
                    BizTalkRestLogLevel.Debug,
                    "HttpClient",
                    null,
                    "Creating HttpClient without client certificate (API-key/header authentication only).");
            }

            var client = new HttpClient(handler, true);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            return client;
        }

        private static bool TryAttachClientCertificate(HttpClientHandler handler, X509Certificate2 cert)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (cert == null) throw new ArgumentNullException(nameof(cert));

            var property = handler.GetType().GetProperty("ClientCertificates");
            if (property == null)
            {
                return false;
            }

            var collection = property.GetValue(handler, null);
            if (collection == null)
            {
                return false;
            }

            var addMethod = collection.GetType().GetMethod("Add", new[] { typeof(X509Certificate2) })
                ?? collection.GetType().GetMethod("Add", new[] { typeof(X509Certificate) });

            if (addMethod == null)
            {
                return false;
            }

            addMethod.Invoke(collection, new object[] { cert });
            return true;
        }

        private static X509Certificate2 FindCertificateByThumbprint(
            string thumbprint,
            StoreLocation storeLocation,
            StoreName storeName)
        {
            using (var store = new X509Store(storeName, storeLocation))
            {
                store.Open(OpenFlags.ReadOnly);

                var found = store.Certificates
                    .Find(X509FindType.FindByThumbprint, thumbprint, false);

                if (found == null || found.Count == 0)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Certificate not found in {0}\\{1} for thumbprint {2}.",
                            storeLocation,
                            storeName,
                            thumbprint));
                }

                var withPrivateKey = found.Cast<X509Certificate2>().FirstOrDefault(c => c.HasPrivateKey);
                return withPrivateKey ?? found[0];
            }
        }

        private static string NormalizeThumbprint(string thumbprint)
        {
            if (string.IsNullOrWhiteSpace(thumbprint))
            {
                return null;
            }

            return thumbprint.Replace(" ", string.Empty).ToUpperInvariant();
        }

        private static string MaskThumbprint(string thumbprint)
        {
            if (string.IsNullOrWhiteSpace(thumbprint))
            {
                return string.Empty;
            }

            var normalized = thumbprint.Replace(" ", string.Empty).ToUpperInvariant();
            if (normalized.Length <= 6)
            {
                return normalized;
            }

            return new string('*', normalized.Length - 6) + normalized.Substring(normalized.Length - 6);
        }
    }
}
