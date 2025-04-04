using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class StandardLearnerDetailsViewModel : LearnerDetailViewModel
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
    }
}