using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class MergeLogViewModel
    {
        public List<string> MergeLogs { get; set; }

        public bool IsEmpty => MergeLogs != null && MergeLogs.Count == 0;
    }
}
