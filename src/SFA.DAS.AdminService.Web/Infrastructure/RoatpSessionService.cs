namespace SFA.DAS.AdminService.Web.Infrastructure
{
    using Newtonsoft.Json;
    using ViewModels.Roatp;

    public class RoatpSessionService : IRoatpSessionService
    {
        private readonly ISessionService _sessionService;

        private const string _searchTermKey = "Roatp_SearchTerm";
        private const string _searchResultsSessionKey = "Roatp_SearchResults";

        public RoatpSessionService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public void SetSearchResults(OrganisationSearchResultsViewModel model)
        {
            var modelJson = JsonConvert.SerializeObject(model);
            _sessionService.Set(_searchResultsSessionKey, modelJson);
        }

        public OrganisationSearchResultsViewModel GetSearchResults()
        {
            var modelJson = _sessionService.Get(_searchResultsSessionKey);

            if (modelJson == null)
            {
                return null;
            }
            var model = JsonConvert.DeserializeObject<OrganisationSearchResultsViewModel>(modelJson);

            return model;
        }

        public void ClearSearchResults()
        {
            _sessionService.Remove(_searchResultsSessionKey);
        }

        public string GetSearchTerm()
        {
            return _sessionService.Get(_searchTermKey);
        }

        public void SetSearchTerm(string searchTerm)
        {
            _sessionService.Set(_searchTermKey, searchTerm);
        }

        public void ClearSearchTerm()
        {
            _sessionService.Remove(_searchTermKey);
        }
    }
}
