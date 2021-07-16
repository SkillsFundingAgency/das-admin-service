using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp.AllowList
{
    public class AllowedUkprn
    {
        public string Ukprn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
