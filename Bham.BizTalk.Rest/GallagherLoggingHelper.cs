using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Bham.BizTalk.Rest;
using NLog;

// Logging helper for Gallagher API operations with NLog support.
// This class provides logging-enabled wrappers around GallagherApiFacade methods.
namespace Bham.BizTalk.Rest
{
    public static class GallagherLoggingHelper 

    {
        /// <summary>
        /// Logs an orchestration transcript or any string to NLog (Info level).
        /// Call this from BizTalk Expression Shapes.
        /// </summary>
        public static void LogTranscript(string transcript)
        {
            var logger = NLog.LogManager.GetLogger(NLogLoggerName);
            logger.Info("Orchestration Transcript: {0}", transcript);
        }
        public static string ResolveAccessGroupIdByNameWithNLog(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint = null,
            int timeoutSeconds = 100)
        {
            return GallagherApiFacade.ResolveAccessGroupIdByName(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupName,
                certThumbprint,
                StoreLocation.CurrentUser,
                StoreName.My,
                timeoutSeconds,
                CreateNLogLogger("ResolveAccessGroupIdByName"));
        }

        public static string GetAccessGroupByIdWithNLog(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupId,
            string certThumbprint = null,
            int timeoutSeconds = 100)
        {
            return GallagherApiFacade.GetAccessGroupById(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupId,
                certThumbprint,
                StoreLocation.CurrentUser,
                StoreName.My,
                timeoutSeconds,
                CreateNLogLogger("GetAccessGroupById"));
        }
        /// <summary>
        /// Checks if a cardholder has a specific access group by name using GallagherApiResponseParser.
        /// </summary>
        public static bool CardholderHasAccessGroupByName(string cardholderAccessGroupsJson, string accessGroupName)
        {
            string id;
            return GallagherApiResponseParser.TryGetEntityIdByName(cardholderAccessGroupsJson, accessGroupName, out id);
        }

        /// <summary>
        /// Checks if a cardholder has a specific access group by name and date range using GallagherApiResponseParser.
        /// </summary>
        public static bool CardholderHasAccessGroupByNameAndDates(string cardholderAccessGroupsJson, string accessGroupName, string fromDate, string untilDate)
        {
            string href;
            return GallagherApiResponseParser.TryGetAccessGroupMembershipHrefByNameAndDates(cardholderAccessGroupsJson, accessGroupName, fromDate, untilDate, out href);
        }

        /// <summary>
        /// Resolves a personal data field name to its Gallagher id, with NLog logging support.
        /// </summary>
        public static string ResolvePersonalDataFieldIdWithNLog(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldName,
            string certThumbprint = null,
            int timeoutSeconds = 100)
        {
            return GallagherApiFacade.ResolvePersonalDataFieldId(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                fieldName,
                certThumbprint,
                StoreLocation.CurrentUser,
                StoreName.My,
                timeoutSeconds,
                CreateNLogLogger("ResolvePersonalDataFieldId"));
        }
        /// <summary>
        /// Example logger for use as a logCallback from BizTalk orchestrations.
        /// Writes log entry messages to Trace output.
        /// </summary>
        public static void MyOrchestrationLogger(BizTalkRestLogEntry entry)
        {
            if (entry != null)
            {
                System.Diagnostics.Trace.WriteLine(entry.Message, "Bham.BizTalk.Rest.Orchestration");
                if (entry.Exception != null)
                {
                    System.Diagnostics.Trace.WriteLine(entry.Exception.ToString(), "Bham.BizTalk.Rest.Orchestration");
                }
            }
        }
    /// <summary>
    /// Example: Call GallagherApiFacade with a custom logCallback (Action<BizTalkRestLogEntry>).
    /// Use this from an orchestration by referencing this method in an Expression shape.
    /// </summary>
    public static string GetCardholdersWithCustomLogger(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        Action<BizTalkRestLogEntry> logCallback,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.GetCardholders(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            certThumbprint,
            StoreLocation.LocalMachine,
            StoreName.My,
            timeoutSeconds,
            logCallback);
    }
    private const string EventSourceName = "Bham.BizTalk.Rest";
    private const string EventLogName = "Application";
    private const string NLogLoggerName = "Bham.BizTalk.Rest.Gallagher";

