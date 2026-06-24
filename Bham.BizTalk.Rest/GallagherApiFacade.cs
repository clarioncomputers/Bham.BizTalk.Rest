using System;
using System.Security.Cryptography.X509Certificates;

namespace Bham.BizTalk.Rest
{
    /// <summary>
    /// BizTalk-safe static facade over GallagherApiClient.
    /// Callers pass primitive arguments and do not hold wrapper instances in orchestration state.
    /// </summary>
    public static class GallagherApiFacade
    {
        /// <summary>
        /// Gets all Gallagher personal data fields using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetPersonalDataFields(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetPersonalDataFields(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets all Gallagher personal data fields, with optional certificate and logger support.
        /// </summary>
        public static string GetPersonalDataFields(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetPersonalDataFields();
        }

        /// <summary>
        /// Gets Gallagher personal data fields filtered by field name using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetPersonalDataFieldsByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldName,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetPersonalDataFieldsByName(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                fieldName,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets Gallagher personal data fields filtered by field name, with optional certificate and logger support.
        /// </summary>
        public static string GetPersonalDataFieldsByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldName,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetPersonalDataFieldsByName(fieldName);
        }

        /// <summary>
        /// Gets one Gallagher personal data field by id using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetPersonalDataFieldById(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldId,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetPersonalDataFieldById(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                fieldId,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets one Gallagher personal data field by id, with optional certificate and logger support.
        /// </summary>
        public static string GetPersonalDataFieldById(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldId,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetPersonalDataFieldById(fieldId);
        }

        /// <summary>
        /// Gets all Gallagher cardholders using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetCardholders(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetCardholders(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets all Gallagher cardholders, with optional certificate and logger support.
        /// </summary>
        public static string GetCardholders(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetCardholders();
        }

        /// <summary>
        /// Searches Gallagher cardholders by Personal Data Field value using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetCardholdersByPdfValue(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string pdfFieldKey,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetCardholdersByPdfValue(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                pdfFieldKey,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Searches Gallagher cardholders by Personal Data Field value, with optional certificate and logger support.
        /// </summary>
        public static string GetCardholdersByPdfValue(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string pdfFieldKey = "pdf_629",
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetCardholdersByPdfValue(cardholderId, pdfFieldKey);
        }

        /// <summary>
        /// Gets one Gallagher cardholder by Gallagher id using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetCardholderById(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetCardholderById(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets one Gallagher cardholder by Gallagher id using default certificate-store settings and timeout.
        /// </summary>
        public static string GetCardholderById(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId)
        {
            return GetCardholderById(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                null,
                StoreLocation.LocalMachine,
                StoreName.My,
                100,
                null);
        }

        /// <summary>
        /// Gets one Gallagher cardholder by Gallagher id, with optional certificate and logger support.
        /// </summary>
        public static string GetCardholderById(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetCardholderById(cardholderId);
        }

        /// <summary>
        /// Gets all Gallagher access groups using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetAccessGroups(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetAccessGroups(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets all Gallagher access groups, with optional certificate and logger support.
        /// </summary>
        public static string GetAccessGroups(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetAccessGroups();
        }

        /// <summary>
        /// Gets one Gallagher access group by id using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetAccessGroupById(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupId,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetAccessGroupById(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupId,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets one Gallagher access group by id, with optional certificate and logger support.
        /// </summary>
        public static string GetAccessGroupById(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupId,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetAccessGroupById(accessGroupId);
        }

        /// <summary>
        /// Searches Gallagher access groups by name using an explicit certificate and timeout configuration.
        /// </summary>
        public static string FindAccessGroupsByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return FindAccessGroupsByName(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupName,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Searches Gallagher access groups by name, with optional certificate and logger support.
        /// </summary>
        public static string FindAccessGroupsByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .FindAccessGroupsByName(accessGroupName);
        }

        /// <summary>
        /// Gets the cardholders assigned to an access group using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetAccessGroupCardholders(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupId,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetAccessGroupCardholders(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupId,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets the cardholders assigned to an access group, with optional certificate and logger support.
        /// </summary>
        public static string GetAccessGroupCardholders(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupId,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetAccessGroupCardholders(accessGroupId);
        }

