using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.AllowedProviders
{
    public class AllowedProvider
    {
        public int Ukprn { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? AddedDateTime { get; set; }
    }
}
