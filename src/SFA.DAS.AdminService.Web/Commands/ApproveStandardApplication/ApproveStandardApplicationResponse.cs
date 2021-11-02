using System.Collections.Generic;
using ApplicationModel = SFA.DAS.AdminService.Web.Models.Apply.Application;

namespace SFA.DAS.AdminService.Web.Commands.ApproveStandardApplication
{
    public class ApproveStandardApplicationResponse
    {
        public ApplicationModel Application { get; set; }
        public string StandardDescription { get; set; }
        public string EndPointAssessorName { get; set; }
        public List<string> Versions { get; set; }
        public List<string> WarningMessages { get; set; }
        public Dictionary<string, string> ErrorMessages { get; set; }
    }
}
