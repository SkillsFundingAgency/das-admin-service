using SFA.DAS.AssessorService.Api.Types.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SFA.DAS.AdminService.Application.Interfaces
{
    public interface IAnswerService
    {
        Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId);
        Task<string> GetAnswer(Guid applicationId, string questionTag);
        Task<List<string>> InjectApplyOrganisationAndContactDetailsIntoRegister(CreateOrganisationContactCommand createOrganisationAndContactCommand);
    }
}
