using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class CertificateVersionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public List<StandardVersion> Standards { get; set; }
        public string SelectedVersion { get; set; }
        public int StandardCode { get; set; }

        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            StandardCode = cert.StandardCode;
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certData.Version = SelectedVersion;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            return certificate;
        }
    }

}
