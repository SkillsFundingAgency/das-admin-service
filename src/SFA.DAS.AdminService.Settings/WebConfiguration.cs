using Newtonsoft.Json;
using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.AdminService.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AzureApiAuthentication AzureApiAuthentication { get; set; }

        [JsonRequired] public ClientApiAuthentication EpaoApiAuthentication { get; set; }


        [JsonRequired] public AuthSettings StaffAuthentication { get; set; }
        [JsonRequired] public ManagedIdentityApiAuthentication QnaApiAuthentication { get; set; }
        [JsonRequired] public ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

        [JsonRequired] public string RoatpApiClientBaseUrl { get; set; }

        [JsonRequired] public string RoatpOversightBaseUrl { get; set; }
        [JsonRequired] public string RoatpAssessorBaseUrl { get; set; }
        [JsonRequired] public string RoatpGatewayBaseUrl { get; set; }
        [JsonRequired] public string RoatpFinanceBaseUrl { get; set; }
        [JsonRequired] public string RoatpProviderModerationBaseUrl { get; set; }

        [JsonRequired] public ManagedIdentityApiAuthentication RoatpApiAuthentication { get; set; }

        [JsonRequired] public FeatureToggles FeatureToggles { get; set; }
        [JsonRequired] public RedisCacheSettings RedisCacheSettings { get; set; }
    }
}
