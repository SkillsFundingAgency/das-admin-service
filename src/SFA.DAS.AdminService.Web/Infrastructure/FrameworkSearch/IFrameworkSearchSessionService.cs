using System;

namespace SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch
{
    public interface IFrameworkSearchSessionService
    {
        Models.Search.FrameworkSearch SessionFrameworkSearch { get; set; }
        void UpdateFrameworkSearchRequest(Action<Models.Search.FrameworkSearch> action);
        void ClearFrameworkSearchRequest();
    }
}
