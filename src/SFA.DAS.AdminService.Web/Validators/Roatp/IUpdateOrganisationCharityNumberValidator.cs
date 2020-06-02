using System.Collections.Generic;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public interface IUpdateOrganisationCharityNumberValidator
    {
        List<ValidationErrorDetail> IsDuplicateCharityNumber(UpdateOrganisationCharityNumberViewModel viewModel);
    }
}
