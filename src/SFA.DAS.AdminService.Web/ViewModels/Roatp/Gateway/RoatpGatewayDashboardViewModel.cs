using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoatpGatewayDashboardViewModel
    {
        public PaginatedList<RoatpApplicationSummaryItem> Applications { get; set; }
    }
}
