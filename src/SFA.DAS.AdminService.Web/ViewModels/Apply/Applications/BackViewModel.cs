namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class BackViewModel
    {
        public string BackAction { get; }

        public string BackController { get; }

        public string BackOrganisationId { get; }

        public BackViewModel(string backAction, string backController, string backOrganisationId)
        {
            BackAction = backAction;
            BackController = backController;
            BackOrganisationId = backOrganisationId;
        }
    }
}
