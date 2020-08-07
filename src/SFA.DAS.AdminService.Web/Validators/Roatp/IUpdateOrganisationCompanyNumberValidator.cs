using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public interface IUpdateOrganisationCompanyNumberValidator
    {
        List<ValidationErrorDetail> IsDuplicateCompanyNumber(UpdateOrganisationCompanyNumberViewModel viewModel);
    }
}
