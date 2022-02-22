using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateGivenNamesViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);
            GivenNames = CertificateData.LearnerGivenNames;
            FullName = CertificateData.FullName;
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.LearnerGivenNames = GivenNames;
            data.FullName = $"{data.LearnerGivenNames} {data.LearnerFamilyName}";

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}
