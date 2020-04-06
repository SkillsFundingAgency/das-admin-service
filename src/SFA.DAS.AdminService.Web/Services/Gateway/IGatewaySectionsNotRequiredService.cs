
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public interface IGatewaySectionsNotRequiredService
    {
        Task SetupNotRequiredLinks(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel, int providerRoute);
    }
}
