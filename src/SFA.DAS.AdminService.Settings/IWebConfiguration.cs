using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.AdminService.Settings
{
    public interface IWebConfiguration
    {
        AzureApiAuthentication AzureApiAuthentication { get; set; }
        ClientApiAuthentication EpaoApiAuthentication { get; set; }


        string AssessmentOrgsApiClientBaseUrl { get; set; }
        string IfaApiClientBaseUrl { get; set; }

        string SessionRedisConnectionString { get; set; }
        AuthSettings StaffAuthentication { get; set; }
        ClientApiAuthentication ApplyApiAuthentication { get; set; }

        string RoatpApiClientBaseUrl { get; set; }

        string RoatpOversightBaseUrl { get; set; }

        ClientApiAuthentication RoatpApiAuthentication { get; set; }

        ClientApiAuthentication QnaApiAuthentication { get; set; }

        FeatureToggles FeatureToggles { get; set; }

    }
}