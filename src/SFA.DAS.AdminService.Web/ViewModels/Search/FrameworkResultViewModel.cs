using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkResultViewModel
    {
        public Guid Id { get; set; }
        public string FrameworkName { get; set; }
        public string ApprenticeshipLevel { get; set; }
        public string CertificationYear { get; set; }
    }
}
