namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class AzureGroupResponse
    {
        [JsonPropertyName("value")]
        public IEnumerable<AzureGroup> Groups { get; set; }
        [JsonPropertyName("count")]
        public int TotalCount { get; set; }
        public string NextLink { get; set; }
    }
}
