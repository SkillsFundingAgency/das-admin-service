
namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public static class CriminalCompliancePageTitles
    {
        public static string OrganisationCompositionCreditors = "Composition with creditors check";
        public static string OrganisationFailedToRepayFunds = "Failed to pay back funds in the last 3 years check";
        public static string OrganisationContractTerminatedByPublicBody = "Contract terminated early by a public body in the last 3 years check";
        public static string OrganisationContractWithdrawnEarly = "Withdrawn from a contract with a public body in the last 3 years check";
    }

    public static class CriminalCompliancePagePostActions
    {
        public static string OrganisationCompositionCreditors = "EvaluateOrganisationCompositionCreditorsPage";
        public static string OrganisationFailedToRepayFunds = "EvaluateOrganisationFailedToRepayFundsPage";
        public static string OrganisationContractTerminatedByPublicBody = "EvaluateOrganisationContractTerminationPage";
        public static string OrganisationContractWithdrawnEarly = "EvaluateOrganisationContractWithdrawnPage";
    }
}
