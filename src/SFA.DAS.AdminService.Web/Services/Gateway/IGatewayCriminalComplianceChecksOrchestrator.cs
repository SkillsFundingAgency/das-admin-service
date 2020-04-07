using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public interface IGatewayCriminalComplianceChecksOrchestrator
    {
        Task<OrganisationCriminalCompliancePageViewModel> GetCriminalComplianceCheckViewModel(GetCriminalComplianceCheckRequest request);
    }
}
