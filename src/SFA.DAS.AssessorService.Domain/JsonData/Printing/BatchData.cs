using System;
using System.Text.Json.Serialization;

namespace SFA.DAS.AssessorService.Domain.JsonData.Printing
{
    public class BatchData
    {
        public int BatchNumber { get; set; }
        public DateTime BatchDate { get; set; }
        public int PostalContactCount { get; set; }
        public int TotalCertificateCount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? PrintedDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? PostedDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? DateOfResponse { get; set; }
    }


    public class BatchDataResponse
    {
        public string BatchNumber { get; set; }
        public DateTime BatchDate { get; set; }
        public int PostalContactCount { get; set; }
        public int TotalCertificateCount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? PrintedDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? PostedDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? DateOfResponse { get; set; }
    }
}
