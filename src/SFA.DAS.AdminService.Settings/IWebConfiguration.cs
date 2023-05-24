using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.AdminService.Settings
{
    public interface IWebConfiguration
    {
        AzureApiAuthentication AzureApiAuthentication { get; set; }
        ClientApiAuthentication EpaoApiAuthentication { get; set; }


        AuthSettings StaffAuthentication { get; set; }
        ManagedIdentityApiAuthentication QnaApiAuthentication { get; set; }
        ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

        string RoatpApiClientBaseUrl { get; set; }

        string RoatpOversightBaseUrl { get; set; }
        string RoatpGatewayBaseUrl { get; set; }
        string RoatpFinanceBaseUrl { get; set; }
        string RoatpAssessorBaseUrl { get; set; }
        string RoatpProviderModerationBaseUrl { get; set; }

        ClientApiAuthentication RoatpApiAuthentication { get; set; }

        FeatureToggles FeatureToggles { get; set; }
        RedisCacheSettings RedisCacheSettings { get; }

        /// <summary>
        /// Gets or Sets the UseDfeSignIn property.
        /// </summary>
        bool UseDfeSignIn { get; }
    }
}