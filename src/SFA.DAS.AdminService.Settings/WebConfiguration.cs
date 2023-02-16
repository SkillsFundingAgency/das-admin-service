using System.Text.Json.Serialization;
using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.AdminService.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        public AzureApiAuthentication AzureApiAuthentication { get; set; }

        [JsonInclude] public ClientApiAuthentication EpaoApiAuthentication { get; set; }


        [JsonInclude] public AuthSettings StaffAuthentication { get; set; }
        [JsonInclude] public ManagedIdentityApiAuthentication QnaApiAuthentication { get; set; }
        [JsonInclude] public ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

        [JsonInclude] public string RoatpApiClientBaseUrl { get; set; }

        [JsonInclude] public string RoatpOversightBaseUrl { get; set; }
        [JsonInclude] public string RoatpAssessorBaseUrl { get; set; }
        [JsonInclude] public string RoatpGatewayBaseUrl { get; set; }
        [JsonInclude] public string RoatpFinanceBaseUrl { get; set; }
        [JsonInclude] public string RoatpProviderModerationBaseUrl { get; set; }

        [JsonInclude] public ClientApiAuthentication RoatpApiAuthentication { get; set; }

        [JsonInclude] public FeatureToggles FeatureToggles { get; set; }
        [JsonInclude] public RedisCacheSettings RedisCacheSettings { get; set; }
    }
}
