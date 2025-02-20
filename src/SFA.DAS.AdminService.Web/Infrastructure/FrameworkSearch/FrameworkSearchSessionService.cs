using SFA.DAS.AdminService.Web.Models.FrameworkSearch;
using SFA.DAS.AdminService.Web.Models.Merge;
using System;

namespace SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch
{
    public class FrameworkSearchSessionService : IFrameworkSearchSessionService
    {
        private ISessionService _sessionService;

        private const string _frameworkSearchSessionKey = "Framework_Search";

        public FrameworkSearchSessionService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public void StartNewFrameworkSearch()
        {
            var frameworkSearchRequest = new FrameworkSearchRequest();

            frameworkSearchRequest.StartNewRequest();

            _sessionService.Set(_frameworkSearchSessionKey, frameworkSearchRequest);
        }

        public FrameworkSearchRequest GetFrameworkSearchRequest()
        {
            return _sessionService.Get<FrameworkSearchRequest>(_frameworkSearchSessionKey);
        }

        public void UpdateFrameworkSearchRequest(FrameworkSearchRequest frameworkSearchRequest)
        {
            _sessionService.Set(_frameworkSearchSessionKey, frameworkSearchRequest);
        }

    }
}
