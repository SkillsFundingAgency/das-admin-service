using Newtonsoft.Json;

namespace SFA.DAS.AdminService.Settings
{
    public class FeatureToggles : IFeatureToggles
    {
        public bool EnableRoatpAssessorReview { get; set; }
        public bool EnableRoatpFinancialReview { get; set; }
        public bool EnableRoatpGatewayReview { get; set; }
        public bool EnableRoatpSnapshot { get; set; }
    }
}
