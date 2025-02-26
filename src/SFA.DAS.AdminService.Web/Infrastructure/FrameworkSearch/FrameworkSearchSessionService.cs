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

        public Models.Search.FrameworkSearch SessionFrameworkSearch
        {
            get
            {
                return _sessionService.Get<Models.Search.FrameworkSearch>(_frameworkSearchSessionKey);
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

        public void UpdateFrameworkSearchRequest(Action<Models.Search.FrameworkSearch> action)
        {
            var sessionObject = SessionFrameworkSearch;
            action(sessionObject);
            SessionFrameworkSearch = sessionObject;
        }
    }
}
