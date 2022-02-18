using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateDelete
{
    public class CertificateSubmitDeleteViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public long Uln { get; set; }
        public string CertificateReference { get; set; }
        public bool? IsDeleteConfirmed { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public string IncidentNumber { get; set; }

        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);

            Uln = cert.Uln;
            StandardCode = cert.StandardCode;
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            certificate.Status = CertificateStatus.Submitted;
            certificate.CertificateData = JsonConvert.SerializeObject(data);
            
            return certificate;
        }
    }
}