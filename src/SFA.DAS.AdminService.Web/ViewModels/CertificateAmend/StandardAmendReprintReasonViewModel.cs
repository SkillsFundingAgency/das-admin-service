using EnumsNET;
using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Enums;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class AmendStandardReprintReasonViewModel : LearnerDetailViewModel
    {
        public string IncidentNumber { get; set; }
        public List<string> Reasons { get; set; }
        public string OtherReason { get; set; }

        public string GetAmendReasonDescription(AmendReasons amendReason)
        {
            return amendReason.AsString(EnumFormat.Description);
        }
    }
}
