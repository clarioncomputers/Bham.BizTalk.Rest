using System;
using System.Collections.Generic;
using System.Text;

namespace Bham.BizTalk.Rest
{
    /// <summary>
    /// Gallagher-specific wrapper over the generic BizTalk REST helper.
    /// Keeps endpoint paths, quoted filter values, and PATCH body shapes in one place.
    /// </summary>
    public sealed class GallagherApiClient
    {
        private readonly string _baseUrl;
        private readonly BizTalkRestClient _client;

        /// <summary>
        /// Creates a Gallagher client from a base URL and REST client settings.
        /// </summary>
        public GallagherApiClient(string baseUrl, BizTalkRestClientSettings settings)
            : this(baseUrl, CreateClient(settings))
        {
        }

        /// <summary>
        /// Creates a Gallagher client from a base URL and an already configured REST client.
        /// </summary>
        public GallagherApiClient(string baseUrl, BizTalkRestClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            ValidateAbsoluteHttpUrl(baseUrl, nameof(baseUrl));

            _baseUrl = baseUrl.TrimEnd('/');
            _client = client;
        }

        /// <summary>
        /// Gets Gallagher personal data fields filtered by field name.
        /// </summary>
        public string GetPersonalDataFieldsByName(string fieldName)
        {
            return _client.GetJson(
                CombineUrl("personal_data_fields"),
                new Dictionary<string, string>
                {
                    { "name", BuildQuotedQueryValue(fieldName) }
                });
        }

        /// <summary>
        /// Gets all Gallagher personal data fields.
        /// </summary>
        public string GetPersonalDataFields()
        {
            return _client.GetJson(CombineUrl("personal_data_fields"));
        }

        /// <summary>
        /// Gets one Gallagher personal data field by id.
        /// </summary>
        public string GetPersonalDataFieldById(string fieldId)
        {
            if (string.IsNullOrWhiteSpace(fieldId)) throw new ArgumentNullException(nameof(fieldId));
            return _client.GetJson(CombineUrl("personal_data_fields/" + EncodePathSegment(fieldId)));
        }

        /// <summary>
        /// Searches Gallagher cardholders by Personal Data Field value.
        /// </summary>
        public string GetCardholdersByPdfValue(string cardholderId, string pdfFieldKey = "pdf_629")
        {
            if (string.IsNullOrWhiteSpace(pdfFieldKey)) throw new ArgumentNullException(nameof(pdfFieldKey));

            return _client.GetJson(
                CombineUrl("cardholders"),
                new Dictionary<string, string>
                {
                    { pdfFieldKey, BuildQuotedQueryValue(cardholderId) }
                });
        }

        /// <summary>
        /// Gets all Gallagher cardholders.
        /// </summary>
        public string GetCardholders()
        {
            return _client.GetJson(CombineUrl("cardholders"));
        }

        /// <summary>
        /// Gets one Gallagher cardholder by Gallagher's own id.
        /// </summary>
        public string GetCardholderById(string cardholderId)
        {
            if (string.IsNullOrWhiteSpace(cardholderId)) throw new ArgumentNullException(nameof(cardholderId));
            return _client.GetJson(CombineUrl("cardholders/" + EncodePathSegment(cardholderId)));
        }

        /// <summary>
        /// Gets all Gallagher access groups.
        /// </summary>
        public string GetAccessGroups()
        {
            return _client.GetJson(CombineUrl("access_groups"));
        }

        /// <summary>
        /// Gets one Gallagher access group by id.
        /// </summary>
        public string GetAccessGroupById(string accessGroupId)
        {
            if (string.IsNullOrWhiteSpace(accessGroupId)) throw new ArgumentNullException(nameof(accessGroupId));
            return _client.GetJson(CombineUrl("access_groups/" + EncodePathSegment(accessGroupId)));
        }

        /// <summary>
        /// Searches Gallagher access groups by name.
        /// </summary>
        public string FindAccessGroupsByName(string accessGroupName)
        {
            return _client.GetJson(
                CombineUrl("access_groups"),
                new Dictionary<string, string>
                {
                    { "name", BuildQuotedQueryValue(accessGroupName) }
                });
        }

