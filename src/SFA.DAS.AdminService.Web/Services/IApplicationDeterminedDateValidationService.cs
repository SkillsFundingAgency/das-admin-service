using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IApplicationDeterminedDateValidationService
    {
        ValidationResponse ValidateApplicationDeterminedDate(int? day, int? month, int? year);
    }
}