using SFA.DAS.AdminService.Web.Models.FrameworkSearch;

namespace SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch
{
    public interface IFrameworkSearchSessionService
    {
        void StartNewFrameworkSearch();
        FrameworkSearchRequest GetFrameworkSearchRequest();
        void UpdateFrameworkSearchRequest(FrameworkSearchRequest mergeRequest);
    }
}
