using Microsoft.Extensions.Configuration;
using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.AdminService.Settings
{
    public interface IWebConfiguration
    {
        AzureApiAuthentication AzureApiAuthentication { get; set; }
        ClientApiAuthentication EpaoApiAuthentication { get; set; }


        string IfaApiClientBaseUrl { get; set; }
        AuthSettings StaffAuthentication { get; set; }
        ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

        string RoatpApiClientBaseUrl { get; set; }

        string RoatpOversightBaseUrl { get; set; }
        string RoatpGatewayBaseUrl { get; set; }

        string RoatpAssessorBaseUrl { get; set; }

        ClientApiAuthentication RoatpApiAuthentication { get; set; }

        ClientApiAuthentication QnaApiAuthentication { get; set; }

        FeatureToggles FeatureToggles { get; set; }
        RedisCacheSettings RedisCacheSettings { get; }
    }
}