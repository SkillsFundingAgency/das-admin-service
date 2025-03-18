using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class LearnerDetailsViewModel : LearnerDetailViewModel
    {
        public string SearchString { get; set; }
        public int Page { get; set; }
        public bool ShowDetail { get; set; }
        public int? BatchNumber { get; set; }
        
        public bool CanRequestReprint => CertificateStatus.CanRequestReprintCertificate(Learner.CertificateStatus);
        public bool CanAmendCertificate => CertificateStatus.CanAmendCertificate(Learner.CertificateStatus);
        public bool CanDeleteCertificate => Learner.CertificateReference != null &&
                                            Learner.CertificateStatus != CertificateStatus.Deleted;

        public bool ShowToAdress => Learner.CertificateStatus == CertificateStatus.Submitted ||
                                    CertificateStatus.HasPrintProcessStatus(Learner.CertificateStatus);

        public string GetReasonLink(CertificateLogSummary log)
        {
            return log.Action == CertificateActions.ReprintReason || log.Action == CertificateActions.AmendReason
                ? "Show other reason"
                : "Show reason for change";
        }

        public string GetReasonHeading(CertificateLogSummary log)
        {
            return log.Action == CertificateActions.ReprintReason || log.Action == CertificateActions.AmendReason
                ? "Other reason"
                : "Reason for change";
        }

        public string ReasonForChange => GetReasonForChange(Learner.ReasonForChange);

        private string GetReasonForChange(string learnerReasonForChange)
        {
            if (string.IsNullOrWhiteSpace(learnerReasonForChange))
                return "No reason entered";
            return learnerReasonForChange;
        }
    }
}