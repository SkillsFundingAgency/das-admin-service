using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class UpdateReprintReasonViewModel
    {
        public List<string> SelectedReprintReasons { get; set; }
        public string TicketNumber { get; set; }
        public string OtherReason { get; set; }
    }
}
