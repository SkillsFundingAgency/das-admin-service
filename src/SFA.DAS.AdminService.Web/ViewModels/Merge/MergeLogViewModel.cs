using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class MergeLogViewModel
    {
        public string ControllerName { get; set; }
        public PaginationViewModel<MergeLogEntry> MergeLogResults { get; set; }
    }
}
