using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp.AllowedProviders
{
    public class AllowedProvider
    {
        public string Ukprn { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? AddedDateTime { get; set; }
    }
}
