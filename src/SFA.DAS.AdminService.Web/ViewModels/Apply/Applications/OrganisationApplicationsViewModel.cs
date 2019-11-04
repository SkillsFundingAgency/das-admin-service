using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class OrganisationApplicationsViewModel
    {
        public OrganisationApplicationsViewModel()
        {
            NewOrganisationApplications = new ApplicationsViewModel();
            InProgressOrganisationApplications = new ApplicationsViewModel();
            FeedbackOrganisationApplications = new ApplicationsViewModel();
            ApprovedOrganisationApplications = new ApplicationsViewModel();
        }

        public ApplicationsViewModel NewOrganisationApplications { get; set; }

        public ApplicationsViewModel InProgressOrganisationApplications { get; set; }

        public ApplicationsViewModel FeedbackOrganisationApplications { get; set; }

        public ApplicationsViewModel ApprovedOrganisationApplications { get; set; }
    }
}
