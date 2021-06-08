using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class CertificateOptionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public bool HasAdditionalLearningOption { get; set; }
        public string Option { get; set; }
        public string SelectedOption { get; set; }
        public IEnumerable<string> Options { get; set; }
        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            Option = CertificateData.CourseOption;
            HasAdditionalLearningOption = !string.IsNullOrWhiteSpace(Option);
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certData.CourseOption = SelectedOption;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            return certificate;
        }
    }
}