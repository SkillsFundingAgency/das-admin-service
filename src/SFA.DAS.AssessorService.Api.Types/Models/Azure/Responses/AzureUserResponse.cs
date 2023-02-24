namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using System.Text.Json.Serialization;
    using System.Collections.Generic;

    public class AzureUserResponse
    {
        [JsonPropertyName("value")]
        public IEnumerable<AzureUser> Users { get; set; }
        [JsonPropertyName("count")]
        public int TotalCount { get; set; }
        public string NextLink { get; set; }
    }
}
