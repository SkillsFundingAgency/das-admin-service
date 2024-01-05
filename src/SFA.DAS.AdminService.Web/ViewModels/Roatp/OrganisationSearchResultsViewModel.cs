using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp
{
    public class OrganisationSearchResultsViewModel
    { 
        public string Title { get; set; }
        public string SearchTerm { get; set; }
        public List<Organisation> SearchResults { get; set; }
        public int TotalCount { get; set; }
        public int SelectedIndex { get; set; }

        public Organisation SelectedResult
        {
            get
            {
                if (SearchResults != null && SearchResults.Count > 0)
                {
                    return SearchResults[SelectedIndex];
                }

                return null;
            }
        }
    }
}
