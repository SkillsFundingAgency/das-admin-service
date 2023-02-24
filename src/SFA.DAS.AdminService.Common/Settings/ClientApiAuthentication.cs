using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class ClientApiAuthentication : IClientApiAuthentication
    {
        public string Instance { get; set; }

        [JsonRequired] public string TenantId { get; set; }

        [JsonRequired] public string ClientId { get; set; }

        [JsonRequired] public string ClientSecret { get; set; }

        [JsonRequired] public string ResourceId { get; set; }

        [JsonRequired] public string ApiBaseAddress { get; set; }
    }
}