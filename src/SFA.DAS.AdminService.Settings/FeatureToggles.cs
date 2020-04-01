using Newtonsoft.Json;

namespace SFA.DAS.AdminService.Settings
{
    public class FeatureToggles : IFeatureToggles
    {
        public bool EnableRoatpApply { get; set; }
    }
}
