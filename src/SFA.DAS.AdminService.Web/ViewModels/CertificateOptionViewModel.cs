using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class CertificateOptionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public bool HasAdditionalLearningOption { get; set; }
        public string Option { get; set; }
        public string SelectedOption { get; set; }
        public List<Option> Options { get; set; }
        public int StandardCode { get; set; }
        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            Option = CertificateData.CourseOption;
            HasAdditionalLearningOption = !string.IsNullOrWhiteSpace(Option);
            StandardCode = cert.StandardCode;
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.CourseOption = SelectedOption;
            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}