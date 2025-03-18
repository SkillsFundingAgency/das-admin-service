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

        public override Certificate GetCertificateFromViewModel(Certificate certificate)
        {
            certificate.CertificateData.LearnerFamilyName = FamilyName;
            certificate.CertificateData.FullName = $"{certificate.CertificateData.LearnerGivenNames} {certificate.CertificateData.LearnerFamilyName}";
            return certificate;
        }
    }
}
