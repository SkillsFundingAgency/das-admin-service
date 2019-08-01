using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Application.Interfaces.Validation
{
    public interface IAssessorValidationService
    {
        Task<ValidationResponse> ValidateNewOrganisationRequest(CreateEpaOrganisationRequest request);
        Task<ValidationResponse> ValidateNewContactRequest(CreateEpaOrganisationContactRequest request);
        Task<ValidationResponse> ValidateNewOrganisationStandardRequest(CreateEpaOrganisationStandardRequest request);
    }
}
