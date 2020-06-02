using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IApplicationDeterminedDateValidationService
    {
        ValidationResponse ValidateApplicationDeterminedDate(int? day, int? month, int? year);
    }
}