        /// <summary>
        /// Gets the cardholders assigned to a Gallagher access group.
        /// </summary>
        public string GetAccessGroupCardholders(string accessGroupId)
        {
            if (string.IsNullOrWhiteSpace(accessGroupId)) throw new ArgumentNullException(nameof(accessGroupId));
            return _client.GetJson(CombineUrl("access_groups/" + EncodePathSegment(accessGroupId) + "/cardholders"));
        }

        /// <summary>
        /// Gets the access groups assigned to a Gallagher cardholder.
        /// </summary>
        public string GetCardholderAccessGroups(string cardholderId)
        {
            if (string.IsNullOrWhiteSpace(cardholderId)) throw new ArgumentNullException(nameof(cardholderId));
            return _client.GetJson(CombineUrl("cardholders/" + EncodePathSegment(cardholderId) + "/access_groups"));
        }

        /// <summary>
        /// Resolves a personal data field name to its Gallagher id.
        /// </summary>
        public string ResolvePersonalDataFieldId(string fieldName)
        {
            return GallagherApiResponseParser.GetEntityIdByName(GetPersonalDataFieldsByName(fieldName), fieldName);
        }

        /// <summary>
        /// Alias for resolving a Personal Data Field id used as a PDF field.
        /// </summary>
        public string ResolvePdfFieldId(string fieldName)
        {
            return ResolvePersonalDataFieldId(fieldName);
        }

        /// <summary>
        /// Resolves Gallagher's cardholder id by searching a Personal Data Field value.
        /// </summary>
        public string ResolveCardholderIdByPdfValue(string cardholderId, string pdfFieldKey = "pdf_629")
        {
            return GallagherApiResponseParser.GetFirstEntityId(GetCardholdersByPdfValue(cardholderId, pdfFieldKey));
        }

        /// <summary>
        /// Alias for resolving Gallagher's cardholder id from an external cardholder id value.
        /// </summary>
        public string ResolveGallagherCardholderId(string cardholderId, string pdfFieldKey = "pdf_629")
        {
            return ResolveCardholderIdByPdfValue(cardholderId, pdfFieldKey);
        }

        /// <summary>
        /// Resolves a Gallagher access group name to its id.
        /// </summary>
        public string ResolveAccessGroupIdByName(string accessGroupName)
        {
            return GallagherApiResponseParser.GetEntityIdByName(FindAccessGroupsByName(accessGroupName), accessGroupName);
        }

        /// <summary>
        /// Alias for searching Gallagher access groups by name.
        /// </summary>
        public string SearchAccessGroupsByName(string accessGroupName)
        {
            return FindAccessGroupsByName(accessGroupName);
        }

        /// <summary>
        /// Resolves the membership href linking a cardholder to an access group.
        /// </summary>
        public string ResolveAccessGroupMembershipHref(string accessGroupId, string cardholderId)
        {
            try
            {
                return GallagherApiResponseParser.GetAccessGroupMembershipHrefForCardholder(GetAccessGroupCardholders(accessGroupId), cardholderId);
            }
            catch (BizTalkRestClientException ex) when (ex.StatusCode == 404)
            {
                // Access group does not exist, so no membership exists
                return string.Empty;
            }
        }

        /// <summary>
        /// Adds an access group membership to a cardholder for the supplied date range.
        /// </summary>
        public string AddAccessGroupToCardholder(string cardholderId, string accessGroupId, string fromDate, string untilDate)
        {
            var url = CombineUrl("cardholders/" + EncodePathSegment(cardholderId));
            var body = BuildAddAccessGroupPatchBody(CombineUrl("access_groups/" + EncodePathSegment(accessGroupId)), fromDate, untilDate);
            return _client.PatchJson(url, body);
        }

        /// <summary>
        /// Removes an access group membership from a cardholder using the membership href.
        /// </summary>
        public string RemoveAccessGroupFromCardholder(string cardholderId, string membershipHref)
        {
            var url = CombineUrl("cardholders/" + EncodePathSegment(cardholderId));
            var body = BuildRemoveAccessGroupPatchBody(membershipHref);
            return _client.PatchJson(url, body);
        }

        /// <summary>
        /// Alias for removing an access group membership from a cardholder.
        /// </summary>
        public string RemoveCardholderFromAccessGroup(string cardholderId, string membershipHref)
        {
            return RemoveAccessGroupFromCardholder(cardholderId, membershipHref);
        }

