using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class FeatureToggles : IFeatureToggles
    {
        [JsonRequired] public bool EnableRoatpSnapshot { get; set; }
    }
}
