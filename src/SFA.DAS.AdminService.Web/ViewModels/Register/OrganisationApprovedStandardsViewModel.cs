using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AdminService.Web.ViewModels.Register
{
    public class OrganisationApprovedStandardsViewModel
    {
        public string OrganisationId { get; set; }

        public PaginationViewModel<OrganisationStandardSummary> PaginationViewModel { get; set; }

        public bool HasStandards
        {
            get
            {
                return (PaginationViewModel?.PaginatedList?.TotalRecordCount ?? 0) > 0;
            }
        }
    }
}
