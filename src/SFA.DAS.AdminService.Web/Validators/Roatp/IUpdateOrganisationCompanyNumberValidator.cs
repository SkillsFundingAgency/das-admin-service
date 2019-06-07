using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public interface IUpdateOrganisationCompanyNumberValidator
    {
        List<ValidationErrorDetail> IsDuplicateCompanyNumber(UpdateOrganisationCompanyNumberViewModel viewModel);
    }
}
