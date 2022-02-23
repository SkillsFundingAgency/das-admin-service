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

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certData.LearnerGivenNames = GivenNames;
            certData.FullName = $"{certData.LearnerGivenNames} {certData.LearnerFamilyName}";

            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            return certificate;
        }
    }
}
