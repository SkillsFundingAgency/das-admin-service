using System.Text.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateVersionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public IEnumerable<StandardVersion> Standards { get; set; }
        
        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certificate.StandardUId = StandardUId;
            certData.Version = Version;
            certificate.CertificateData = JsonSerializer.Serialize(certData);
            
            return certificate;
        }
    }

}
