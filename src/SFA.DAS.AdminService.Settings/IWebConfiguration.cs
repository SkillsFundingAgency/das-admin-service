using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.AdminService.Settings
{
    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiAuthentication ApiAuthentication { get; set; }
        AzureApiAuthentication AzureApiAuthentication { get; set; }
        ClientApiAuthentication ClientApiAuthentication { get; set; }


        string AssessmentOrgsApiClientBaseUrl { get; set; }
        string IfaApiClientBaseUrl { get; set; }

        string SessionRedisConnectionString { get; set; }
        AuthSettings StaffAuthentication { get; set; }
        ClientApiAuthentication ApplyApiAuthentication { get; set; }

        string RoatpApiClientBaseUrl { get; set; }

        string RoatpOversightBaseUrl { get; set; }
        string RoatpGatewayBaseUrl { get; set; }

        ClientApiAuthentication RoatpApiAuthentication { get; set; }

        ClientApiAuthentication QnaApiAuthentication { get; set; }

        FeatureToggles FeatureToggles { get; set; }

    }
}