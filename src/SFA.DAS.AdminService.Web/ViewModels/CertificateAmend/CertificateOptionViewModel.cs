using System.Text.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateOptionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public bool HasAdditionalLearningOption { get; set; }
        public string Option { get; set; }
        public string SelectedOption { get; set; }
        public IEnumerable<string> Options { get; set; }
        
        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);
            Option = CertificateData.CourseOption;
            HasAdditionalLearningOption = !string.IsNullOrWhiteSpace(Option);
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certData.CourseOption = SelectedOption;
            certificate.CertificateData = JsonSerializer.Serialize(certData);

            return certificate;
        }
    }
}