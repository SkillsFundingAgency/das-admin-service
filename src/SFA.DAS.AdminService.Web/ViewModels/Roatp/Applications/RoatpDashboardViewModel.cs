using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class RoatpDashboardViewModel
    {
        public PaginatedList<AssessorService.ApplyTypes.Roatp.Apply> Applications { get; set; }
    }
}
