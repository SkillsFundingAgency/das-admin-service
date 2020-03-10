using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IGetTagFromApplyDataService
    {
        string GetValueFromQuestionTag(Dictionary<string, object> qnaApplyData, string tagKey);
    }
}