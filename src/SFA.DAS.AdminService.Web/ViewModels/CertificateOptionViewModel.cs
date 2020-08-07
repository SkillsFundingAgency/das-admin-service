using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class CertificateOptionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public bool HasAdditionalLearningOption { get; set; }
        public string Option { get; set; }
        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            Option = CertificateData.CourseOption;
            HasAdditionalLearningOption = !string.IsNullOrWhiteSpace(Option);
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.CourseOption = HasAdditionalLearningOption ? Option : string.Empty;
            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}