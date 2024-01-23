using SFA.DAS.AdminService.Application.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IAnswerInjectionService
    {
        Task<CreateOrganisationAndContactFromApplyResponse> InjectApplyOrganisationAndContactDetailsIntoRegister(CreateOrganisationContactCommand command);
        Task<CreateOrganisationStandardFromApplyResponse> InjectApplyOrganisationStandardDetailsIntoRegister(CreateOrganisationStandardCommand command);
    }
}