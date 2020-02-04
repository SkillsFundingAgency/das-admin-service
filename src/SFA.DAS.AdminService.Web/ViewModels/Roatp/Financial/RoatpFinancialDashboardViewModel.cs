using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Financial
{
    public class RoatpFinancialDashboardViewModel
    {
        public PaginatedList<RoatpApplicationSummaryItem> Applications { get; set; }
    }
}
