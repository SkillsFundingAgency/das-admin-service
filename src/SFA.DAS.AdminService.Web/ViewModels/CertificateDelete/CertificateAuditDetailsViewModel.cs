using System.Text.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateDelete
{
    public class CertificateAuditDetailsViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public long Uln { get; set; }
        public string CertificateReference { get; set; }
        public string IncidentNumber { get; set; }
        public bool? IsDeleteConfirmed { get; set; }

        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);

            Uln = cert.Uln;
            CertificateReference = cert.CertificateReference;
            StandardCode = cert.StandardCode;
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            certificate.Status = CertificateStatus.Submitted;
            certificate.CertificateData = JsonSerializer.Serialize(data);
            
            return certificate;
        }
    }
}