using System;

namespace Bham.BizTalk.Rest.Tests
{
    /// <summary>
    /// Minimal in-project test runner that executes the lightweight regression tests without an external test host.
    /// </summary>
    public static class TestRunner
    {
        /// <summary>
        /// Executes all registered regression tests and throws if any individual test fails.
        /// </summary>
        public static void RunAll()
        {
            Run(nameof(BizTalkRestClientTests.BuildUrl_AppendsEncodedQueryParameters), BizTalkRestClientTests.BuildUrl_AppendsEncodedQueryParameters);
            Run(nameof(BizTalkRestClientTests.BuildUrl_AppendsWithAmpersand_WhenBaseAlreadyHasQuery), BizTalkRestClientTests.BuildUrl_AppendsWithAmpersand_WhenBaseAlreadyHasQuery);
            Run(nameof(BizTalkRestClientTests.GetJson_ThrowsArgumentException_WhenUrlIsNotHttpOrHttps), BizTalkRestClientTests.GetJson_ThrowsArgumentException_WhenUrlIsNotHttpOrHttps);
            Run(nameof(GallagherApiClientTests.BuildQuotedQueryValue_WrapsAndTrimsValue), GallagherApiClientTests.BuildQuotedQueryValue_WrapsAndTrimsValue);
            Run(nameof(GallagherApiClientTests.Constructor_ThrowsArgumentException_WhenBaseUrlIsNotHttpOrHttps), GallagherApiClientTests.Constructor_ThrowsArgumentException_WhenBaseUrlIsNotHttpOrHttps);
            Run(nameof(GallagherApiClientTests.GetCardholderById_ThrowsArgumentNullException_WhenIdMissing), GallagherApiClientTests.GetCardholderById_ThrowsArgumentNullException_WhenIdMissing);
            Run(nameof(GallagherApiClientTests.GetAccessGroupById_ThrowsArgumentNullException_WhenIdMissing), GallagherApiClientTests.GetAccessGroupById_ThrowsArgumentNullException_WhenIdMissing);
            Run(nameof(GallagherApiClientTests.GetPersonalDataFieldById_ThrowsArgumentNullException_WhenIdMissing), GallagherApiClientTests.GetPersonalDataFieldById_ThrowsArgumentNullException_WhenIdMissing);
            Run(nameof(GallagherApiClientTests.GetCardholderAccessGroups_ThrowsArgumentNullException_WhenIdMissing), GallagherApiClientTests.GetCardholderAccessGroups_ThrowsArgumentNullException_WhenIdMissing);
            Run(nameof(GallagherApiFacadeTests.GetCardholderById_ThrowsArgumentNullException_WhenIdMissing), GallagherApiFacadeTests.GetCardholderById_ThrowsArgumentNullException_WhenIdMissing);
            Run(nameof(GallagherApiFacadeTests.GetCardholders_ThrowsArgumentException_WhenBaseUrlIsNotHttpOrHttps), GallagherApiFacadeTests.GetCardholders_ThrowsArgumentException_WhenBaseUrlIsNotHttpOrHttps);
            Run(nameof(GallagherApiFacadeTests.GetCardholders_ThrowsArgumentNullException_WhenHeaderNameMissing), GallagherApiFacadeTests.GetCardholders_ThrowsArgumentNullException_WhenHeaderNameMissing);
            Run(nameof(GallagherApiClientTests.BuildAddAccessGroupPatchBody_MatchesGallagherShape), GallagherApiClientTests.BuildAddAccessGroupPatchBody_MatchesGallagherShape);
            Run(nameof(GallagherApiClientTests.BuildRemoveAccessGroupPatchBody_MatchesGallagherShape), GallagherApiClientTests.BuildRemoveAccessGroupPatchBody_MatchesGallagherShape);
            Run(nameof(GallagherApiClientTests.BuildUpdateAccessGroupPatchBody_MatchesGallagherShape), GallagherApiClientTests.BuildUpdateAccessGroupPatchBody_MatchesGallagherShape);
            Run(nameof(GallagherApiResponseParserTests.GetFirstEntityId_FindsFirstNestedId), GallagherApiResponseParserTests.GetFirstEntityId_FindsFirstNestedId);
            Run(nameof(GallagherApiResponseParserTests.GetEntityIdByName_FindsMatchingName), GallagherApiResponseParserTests.GetEntityIdByName_FindsMatchingName);
            Run(nameof(GallagherApiResponseParserTests.GetAccessGroupMembershipHrefForCardholder_FindsMembershipHref), GallagherApiResponseParserTests.GetAccessGroupMembershipHrefForCardholder_FindsMembershipHref);
            Run(nameof(GallagherApiResponseParserTests.GetAccessGroupMembershipHrefByNameAndDates_FindsMatchingRecord), GallagherApiResponseParserTests.GetAccessGroupMembershipHrefByNameAndDates_FindsMatchingRecord);
            Run(nameof(GallagherApiResponseParserTests.TryGetAccessGroupMembershipHrefByNameAndDates_AllowsNameOnlyFilter), GallagherApiResponseParserTests.TryGetAccessGroupMembershipHrefByNameAndDates_AllowsNameOnlyFilter);
            Run(nameof(SmokeTestWorkflowOptionTests.Validate_BuildsPdfFieldKeyFromPdfFieldId), SmokeTestWorkflowOptionTests.Validate_BuildsPdfFieldKeyFromPdfFieldId);
            Run(nameof(SmokeTestWorkflowOptionTests.ApplyNamedArguments_AcceptsLegacyPdfValueAlias), SmokeTestWorkflowOptionTests.ApplyNamedArguments_AcceptsLegacyPdfValueAlias);
            Run(nameof(SmokeTestWorkflowOptionTests.ApplyNamedArguments_PreservesGallagherCardholderId_WhenProvided), SmokeTestWorkflowOptionTests.ApplyNamedArguments_PreservesGallagherCardholderId_WhenProvided);
            Run(nameof(SmokeTestWorkflowOptionTests.LoadFromJsonFile_ReadsGallagherCardholderIdSample), SmokeTestWorkflowOptionTests.LoadFromJsonFile_ReadsGallagherCardholderIdSample);
        }

        private static void Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine("PASS: " + name);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("FAIL: " + name, ex);
            }
        }
    }
}
