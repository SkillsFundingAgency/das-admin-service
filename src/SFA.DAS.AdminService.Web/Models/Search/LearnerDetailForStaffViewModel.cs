using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AdminService.Web.Models.Search
{
    public class LearnerDetailForStaffViewModel
    {
        public LearnerDetailResult Learner { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public bool ShowDetail { get; set; }
        public int? BatchNumber { get; set; }
        public bool CanRequestDuplicate => CertificateStatus.CanRequestDuplicateCertificate(Learner.CertificateStatus);
        public bool CanAmendCertificate => CertificateStatus.CanAmendCertificate(Learner.CertificateStatus);
        public bool CanDeleteCertificate => Learner.CertificateReference != null && Learner.CertificateStatus != CertificateStatus.Deleted;
        public string DateStatusTitle => GetDateStatusTitle(Learner.CertificateStatus);

        private string GetDateStatusTitle(string learnerCertificateStatus)
        {
            var dateTitle = "Date ";
            switch (learnerCertificateStatus)
            {
                case "Submitted":
                    return dateTitle + "submitted";
                case "SentToPrinter":
                    return dateTitle + "sent to printer";
                case "Printed":
                    return dateTitle + "printed";
                case "NotDelivered":
                    return dateTitle + "delivery attempted";
                case "Delivered":
                    return dateTitle + "delivered";
                case "Deleted":
                    return dateTitle + "deleted";
            }
            return string.Empty;
        }
    }
}