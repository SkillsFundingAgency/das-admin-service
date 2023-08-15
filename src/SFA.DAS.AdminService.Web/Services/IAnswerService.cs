using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IAnswerService
    {
        Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId);
        Task<CreateOrganisationStandardCommand> GatherAnswersForOrganisationStandardForApplication(Guid applicationId);

        Task<WithdrawOrganisationRequest> GatherAnswersForWithdrawOrganisationForApplication(Guid applicationId, string updatedBy);
        Task<WithdrawStandardRequest> GatherAnswersForWithdrawStandardForApplication(Guid applicationId);
    }
}