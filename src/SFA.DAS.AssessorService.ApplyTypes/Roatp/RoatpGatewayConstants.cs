using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public static class RoatpGatewayConstants
    {
        public const string ProviderContactDetailsTypeLegalIdentifier = "L";

        public static class Captions
        {
            public static string OrganisationChecks = "Organisation checks";
        }

        public static class Headings
        {
            public static string OrganisationStatusCheck = "Organisation status check";
            public static string AddressCheck = "Address check";
            public static string IcoNumber = "Information Commissioner's Office (ICO) registration number check";
            public static string Website = "Website address check";
            public static string OrganisationRisk = "Organisation high risk check";
        }
    }

    public static class NoSelectionErrorMessages
    {
        public static string LegalName = "Select the outcome for legal name check";
        public static string TradingName = "Select the outcome for trading name check";
        public static string OrganisationStatusCheck = "Select the outcome for organisation status check";
        public static string AddressCheck = "Select the outcome for address check";
        public static string IcoNumber = "Select the outcome for ICO registration number check";
        public static string Website = "Select the outcome for website address check";
        public static string OrganisationRisk = "Select the outcome for organisation high risk check";
    }
}
