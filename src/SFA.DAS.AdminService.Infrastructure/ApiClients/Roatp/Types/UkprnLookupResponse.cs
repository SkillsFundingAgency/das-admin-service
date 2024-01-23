using System.Collections.Generic;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    public class UkprnLookupResponse
    {
        public bool Success { get; set; }
        public List<ProviderDetails> Results { get; set; }
    }
}
