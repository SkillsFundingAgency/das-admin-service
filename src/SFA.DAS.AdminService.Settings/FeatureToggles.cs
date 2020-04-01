using Newtonsoft.Json;

namespace SFA.DAS.AdminService.Settings
{
    public class FeatureToggles : IFeatureToggles
    {
        [JsonRequired]  public bool EnableRoatpApply { get; set; }
    }
}
