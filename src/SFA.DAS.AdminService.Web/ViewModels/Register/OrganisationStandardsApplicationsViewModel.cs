using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AdminService.Web.ViewModels.Register
{
    public class OrganisationStandardsApplicationsViewModel
    {
        public string OrganisationId { get; set; }

        public PaginationViewModel<ApplicationSummaryItem> PaginationViewModel { get; set; }

        public bool HasStandards => (PaginationViewModel?.PaginatedList?.TotalRecordCount ?? 0) > 0;
    }
}
