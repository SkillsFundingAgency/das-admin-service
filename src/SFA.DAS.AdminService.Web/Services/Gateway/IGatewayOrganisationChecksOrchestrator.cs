using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public interface IGatewayOrganisationChecksOrchestrator
    {
        Task<LegalNamePageViewModel> GetLegalNameViewModel(GetLegalNameRequest getLegalNameRequest);
        Task<TradingNamePageViewModel> GetTradingNameViewModel(GetTradingNameRequest getTradingNameRequest);
        Task<OrganisationStatusViewModel> GetOrganisationStatusViewModel(GetOrganisationStatusRequest getOrganisationStatusRequest);
        Task<AddressCheckViewModel> GetAddressViewModel(GetAddressRequest request);
        Task<IcoNumberViewModel> GetIcoNumberViewModel(GetIcoNumberRequest request);
        Task<WebsiteViewModel> GetWebsiteViewModel(GetWebsiteRequest request);
        Task<OrganisationRiskViewModel> GetOrganisationRiskViewModel(GetOrganisationRiskRequest request);
    }
}
