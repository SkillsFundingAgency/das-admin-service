using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Commands.ApproveStandardApplication
{
    public class ApproveStandardApplicationResponse
    {
        public Application Application { get; set; }
        public string StandardDescription { get; set; }
        public string EndPointAssessorName { get; set; }
        public List<string> Versions { get; set; }
        public List<string> WarningMessages { get; set; }
    }
}
