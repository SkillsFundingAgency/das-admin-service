using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class CertificateVersionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string SelectedVersion { get; set; }


        public int StandardCode { get; set; }

        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            StandardCode = cert.StandardCode;
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.Version = SelectedVersion;
            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }

}
