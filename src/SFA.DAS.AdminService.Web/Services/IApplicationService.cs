using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationModel = SFA.DAS.AdminService.Web.Models.Apply.Application;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IApplicationService
    {
        Task<ApplicationModel> GetApplication(Guid applicationId);
        Task<List<string>> ApproveApplication(ApplicationModel applicationId);
        Task<Organisation> GetOrganisation(Guid organisationId);
        Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string username);
    }
}
