using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateFamilyNameViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);
            FamilyName = CertificateData.LearnerFamilyName;
            FullName = CertificateData.FullName;
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certData.LearnerFamilyName = FamilyName;
            certData.FullName = $"{certData.LearnerGivenNames} {certData.LearnerFamilyName}";

            certificate.CertificateData = certData;
            return certificate;
        }
    }
}
