using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Settings
{
    public class ApiAuthentication : IApiAuthentication
    {
        [JsonInclude] public string ClientId { get; set; }

        [JsonInclude] public string Instance { get; set; }

        [JsonInclude] public string TenantId { get; set; }

        [JsonInclude] public string Audience { get; set; }
    }
}