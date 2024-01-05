namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    using System.Collections.Generic;

    public class OrganisationSearchResults
    {
        public List<Organisation> SearchResults { get; set; }

        public int TotalCount { get; set; }
    }
}