        /// <summary>
        /// Updates the dates on an existing access group membership.
        /// </summary>
        public string UpdateAccessGroupForCardholder(string cardholderId, string membershipHref, string fromUtc, string untilUtc)
        {
            var url = CombineUrl("cardholders/" + EncodePathSegment(cardholderId));
            var body = BuildUpdateAccessGroupPatchBody(membershipHref, fromUtc, untilUtc);
            return _client.PatchJson(url, body);
        }

        /// <summary>
        /// Alias for updating the dates on an existing access group membership.
        /// </summary>
        public string UpdateCardholderAccessGroup(string cardholderId, string membershipHref, string fromUtc, string untilUtc)
        {
            return UpdateAccessGroupForCardholder(cardholderId, membershipHref, fromUtc, untilUtc);
        }

        /// <summary>
        /// Wraps a query value in quotes to match Gallagher's expected filter format.
        /// </summary>
        public static string BuildQuotedQueryValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            return "\"" + value.Trim() + "\"";
        }

        /// <summary>
        /// Builds the JSON PATCH body used to add an access group membership.
        /// </summary>
        public static string BuildAddAccessGroupPatchBody(string accessGroupHref, string fromDate, string untilDate)
        {
            ValidateAbsoluteHttpUrl(accessGroupHref, nameof(accessGroupHref));
            if (string.IsNullOrWhiteSpace(fromDate)) throw new ArgumentNullException(nameof(fromDate));
            if (string.IsNullOrWhiteSpace(untilDate)) throw new ArgumentNullException(nameof(untilDate));

            return "{"
                + "\"accessGroups\":{"
                + "\"add\":[{"
                + "\"accessGroup\":{\"href\":\"" + EscapeJson(accessGroupHref) + "\"},"
                + "\"from\":\"" + EscapeJson(fromDate.Trim()) + "\","
                + "\"until\":\"" + EscapeJson(untilDate.Trim()) + "\""
                + "}]}"
                + "}";
        }

        /// <summary>
        /// Builds the JSON PATCH body used to remove an access group membership.
        /// </summary>
        public static string BuildRemoveAccessGroupPatchBody(string membershipHref)
        {
            ValidateAbsoluteHttpUrl(membershipHref, nameof(membershipHref));

            return "{"
                + "\"accessGroups\":{"
                + "\"remove\":[{"
                + "\"href\":\"" + EscapeJson(membershipHref) + "\""
                + "}]}"
                + "}";
        }

        /// <summary>
        /// Builds the JSON PATCH body used to update an existing access group membership.
        /// </summary>
        public static string BuildUpdateAccessGroupPatchBody(string membershipHref, string fromUtc, string untilUtc)
        {
            ValidateAbsoluteHttpUrl(membershipHref, nameof(membershipHref));
            if (string.IsNullOrWhiteSpace(fromUtc)) throw new ArgumentNullException(nameof(fromUtc));
            if (string.IsNullOrWhiteSpace(untilUtc)) throw new ArgumentNullException(nameof(untilUtc));

            return "{"
                + "\"accessGroups\":{"
                + "\"update\":[{"
                + "\"href\":\"" + EscapeJson(membershipHref) + "\","
                + "\"from\":\"" + EscapeJson(fromUtc.Trim()) + "\","
                + "\"until\":\"" + EscapeJson(untilUtc.Trim()) + "\""
                + "}]}"
                + "}";
        }

        private string CombineUrl(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentNullException(nameof(relativePath));
            return _baseUrl + "/" + relativePath.TrimStart('/');
        }

        private static BizTalkRestClient CreateClient(BizTalkRestClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            return new BizTalkRestClient(settings);
        }

        private static string EncodePathSegment(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            return Uri.EscapeDataString(value.Trim());
        }

        private static void ValidateAbsoluteHttpUrl(string url, string parameterName)
        {
            Uri parsed;
            if (!Uri.TryCreate(url, UriKind.Absolute, out parsed) ||
                (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("URL must be an absolute http or https URI.", parameterName);
            }
        }

        private static string EscapeJson(string value)
        {
            var builder = new StringBuilder(value.Length + 8);

            for (var i = 0; i < value.Length; i++)
            {
                var current = value[i];
                switch (current)
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        if (current < 32)
                        {
                            builder.Append("\\u");
                            builder.Append(((int)current).ToString("x4"));
                        }
                        else
                        {
                            builder.Append(current);
                        }

                        break;
                }
            }

            return builder.ToString();
        }
    }
}