namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int OrganisationApplicationsNew { get; set; }
        public int OrganisationApplicationsInProgress { get; set; }
        public int OrganisationApplicationsFeedback { get; set; }
        public int OrganisationApplicationsApproved { get; set; }

        public int StandardApplicationsNew { get; set; }
        public int StandardApplicationsInProgress { get; set; }
        public int StandardApplicationsFeedback { get; set; }
        public int StandardApplicationsApproved { get; set; }
    }
}