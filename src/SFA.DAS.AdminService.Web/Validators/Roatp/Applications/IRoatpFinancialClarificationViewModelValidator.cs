using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Financial;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public interface IRoatpFinancialClarificationViewModelValidator
    {
        ValidationResponse Validate(RoatpFinancialClarificationViewModel vm, bool isClarificationFilesUpload, bool isClarificationOutcome);
    }
}
