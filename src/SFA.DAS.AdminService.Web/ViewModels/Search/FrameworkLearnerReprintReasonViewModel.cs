using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkLearnerReprintReasonViewModel
    {
        public IEnumerable<string> AvailableReprintReasons { get; }
        public string CertificateNumber { get; set; }
        public string ApprenticeName { get; set; }
        public bool HasPreviousReprint { get; set; }
        public string Status { get; set; }
        public System.DateTime DateSentToPrinter { get; set; }
        public string TicketNumber { get; set; }
        public List<string> SelectedReprintReasons { get; set; }
        public string OtherReason { get; set; }
        public FrameworkLearnerReprintReasonViewModel()
        {
            AvailableReprintReasons = new List<string>
            { 
                "Delivery failed",
                "Apprentice moved to different employer or no longer with employer",
                "Incorrect apprentice details",
                "Incorrect employer address",
                "Incorrect apprenticeship details",
                "Lost or damaged by receiver",
                "Other"
            };
        }
    }
}
