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
}
