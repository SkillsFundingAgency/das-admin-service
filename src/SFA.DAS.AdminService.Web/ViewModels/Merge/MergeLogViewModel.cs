using SFA.DAS.AssessorService.Api.Types.Models.Merge;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class MergeLogViewModel
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public PaginatedList<MergeLogEntry> MergeLogs { get; set; }

        public bool IsEmpty => MergeLogs != null && MergeLogs.TotalRecordCount == 0;
    }
}
