using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class ClientApiAuthentication : IClientApiAuthentication
    {
        public string Instance { get; set; }

        [JsonInclude] public string TenantId { get; set; }

        [JsonInclude] public string ClientId { get; set; }

        [JsonInclude] public string ClientSecret { get; set; }

        [JsonInclude] public string ResourceId { get; set; }

        [JsonInclude] public string ApiBaseAddress { get; set; }
    }
}