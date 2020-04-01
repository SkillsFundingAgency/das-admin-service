
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
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, "Removed from Register of Training Organisations (RoTO) in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "Funding removed from any education bodies in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, "Removed from any professional or trade registers in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "Involuntary withdrawal from Initial Teacher Training accreditation in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "Removed from any charity register check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, "Investigated due to safeguarding issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing, "Investigated due to whistleblowing issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "Subject to insolvency or winding up proceedings in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions, "Unspent criminal convictions check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds, "Failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities, "Investigated for fraud or irregularities in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation, "Ongoing investigations for fraud or irregularities check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated, "Contract terminated by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract, "Withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments, "Breached tax payments or social security contributions in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees, "Register of Removed Trustees check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt, "People in control or any partner organisations been made bankrupt in the last 3 years check" }
        };

        public static Dictionary<string, string> NoSelectionErrorMessages = new Dictionary<string, string>
        {
            { GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "Select the outcome for composition with creditors check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "Select the outcome for failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "Select the outcome for contract terminated early by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "Select the outcome for withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, "Select the outcome for Removed from Register of Training Organisations(RoTO) in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "Select the outcome for funding removed from any education bodies in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, "Select the outcome for removed from any professional or trade registers in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "Select the outcome for Involuntary withdrawal from Initial Teacher Training accreditation in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "Select the outcome for removed from any charity register check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding,"Select the outcome for investigated due to safeguarding issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing, "Select the outcome for investigated due to whistleblowing issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "Select the outcome for subject to insolvency or winding up proceedings in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions, "Select the outcome for unspent criminal convictions check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds, "Select the outcome for failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities, "Select the outcome for investigated for fraud or irregularities in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation, "Select the outcome for ongoing investigations for fraud or irregularities check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated, "Select the outcome for contract terminated by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract, "Select the outcome for withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments, "Select the outcome for breached tax payments or social security contributions in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees, "Select the outcome for Register of Removed Trustees check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt, "Select the outcome for people in control or any partner organisations been made bankrupt in the last 3 years check" }

        };
    }
}
