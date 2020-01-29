using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Financial
{
    public class RoatpFinancialDashboardViewModel
    {
        public PaginatedList<AssessorService.ApplyTypes.Roatp.Apply> Applications { get; set; }
    }
}
