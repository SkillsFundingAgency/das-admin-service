using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Financial
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public class RoatpFinancialDashboardViewModel
    {
        public PaginatedList<RoatpFinancialSummaryItem> Applications { get; set; }
        public RoatpFinancialApplicationsStatusCounts StatusCounts { get; set; }
        public string SelectedTab { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
    }
}
