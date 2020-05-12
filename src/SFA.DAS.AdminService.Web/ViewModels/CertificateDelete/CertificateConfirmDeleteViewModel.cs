using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateDelete
{
    public class CertificateConfirmDeleteViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public long Uln { get; set; }
        public string CertificateReference { get; set; }
        public int StandardCode { get; set; }
        public string IncidentNumber { get; set; }
        public string UserName { get; set; }
        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);

            Uln = cert.Uln;
            CertificateReference = cert.CertificateReference;
            StandardCode = cert.StandardCode;
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            certificate.Status = CertificateStatus.Submitted;
            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}