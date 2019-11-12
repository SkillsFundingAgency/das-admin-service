using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;

namespace SFA.DAS.AdminService.Web.ViewModels.Register
{
    public class RegisterViewOrganisationStandardsViewModel
    {
        public RegisterViewOrganisationStandardsViewModel(string controllerName)
        {
            ControllerName = controllerName;
            OrganisationStandards = new OrganisationStandardsViewModel();
            InProgressApplications = new ApplicationsViewModel();
            FeedbackApplications = new ApplicationsViewModel();
        }

        public string ControllerName { get; }

        public OrganisationStandardsViewModel OrganisationStandards { get; set; }

        public ApplicationsViewModel InProgressApplications { get; set; }

        public ApplicationsViewModel FeedbackApplications { get; set; }

        public bool HasStandards
        {
            get
            {
                return
                    OrganisationStandards.PaginationViewModel.PaginatedList.TotalRecordCount > 0 ||
                    InProgressApplications.PaginatedList.TotalRecordCount > 0 ||
                    FeedbackApplications.PaginatedList.TotalRecordCount > 0;

            }
        }
    }
}
