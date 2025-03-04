using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkCertificateSummaryViewModel
    {
        public Guid Id { get; set; }
        public string FrameworkName { get; set; }
        public string ApprenticeshipLevelName { get; set; }
        public string CertificationYear { get; set; }
    }
}
