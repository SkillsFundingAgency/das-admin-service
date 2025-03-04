using System;

namespace SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch
{
    public interface IFrameworkSearchSessionService
    {
        Models.Search.FrameworkSearchSessionData SessionFrameworkSearch { get; set; }
        void UpdateFrameworkSearchRequest(Action<Models.Search.FrameworkSearchSessionData> action);
        void ClearFrameworkSearchRequest();
    }
}
