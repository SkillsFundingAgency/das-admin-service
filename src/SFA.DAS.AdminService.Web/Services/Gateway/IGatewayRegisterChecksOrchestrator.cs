using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public interface IGatewayRegisterChecksOrchestrator
    {
        Task<RoatpPageViewModel> GetRoatpViewModel(GetRoatpRequest getRoatpRequest);
    }
}
