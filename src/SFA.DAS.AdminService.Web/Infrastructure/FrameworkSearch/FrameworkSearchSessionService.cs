using System;

namespace SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch
{
    public class FrameworkSearchSessionService : IFrameworkSearchSessionService
    {
        private readonly ISessionService _sessionService;

        private const string _frameworkSearchSessionKey = "Framework_Search";

        public FrameworkSearchSessionService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public Models.Search.FrameworkSearchSession SessionFrameworkSearch
        {
            get
            {
                return _sessionService.Get<Models.Search.FrameworkSearchSession>(_frameworkSearchSessionKey);
            }
            set
            {
                if (value == null)
                {
                    _sessionService.Remove(_frameworkSearchSessionKey);
                }
                else
                {
                    _sessionService.Set(_frameworkSearchSessionKey, value);
                }
            }
        }

        public void ClearFrameworkSearchRequest()
        {
            _sessionService.Remove(_frameworkSearchSessionKey);
        }

        public void UpdateFrameworkSearchRequest(Action<Models.Search.FrameworkSearchSession> action)
        {
            var sessionObject = SessionFrameworkSearch;
            action(sessionObject);
            SessionFrameworkSearch = sessionObject;
        }
    }
}
