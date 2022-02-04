namespace SFA.DAS.AdminService.Web.Models.Merge
{
    public class SessionCommand
    {
        public string CommandName { get; set; }
        public int Order { get; set; }
        public string SearchString { get; set; }
        public string EpaoId { get; set; }

        public SessionCommand(string command, string searchString, string epaoId)
        {
            CommandName = command;
            SearchString = searchString;
            EpaoId = epaoId;
        }
    }
}
