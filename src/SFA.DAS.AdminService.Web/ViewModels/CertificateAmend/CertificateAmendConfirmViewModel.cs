using System;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateAmendConfirmViewModel
    {
        public Guid CertificateId { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public long Uln { get; set; }
        public int StdCode { get; set; }
        public bool BackToCheckPage { get; set; }
        public string CertificateReference { get; set; }
        public string FullName { get; set; }
    }
}