using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public class GetTagFromApplyDataService: IGetTagFromApplyDataService
    {
        public string GetValueFromQuestionTag(Dictionary<string, object> qnaApplyData, string tagKey)
        {
            if (qnaApplyData == null || !qnaApplyData.Any()) return string.Empty;

            foreach (var variable in qnaApplyData.Where(variable => variable.Value != null).Where(variable => variable.Key == tagKey))
            {
                return variable.Value.ToString();
            }

            return string.Empty;
        }
    }

    public interface IGetTagFromApplyDataService
    {
        string GetValueFromQuestionTag(Dictionary<string, object> qnaApplyData, string tagKey);
    }
}
