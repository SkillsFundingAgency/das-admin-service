using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class AuthSettings : IAuthSettings
    {
        [JsonRequired] public string WtRealm { get; set; }

        [JsonRequired] public string MetadataAddress { get; set; }

        public string Role { get; set; }
    }
}