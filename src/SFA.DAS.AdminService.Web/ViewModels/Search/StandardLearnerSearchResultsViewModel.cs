using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class StandardLearnerSearchResultsViewModel
    {
        public string SearchString { get; set; } = string.Empty;
        public string OrganisationName { get; set; }
        public int Page { get; set; }
        public StaffSearchResult StaffSearchResult { get; set; }
    }
}
