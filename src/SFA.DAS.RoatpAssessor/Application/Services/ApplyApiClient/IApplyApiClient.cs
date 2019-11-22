using SFA.DAS.RoatpAssessor.Application.Gateway.Commands;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application
{
    public interface IApplyApiClient
    {
        Task<List<Domain.DTOs.Application>> GetSubmittedApplicationsAsync();
        Task<GatewayCounts> GetGatewayCountsAsync();
        Task UpdateGatewayOutcomesAsync(UpdateGatewayOutcomesCommand command);
        Task<Domain.DTOs.Gateway> GetGatewayReviewAsync(Guid applicationId);
        Task<Domain.DTOs.Application> GetApplicationAsync(Guid applicationId);
    }
}
