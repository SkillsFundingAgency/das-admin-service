using EnumsNET;
using SFA.DAS.AssessorService.Api.Types.Enums;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkLearnerReprintReasonViewModel
    {
        public string CertificateNumber { get; set; }
        public string ApprenticeName { get; set; }
        public bool HasPreviousReprint { get; set; }
        public string Status { get; set; }
        public System.DateTime DateSentToPrinter { get; set; }
        public string TicketNumber { get; set; }
        public List<string> SelectedReprintReasons { get; set; }
        public string OtherReason { get; set; }
        public bool BackToCheckAnswers { get; set; }
        public string BackAction { get; set; }}

        public string GetReprintReasonDescription(ReprintReasons reprintReason)
        {
            return reprintReason.AsString(EnumFormat.Description);
        }
    }
}
