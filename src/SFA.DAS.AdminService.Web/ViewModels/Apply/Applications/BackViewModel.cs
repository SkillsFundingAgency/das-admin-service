namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class BackViewModel
    {
        public string BackAction { get; set; }

        public string BackController { get; set; }

        public string BackOrganisationId { get; set; }

        public BackViewModel()
        {
        }

        public BackViewModel(string backAction, string backController, string backOrganisationId)
        {
            BackAction = backAction;
            BackController = backController;
            BackOrganisationId = backOrganisationId;
        }
    }
}