        /// <summary>
        /// Gets the access groups assigned to a cardholder using an explicit certificate and timeout configuration.
        /// </summary>
        public static string GetCardholderAccessGroups(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return GetCardholderAccessGroups(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Gets the access groups assigned to a cardholder, with optional certificate and logger support.
        /// </summary>
        public static string GetCardholderAccessGroups(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .GetCardholderAccessGroups(cardholderId);
        }

        /// <summary>
        /// Resolves a personal data field name to its Gallagher id using an explicit certificate and timeout configuration.
        /// </summary>
        public static string ResolvePersonalDataFieldId(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldName,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return ResolvePersonalDataFieldId(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                fieldName,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Resolves a personal data field name to its Gallagher id, with optional certificate and logger support.
        /// </summary>
        public static string ResolvePersonalDataFieldId(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldName,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .ResolvePersonalDataFieldId(fieldName);
        }

        /// <summary>
        /// Resolves a PDF field name to its Gallagher field id using an explicit certificate and timeout configuration.
        /// </summary>
        public static string ResolvePdfFieldId(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldName,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return ResolvePdfFieldId(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                fieldName,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Resolves a PDF field name to its Gallagher field id, with optional certificate and logger support.
        /// </summary>
        public static string ResolvePdfFieldId(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string fieldName,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .ResolvePdfFieldId(fieldName);
        }

        /// <summary>
        /// Resolves Gallagher's cardholder id from a Personal Data Field value using an explicit certificate and timeout configuration.
        /// </summary>
        public static string ResolveCardholderIdByPdfValue(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string pdfFieldKey,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return ResolveCardholderIdByPdfValue(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                pdfFieldKey,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Resolves Gallagher's cardholder id from a Personal Data Field value, with optional certificate and logger support.
        /// </summary>
        public static string ResolveCardholderIdByPdfValue(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string pdfFieldKey = "pdf_629",
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .ResolveCardholderIdByPdfValue(cardholderId, pdfFieldKey);
        }

        /// <summary>
        /// Alias for resolving Gallagher's cardholder id from an external cardholder id using an explicit certificate and timeout configuration.
        /// </summary>
        public static string ResolveGallagherCardholderId(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string pdfFieldKey,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return ResolveGallagherCardholderId(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                pdfFieldKey,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Alias for resolving Gallagher's cardholder id from an external cardholder id, with optional certificate and logger support.
        /// </summary>
        public static string ResolveGallagherCardholderId(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string pdfFieldKey = "pdf_629",
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .ResolveGallagherCardholderId(cardholderId, pdfFieldKey);
        }

        /// <summary>
        /// Resolves an access group name to its Gallagher id using an explicit certificate and timeout configuration.
        /// </summary>
        public static string ResolveAccessGroupIdByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return ResolveAccessGroupIdByName(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupName,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Resolves an access group name to its Gallagher id, with optional certificate and logger support.
        /// </summary>
        public static string ResolveAccessGroupIdByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .ResolveAccessGroupIdByName(accessGroupName);
        }

        /// <summary>
        /// Resolves an access group name to its Gallagher href using an explicit certificate and timeout configuration.
        /// </summary>
        public static string ResolveAccessGroupHrefByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return ResolveAccessGroupHrefByName(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupName,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Resolves an access group name to its Gallagher href, with optional certificate and logger support.
        /// </summary>
        public static string ResolveAccessGroupHrefByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .ResolveAccessGroupHrefByName(accessGroupName);
        }

        /// <summary>
        /// Alias for searching access groups by name using an explicit certificate and timeout configuration.
        /// </summary>
        public static string SearchAccessGroupsByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return SearchAccessGroupsByName(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupName,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Alias for searching access groups by name, with optional certificate and logger support.
        /// </summary>
        public static string SearchAccessGroupsByName(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupName,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .SearchAccessGroupsByName(accessGroupName);
        }

        /// <summary>
        /// Resolves the membership href linking a cardholder to an access group using an explicit certificate and timeout configuration.
        /// </summary>
        public static string ResolveAccessGroupMembershipHref(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupId,
            string cardholderId,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return ResolveAccessGroupMembershipHref(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                accessGroupId,
                cardholderId,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Resolves the membership href linking a cardholder to an access group, with optional certificate and logger support.
        /// </summary>
        public static string ResolveAccessGroupMembershipHref(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string accessGroupId,
            string cardholderId,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .ResolveAccessGroupMembershipHref(accessGroupId, cardholderId);
        }

        /// <summary>
        /// Adds an access group to a cardholder for a date range using an explicit certificate and timeout configuration.
        /// </summary>
        public static string AddAccessGroupToCardholder(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string accessGroupId,
            string fromDate,
            string untilDate,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return AddAccessGroupToCardholder(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                accessGroupId,
                fromDate,
                untilDate,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Adds an access group to a cardholder for a date range, with optional certificate and logger support.
        /// </summary>
        public static string AddAccessGroupToCardholder(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string accessGroupId,
            string fromDate,
            string untilDate,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .AddAccessGroupToCardholder(cardholderId, accessGroupId, fromDate, untilDate);
        }

        /// <summary>
        /// Converts UK date and time values (for example 17/04/26 and 10:00) into Gallagher UTC format.
        /// </summary>
        public static string ConvertUkDateAndTimeToUtcIso8601(string ukDate, string ukTime)
        {
            return GallagherDateTimeHelper.ConvertUkDateAndTimeToUtcIso8601(ukDate, ukTime);
        }

        /// <summary>
        /// Alias for converting UK date and time values into Gallagher UTC format.
        /// </summary>
        public static string ConvertUkDateAndTimeToGallagherUtc(string ukDate, string ukTime)
        {
            return ConvertUkDateAndTimeToUtcIso8601(ukDate, ukTime);
        }
        
        /// <summary>
        /// Removes an access group assignment from a cardholder using an explicit certificate and timeout configuration.
        /// </summary>
        public static string RemoveAccessGroupFromCardholder(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string membershipHref,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return RemoveAccessGroupFromCardholder(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                membershipHref,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Removes an access group assignment from a cardholder, with optional certificate and logger support.
        /// </summary>
        public static string RemoveAccessGroupFromCardholder(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string membershipHref,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .RemoveAccessGroupFromCardholder(cardholderId, membershipHref);
        }

        /// <summary>
        /// Alias for removing a cardholder from an access group using an explicit certificate and timeout configuration.
        /// </summary>
        public static string RemoveCardholderFromAccessGroup(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string membershipHref,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return RemoveCardholderFromAccessGroup(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                membershipHref,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Alias for removing a cardholder from an access group, with optional certificate and logger support.
        /// </summary>
        public static string RemoveCardholderFromAccessGroup(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string membershipHref,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .RemoveCardholderFromAccessGroup(cardholderId, membershipHref);
        }

        /// <summary>
        /// Updates an existing access group assignment using an explicit certificate and timeout configuration.
        /// </summary>
        public static string UpdateAccessGroupForCardholder(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string membershipHref,
            string fromUtc,
            string untilUtc,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return UpdateAccessGroupForCardholder(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                membershipHref,
                fromUtc,
                untilUtc,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Updates an existing access group assignment, with optional certificate and logger support.
        /// </summary>
        public static string UpdateAccessGroupForCardholder(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string membershipHref,
            string fromUtc,
            string untilUtc,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .UpdateAccessGroupForCardholder(cardholderId, membershipHref, fromUtc, untilUtc);
        }

        /// <summary>
        /// Alias for updating a cardholder access group assignment using an explicit certificate and timeout configuration.
        /// </summary>
        public static string UpdateCardholderAccessGroup(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string membershipHref,
            string fromUtc,
            string untilUtc,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds)
        {
            return UpdateCardholderAccessGroup(
                baseUrl,
                apiKeyHeaderName,
                apiKeyHeaderValue,
                cardholderId,
                membershipHref,
                fromUtc,
                untilUtc,
                certThumbprint,
                storeLocation,
                storeName,
                timeoutSeconds,
                null);
        }

        /// <summary>
        /// Alias for updating a cardholder access group assignment, with optional certificate and logger support.
        /// </summary>
        public static string UpdateCardholderAccessGroup(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string cardholderId,
            string membershipHref,
            string fromUtc,
            string untilUtc,
            string certThumbprint = null,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            int timeoutSeconds = 100,
            Action<BizTalkRestLogEntry> logger = null)
        {
            return CreateClient(baseUrl, apiKeyHeaderName, apiKeyHeaderValue, certThumbprint, storeLocation, storeName, timeoutSeconds, logger)
                .UpdateCardholderAccessGroup(cardholderId, membershipHref, fromUtc, untilUtc);
        }

        private static GallagherApiClient CreateClient(
            string baseUrl,
            string apiKeyHeaderName,
            string apiKeyHeaderValue,
            string certThumbprint,
            StoreLocation storeLocation,
            StoreName storeName,
            int timeoutSeconds,
            Action<BizTalkRestLogEntry> logger)
        {
            return new GallagherApiClient(
                baseUrl,
                new BizTalkRestClientSettings
                {
                    ApiKeyHeaderName = apiKeyHeaderName,
                    ApiKeyHeaderValue = apiKeyHeaderValue,
                    CertThumbprint = certThumbprint,
                    StoreLocation = storeLocation,
                    StoreName = storeName,
                    TimeoutSeconds = timeoutSeconds,
                    Logger = logger
                });
        }
    }
}
