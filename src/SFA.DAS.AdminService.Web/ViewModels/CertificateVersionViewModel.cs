using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class CertificateVersionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public IEnumerable<StandardVersion> Standards { get; set; }
        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certificate.StandardUId = StandardUId;
            certData.Version = Version;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            return certificate;
        }
    }

}
