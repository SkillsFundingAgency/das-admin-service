using System;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IGatewayCompositionService
    {
       RoatpGatewayPageViewModel GetViewModelForPage(Guid applicationId, string pageId);
    }
}
