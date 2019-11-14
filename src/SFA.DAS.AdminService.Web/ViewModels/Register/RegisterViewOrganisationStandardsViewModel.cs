using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;

namespace SFA.DAS.AdminService.Web.ViewModels.Register
{
    public class RegisterViewOrganisationStandardsViewModel
    {
        public RegisterViewOrganisationStandardsViewModel(string controllerName)
        {
            ControllerName = controllerName;
            OrganisationStandards = new OrganisationApprovedStandardsViewModel();
            InProgressApplications = new OrganisationStandardsApplicationsViewModel();
            FeedbackApplications = new OrganisationStandardsApplicationsViewModel();
        }

        public string ControllerName { get; }

        public OrganisationApprovedStandardsViewModel OrganisationStandards { get; set; }

        public OrganisationStandardsApplicationsViewModel InProgressApplications { get; set; }

        public OrganisationStandardsApplicationsViewModel FeedbackApplications { get; set; }

        public bool HasStandards
        {
            get
            {
                return OrganisationStandards.HasStandards || InProgressApplications.HasStandards || FeedbackApplications.HasStandards;
            }
        }
    }
}
