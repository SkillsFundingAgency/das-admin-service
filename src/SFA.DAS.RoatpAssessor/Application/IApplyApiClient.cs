using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application
{
    public interface IApplyApiClient
    {
        Task<List<Domain.DTOs.Application>> GetSubmittedApplicationsAsync();
        Task<GatewayCounts> GetGatewayCounts();
    }
}
