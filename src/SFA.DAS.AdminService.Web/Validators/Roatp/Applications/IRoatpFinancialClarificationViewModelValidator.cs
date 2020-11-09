using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Financial;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    public interface IRoatpFinancialClarificationViewModelValidator
    {
        Task<ValidationResponse> Validate(RoatpFinancialClarificationViewModel vm, bool isClarificationFilesUpdate, bool isClarificationOutcome);
    }
}
