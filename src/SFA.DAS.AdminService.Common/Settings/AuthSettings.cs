using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class AuthSettings : IAuthSettings
    {
        [JsonInclude] public string WtRealm { get; set; }

        [JsonInclude] public string MetadataAddress { get; set; }

        public string Role { get; set; }
    }
}