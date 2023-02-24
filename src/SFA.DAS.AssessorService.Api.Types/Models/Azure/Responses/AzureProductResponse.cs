namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    using System.Text.Json.Serialization;
    using System.Collections.Generic;

    public class AzureProductResponse
    {
        [JsonPropertyName("value")]
        public IEnumerable<AzureProduct> Products { get; set; }
        [JsonPropertyName("count")]
        public int TotalCount { get; set; }
        public string NextLink { get; set; }
    }
}
