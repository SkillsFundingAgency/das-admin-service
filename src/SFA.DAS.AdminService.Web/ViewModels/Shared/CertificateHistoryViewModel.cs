using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;

namespace SFA.DAS.AdminService.Web.ViewModels.Shared
{
    public abstract class CertificateHistoryViewModel
    {
        public static string GetReasonLink(CertificateLogSummary log)
        {
            return log.Action == CertificateActions.ReprintReason || log.Action == CertificateActions.AmendReason
                ? "Show other reason"
                : "Show reason for change";
        }

        public static string GetReasonHeading(CertificateLogSummary log)
        {
            return log.Action == CertificateActions.ReprintReason || log.Action == CertificateActions.AmendReason
                ? "Other reason"
                : "Reason for change";
        }

        protected static string GetReasonForChange(string learnerReasonForChange)
        {
            if (string.IsNullOrWhiteSpace(learnerReasonForChange))
                return "No reason entered";
            return learnerReasonForChange;
        }
    }
}
