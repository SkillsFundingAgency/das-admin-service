using SFA.DAS.AssessorService.Domain.Consts;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateReprintConfirmViewModel
    {
        public Guid CertificateId { get; set; }
        public string NextBatchDate { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public long Uln { get; set; }
        public int StdCode { get; set; }
        public bool BackToCheckPage { get; set; }
        public string CertificateReference { get; set; }
        public string Status { get; set; }
        public string PrivatelyFundedStatus { get; set; }
        public string FullName { get; set; }

        public bool CanRequestReprint => CertificateStatus.CanRequestReprintCertificate(Status);
    }
}