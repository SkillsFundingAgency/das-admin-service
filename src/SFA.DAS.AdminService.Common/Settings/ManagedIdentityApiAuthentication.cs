using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class ManagedIdentityApiAuthentication : IManagedIdentityApiAuthentication
    {
        [JsonInclude] public string Identifier { get; set; }

        [JsonInclude] public string ApiBaseAddress { get; set; }
    }
}
