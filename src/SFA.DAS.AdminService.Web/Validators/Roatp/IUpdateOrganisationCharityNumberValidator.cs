using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public interface IUpdateOrganisationCharityNumberValidator
    {
        List<ValidationErrorDetail> IsDuplicateCharityNumber(UpdateOrganisationCharityNumberViewModel viewModel);
    }
}
