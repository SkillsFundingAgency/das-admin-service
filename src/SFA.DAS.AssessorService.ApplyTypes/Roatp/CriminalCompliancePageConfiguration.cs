
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public static class CriminalCompliancePageConfiguration
    {
        public static Dictionary<string, string> Titles = new Dictionary<string, string>
        {
            { GatewayPageIds.CCOrganisationCompositionCreditors, "Composition with creditors check" },
            { GatewayPageIds.CCOrganisationFailedToRepayFunds, "Failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CCOrganisationContractTermination, "Contract terminated early by a public body in the last 3 years check" },
            { GatewayPageIds.CCOrganisationContractWithdrawnEarly, "Withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CCOrganisationRemovedRoTO, "Removed from Register of Training Organisations (RoTO) in the last 3 years check" },
            { GatewayPageIds.CCOrganisationFundingRemoved, "Funding removed from any education bodies in the last 3 years check" }
        };
    }
}
