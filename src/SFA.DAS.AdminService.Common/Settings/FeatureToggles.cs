using Newtonsoft.Json;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class FeatureToggles : IFeatureToggles
    {
        [JsonRequired] public bool EnableRoatpAssessorReview { get; set; }
        [JsonRequired] public bool EnableRoatpFinancialReview { get; set; }
        [JsonRequired] public bool EnableRoatpGatewayReview { get; set; }
        [JsonRequired] public bool EnableRoatpSnapshot { get; set; }
        [JsonRequired] public bool EnableRoatpOversightReview { get; set; }
    }
}
