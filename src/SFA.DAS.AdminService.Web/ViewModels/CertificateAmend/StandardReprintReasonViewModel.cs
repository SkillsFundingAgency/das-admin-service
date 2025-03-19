using EnumsNET;
using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Domain.Consts;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class StandardReprintReasonViewModel : LearnerDetailViewModel
    {
        public string IncidentNumber { get; set; }
        public List<string> Reasons { get; set; }
        public string OtherReason { get; set; }

        public bool ShowWarning => Learner.CertificateStatus == CertificateStatus.SentToPrinter
            || Learner.CertificateStatus == CertificateStatus.Printed
            || Learner.CertificateStatus == CertificateStatus.Reprint;

        public string GetReprintReasonDescription(ReprintReasons reprintReason)
        {
            return reprintReason.AsString(EnumFormat.Description);
        }
    }
}
