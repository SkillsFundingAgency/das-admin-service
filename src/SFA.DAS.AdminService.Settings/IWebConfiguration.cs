using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AdminService.Settings
{
    public interface IWebConfiguration
    {
        AzureApiClientConfiguration AzureApiAuthentication { get; set; }
        AzureActiveDirectoryClientConfiguration EpaoApiAuthentication { get; set; }

        AuthSettings StaffAuthentication { get; set; }
        ManagedIdentityClientConfiguration QnaApiAuthentication { get; set; }
        RoatpApplicationApiClientConfiguration ApplyApiAuthentication { get; set; }

        string RoatpOversightBaseUrl { get; set; }
        string RoatpGatewayBaseUrl { get; set; }
        string RoatpFinanceBaseUrl { get; set; }
        string RoatpAssessorBaseUrl { get; set; }
        string RoatpProviderModerationBaseUrl { get; set; }

        RoatpApiClientConfiguration RoatpApiAuthentication { get; set; }

        FeatureToggles FeatureToggles { get; set; }
        RedisCacheSettings RedisCacheSettings { get; }

        bool UseDfESignIn { get; }

        string DfESignInServiceHelpUrl { get; set; }
    }
}