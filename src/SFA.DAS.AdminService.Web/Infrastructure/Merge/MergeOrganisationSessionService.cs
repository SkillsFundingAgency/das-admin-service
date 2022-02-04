using SFA.DAS.AdminService.Web.Models.Merge;

namespace SFA.DAS.AdminService.Web.Infrastructure.Merge
{
    public class MergeOrganisationSessionService : IMergeOrganisationSessionService
    {
        private ISessionService _sessionService;

        private const string _mergeOrganisationsSessionKey = "Merge_Organisations";

        public MergeOrganisationSessionService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public void StartNewMergeRequest()
        {
            var mergeRequest = new MergeRequest();

            mergeRequest.StartNewRequest();

            _sessionService.Set(_mergeOrganisationsSessionKey, mergeRequest);
        }

        public MergeRequest GetMergeRequest()
        {
            return _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);
        }

        public void UpdateMergeRequest(MergeRequest mergeRequest)
        {
            _sessionService.Set(_mergeOrganisationsSessionKey, mergeRequest);
        }


        public void AddSearchEpaoCommand(string type, string searchString)
        {
            var request = _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);

            request.AddSearchEpaoCommand(type, searchString);

            _sessionService.Set(_mergeOrganisationsSessionKey, request);
        }

        public IPagingState MergeOrganisationPagingState
        {
            get
            {
                return new PagingState(_sessionService, "Merge_Organisations_PagingState");
            }
            set
            {
                _sessionService.Set("Merge_Organisations_PagingState", value);
            }
        }
    }
}
