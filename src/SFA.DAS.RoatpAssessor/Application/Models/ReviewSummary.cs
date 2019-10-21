using System;

namespace SFA.DAS.RoatpAssessor.Application.Models
{
    public class ReviewSummary
    {
        public Guid ApplicationId { get; set; }
        public string Organisation { get; set; }
        public string Ukprn { get; set; }
        public string ApplicationRef { get; set; }
        public string ProviderType { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