    public static string GetCardholdersWithLogging(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.GetCardholders(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateLogger("GetCardholders"));
    }

    public static string ResolveGallagherCardholderIdWithLogging(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string externalCardholderId,
        string pdfFieldKey,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.ResolveGallagherCardholderId(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            externalCardholderId,
            pdfFieldKey,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateLogger("ResolveGallagherCardholderId"));
    }

    public static string AddAccessGroupToCardholderWithLogging(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string cardholderId,
        string accessGroupId,
        string fromDate,
        string untilDate,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.AddAccessGroupToCardholder(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            cardholderId,
            accessGroupId,
            fromDate,
            untilDate,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateLogger("AddAccessGroupToCardholder"));
    }

    public static string GetCardholdersWithEventLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.GetCardholders(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateEventLogLogger("GetCardholders"));
    }

    public static string ResolveGallagherCardholderIdWithEventLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string externalCardholderId,
        string pdfFieldKey,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.ResolveGallagherCardholderId(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            externalCardholderId,
            pdfFieldKey,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateEventLogLogger("ResolveGallagherCardholderId"));
    }

    public static string AddAccessGroupToCardholderWithEventLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string cardholderId,
        string accessGroupId,
        string fromDate,
        string untilDate,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.AddAccessGroupToCardholder(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            cardholderId,
            accessGroupId,
            fromDate,
            untilDate,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateEventLogLogger("AddAccessGroupToCardholder"));
    }

    public static string GetCardholdersWithNLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.GetCardholders(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateNLogLogger("GetCardholders"));
    }

    public static string ResolveGallagherCardholderIdWithNLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string externalCardholderId,
        string pdfFieldKey,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.ResolveGallagherCardholderId(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            externalCardholderId,
            pdfFieldKey,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateNLogLogger("ResolveGallagherCardholderId"));
    }

    public static string GetCardholdersByPdfValueWithNLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string cardholderId,
        string pdfFieldKey,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.GetCardholdersByPdfValue(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            cardholderId,
            pdfFieldKey,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateNLogLogger("GetCardholdersByPdfValue"));
    }

    public static string ResolveAccessGroupHrefByNameWithNLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string accessGroupName,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.ResolveAccessGroupHrefByName(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            accessGroupName,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateNLogLogger("ResolveAccessGroupHrefByName"));
    }

    public static string ResolveAccessGroupMembershipHrefWithNLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string accessGroupHref,
        string cardholderId,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.ResolveAccessGroupMembershipHref(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            accessGroupHref,
            cardholderId,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateNLogLogger("ResolveAccessGroupMembershipHref"));
    }

    public static string AddAccessGroupToCardholderWithNLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string cardholderId,
        string accessGroupId,
        string fromDate,
        string untilDate,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.AddAccessGroupToCardholder(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            cardholderId,
            accessGroupId,
            fromDate,
            untilDate,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateNLogLogger("AddAccessGroupToCardholder"));
    }

    public static string RemoveCardholderFromAccessGroupWithNLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string cardholderId,
        string membershipHref,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.RemoveCardholderFromAccessGroup(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            cardholderId,
            membershipHref,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateNLogLogger("RemoveCardholderFromAccessGroup"));
    }

    public static string GetPersonalDataFieldsByNameWithNLog(
        string baseUrl,
        string apiKeyHeaderName,
        string apiKeyHeaderValue,
        string fieldName,
        string certThumbprint = null,
        int timeoutSeconds = 100)
    {
        return GallagherApiFacade.GetPersonalDataFieldsByName(
            baseUrl,
            apiKeyHeaderName,
            apiKeyHeaderValue,
            fieldName,
            certThumbprint,
            StoreLocation.CurrentUser,
            StoreName.My,
            timeoutSeconds,
            CreateNLogLogger("GetPersonalDataFieldsByName"));
    }

    private static Action<BizTalkRestLogEntry> CreateLogger(string context)
    {
        return entry =>
        {
            if (entry == null)
            {
                return;
            }

            var line = BuildLine(entry, context);

            // Default sink for BizTalk host processes. Replace with Event Log or SIEM forwarding if preferred.
            Trace.WriteLine(line, "Bham.BizTalk.Rest");

            if (entry.Exception != null)
            {
                Trace.WriteLine(entry.Exception.ToString(), "Bham.BizTalk.Rest");
            }
        };
    }

    private static Action<BizTalkRestLogEntry> CreateEventLogLogger(string context)
    {
        return entry =>
        {
            if (entry == null)
            {
                return;
            }

            var line = BuildLine(entry, context);

            if (!WriteEventLogSafe(line, MapToEventLogEntryType(entry.Level)))
            {
                // Fallback when source registration/write permissions are unavailable.
                Trace.WriteLine(line, EventSourceName);
            }

            if (entry.Exception != null)
            {
                var exceptionText = entry.Exception.ToString();
                if (!WriteEventLogSafe(exceptionText, EventLogEntryType.Error))
                {
                    Trace.WriteLine(exceptionText, EventSourceName);
                }
            }
        };
    }

    private static Action<BizTalkRestLogEntry> CreateNLogLogger(string context)
    {
        var logger = NLog.LogManager.GetLogger(NLogLoggerName);

        return entry =>
        {
            if (entry == null)
            {
                return;
            }

            var evt = new NLog.LogEventInfo
            {
                LoggerName = logger.Name,
                Level = MapToNLogLevel(entry.Level),
                Message = BuildLine(entry, context),
                Exception = entry.Exception
            };

            evt.Properties["timestampUtc"] = entry.TimestampUtc;
            evt.Properties["operation"] = entry.Operation ?? string.Empty;
            evt.Properties["url"] = entry.Url ?? string.Empty;
            evt.Properties["statusCode"] = entry.StatusCode.HasValue ? entry.StatusCode.Value : 0;
            evt.Properties["context"] = context;

            logger.Log(evt);
        };
    }

    private static string BuildLine(BizTalkRestLogEntry entry, string context)
    {
        var statusText = entry.StatusCode.HasValue ? entry.StatusCode.Value.ToString() : "-";
        var urlText = string.IsNullOrWhiteSpace(entry.Url) ? "-" : entry.Url;
        return string.Format(
            "[{0:O}] [{1}] [{2}] [{3}] status={4} url={5} {6}",
            entry.TimestampUtc,
            entry.Level,
            context,
            entry.Operation ?? "n/a",
            statusText,
            urlText,
            entry.Message ?? string.Empty);
    }

    private static bool WriteEventLogSafe(string message, EventLogEntryType entryType)
    {
        try
        {
            if (!EventLog.SourceExists(EventSourceName))
            {
                EventLog.CreateEventSource(EventSourceName, EventLogName);
            }

            EventLog.WriteEntry(EventSourceName, message ?? string.Empty, entryType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static EventLogEntryType MapToEventLogEntryType(BizTalkRestLogLevel level)
    {
        switch (level)
        {
            case BizTalkRestLogLevel.Error:
                return EventLogEntryType.Error;
            case BizTalkRestLogLevel.Warning:
                return EventLogEntryType.Warning;
            default:
                return EventLogEntryType.Information;
        }
    }

    private static NLog.LogLevel MapToNLogLevel(BizTalkRestLogLevel level)
    {
        switch (level)
        {
            case BizTalkRestLogLevel.Debug:
                return NLog.LogLevel.Debug;
            case BizTalkRestLogLevel.Information:
                return NLog.LogLevel.Info;
            case BizTalkRestLogLevel.Warning:
                return NLog.LogLevel.Warn;
            case BizTalkRestLogLevel.Error:
                return NLog.LogLevel.Error;
            default:
                return NLog.LogLevel.Info;
        }
    }
    }
}