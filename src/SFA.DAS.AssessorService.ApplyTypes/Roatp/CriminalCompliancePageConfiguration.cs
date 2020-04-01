using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public static class CriminalCompliancePageConfiguration
    {
        public static Dictionary<string, string> Headings = new Dictionary<string, string>
        {
            { GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "Composition with creditors check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "Failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "Contract terminated early by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "Withdrawn from a contract with a public body in the last 3 years check" },

            // TODO Not Assigned
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Roto, "Roto" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "FundingRemoved" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedProfessionalRegister, "RemovedProfessionalRegister" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "IttAccreditation" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "RemovedCharityRegister" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, "Safeguarding" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing, "Whistleblowing" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "Insolvency" }
        };

        public static Dictionary<string, string> NoSelectionErrorMessages = new Dictionary<string, string>
        {
            { GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "Select the outcome for composition with creditors check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "Select the outcome for failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "Select the outcome for contract terminated early by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "Select the outcome for withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Roto, "Select the outcome for Removed from Register of Training Organisations(RoTO) in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "Select the outcome for funding removed from any education bodies in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedProfessionalRegister, "Select the outcome for removed from any professional or trade registers in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "Select the outcome for Involuntary withdrawal from Initial Teacher Training accreditation in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "Select the outcome for removed from any charity register check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding,"Select the outcome for investigated due to safeguarding issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing, "Select the outcome for investigated due to whistleblowing issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "Select the outcome for subject to insolvency or winding up proceedings in the last 3 years check" }
        };
    }
}