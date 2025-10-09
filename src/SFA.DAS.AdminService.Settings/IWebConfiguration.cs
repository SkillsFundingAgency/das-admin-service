using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.AdminService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;

namespace SFA.DAS.AdminService.Settings
{
    public interface IWebConfiguration
    {
        AzureApiClientConfiguration AzureApiAuthentication { get; set; }
        AssessorApiClientConfiguration EpaoApiAuthentication { get; set; }

        AuthSettings StaffAuthentication { get; set; }
        QnaApiClientConfiguration QnaApiAuthentication { get; set; }
        RoatpApplicationApiClientConfiguration ApplyApiAuthentication { get; set; }

        string RoatpOversightBaseUrl { get; set; }
        string RoatpGatewayBaseUrl { get; set; }
        string RoatpFinanceBaseUrl { get; set; }
        string RoatpAssessorBaseUrl { get; set; }
        string RoatpProviderModerationBaseUrl { get; set; }
        string RoatpAdminBaseUrl { get; set; }
        RoatpApiClientConfiguration RoatpApiAuthentication { get; set; }

        FeatureToggles FeatureToggles { get; set; }
        RedisCacheSettings RedisCacheSettings { get; }

        bool UseDfESignIn { get; }

        string DfESignInServiceHelpUrl { get; set; }
    }
}