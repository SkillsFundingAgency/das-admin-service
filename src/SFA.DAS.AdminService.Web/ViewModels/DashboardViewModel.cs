namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int OrganisationApplicationsNew { get; set; }
        public int OrganisationApplicationsInProgress { get; set; }
        public int OrganisationApplicationsHasFeedback { get; set; }
        public int OrganisationApplicationsApproved { get; set; }

        public int StandardApplicationsNew { get; set; }
        public int StandardApplicationsInProgress { get; set; }
        public int StandardApplicationsHasFeedback { get; set; }
        public int StandardApplicationsApproved { get; set; }

        public int WithdrawalApplicationsNew { get; set; }
        public int WithdrawalApplicationsInProgress { get; set; }
        public int WithdrawalApplicationsHasFeedback { get; set; }
        public int WithdrawalApplicationsApproved { get; set; }

        public string RoatpOversightBaseUrl { get; set; }
        public string RoatpAssessorBaseUrl { get; set; }
        public string RoatpGatewayBaseUrl { get; set; }
        public string RoatpFinanceBaseUrl { get; set; }
        public string RoatpProviderModerationBaseUrl { get; set; }
    }
}