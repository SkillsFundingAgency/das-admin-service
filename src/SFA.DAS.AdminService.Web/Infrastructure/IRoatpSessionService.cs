namespace SFA.DAS.AdminService.Web.Infrastructure
{
    using ViewModels.Roatp;

    public interface IRoatpSessionService
    {
        OrganisationSearchResultsViewModel GetSearchResults();
        void SetSearchResults(OrganisationSearchResultsViewModel model);
        void ClearSearchResults();
        string GetSearchTerm();
        void SetSearchTerm(string searchTerm);
        void ClearSearchTerm();
    }
}
