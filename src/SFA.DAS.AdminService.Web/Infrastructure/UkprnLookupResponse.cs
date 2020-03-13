using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class UkprnLookupResponse
    {
        public bool Success { get; set; }
        public List<ProviderDetails> Results { get; set; }
    }
}