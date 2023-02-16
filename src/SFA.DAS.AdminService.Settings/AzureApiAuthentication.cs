using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Settings
{
    public class AzureApiAuthentication : IAzureApiAuthentication
    {
        [JsonInclude] public string Id { get; set; }

        [JsonInclude] public string Key { get; set; }

        [JsonInclude] public string ApiBaseAddress { get; set; }

        [JsonInclude] public string ProductId { get; set; }

        [JsonInclude] public string GroupId { get; set; }

        [JsonInclude] public string RequestBaseAddress { get; set; }
    }
}