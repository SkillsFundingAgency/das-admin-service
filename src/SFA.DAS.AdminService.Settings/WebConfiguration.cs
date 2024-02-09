using Newtonsoft.Json;
using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.AdminService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;

namespace SFA.DAS.AdminService.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AzureApiClientConfiguration AzureApiAuthentication { get; set; }

        [JsonRequired] public AssessorApiClientConfiguration EpaoApiAuthentication { get; set; }


        [JsonRequired] public AuthSettings StaffAuthentication { get; set; }
        [JsonRequired] public QnaApiClientConfiguration QnaApiAuthentication { get; set; }
        [JsonRequired] public RoatpApplicationApiClientConfiguration ApplyApiAuthentication { get; set; }

        [JsonRequired] public string RoatpOversightBaseUrl { get; set; }
        [JsonRequired] public string RoatpAssessorBaseUrl { get; set; }
        [JsonRequired] public string RoatpGatewayBaseUrl { get; set; }
        [JsonRequired] public string RoatpFinanceBaseUrl { get; set; }
        [JsonRequired] public string RoatpProviderModerationBaseUrl { get; set; }

        [JsonRequired] public RoatpApiClientConfiguration RoatpApiAuthentication { get; set; }

        [JsonRequired] public FeatureToggles FeatureToggles { get; set; }
        [JsonRequired] public RedisCacheSettings RedisCacheSettings { get; set; }

        [JsonRequired] public bool UseDfESignIn { get; set; } = false;

        [JsonRequired] public string DfESignInServiceHelpUrl { get; set; }
    }
}
