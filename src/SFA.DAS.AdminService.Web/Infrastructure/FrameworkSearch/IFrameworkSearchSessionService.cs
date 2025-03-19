using System;

namespace SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch
{
    public interface IFrameworkSearchSessionService
    {
        Models.Search.FrameworkSearchSession SessionFrameworkSearch { get; set; }
        void UpdateFrameworkSearchRequest(Action<Models.Search.FrameworkSearchSession> action);
        void ClearFrameworkSearchRequest();
    }
}
