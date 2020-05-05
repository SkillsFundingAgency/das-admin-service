using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationsDashboardViewModel
    {
        public ApplicationsDashboardViewModel(string controllerName)
        {
            ControllerName = controllerName;
            NewApplications = new ApplicationsViewModel();
            InProgressApplications = new ApplicationsViewModel();
            FeedbackApplications = new ApplicationsViewModel();
            ApprovedApplications = new ApplicationsViewModel();
        }

        public string ControllerName { get; }

        public ApplicationsViewModel NewApplications { get; set; }

        public ApplicationsViewModel InProgressApplications { get; set; }

        public ApplicationsViewModel FeedbackApplications { get; set; }

        public ApplicationsViewModel ApprovedApplications { get; set; }
    }
}
