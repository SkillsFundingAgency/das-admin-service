using System.Collections.Generic;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkCertificateViewModel
    {
        public Guid Id { get; set; }
        public string ApprenticeForename { get; set; }
        public string ApprenticeSurname { get; set; }
        public DateTime ApprenticeDoB { get; set; }
        public long? ApprenticeULN { get; set; }
        public string CertificateNumber { get; set; }
        public string FrameworkName { get; set; }
        public string PathwayName { get; set; }
        public string ApprenticeshipLevelName { get; set; }
        public List<string> Qualifications { get; set; }
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public DateTime? ApprenticeStartdate { get; set; }
        public DateTime? ApprenticeLastdateInLearning { get; set; }
        public DateTime CertificationDate { get; set; }
        public string BackAction { get; set; }
    }
}
