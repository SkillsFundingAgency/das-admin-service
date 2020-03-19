using SFA.DAS.AdminService.Web.Handlers.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public interface IGatewayOrganisationChecksOrchestrator
    {
        Task<LegalNamePageViewModel> GetLegalNameViewModel(GetLegalNameRequest getLegalNameRequest);
        Task<TradingNamePageViewModel> GetTradingNameViewModel(GetTradingNameRequest getTradingNameRequest);
    }
}
