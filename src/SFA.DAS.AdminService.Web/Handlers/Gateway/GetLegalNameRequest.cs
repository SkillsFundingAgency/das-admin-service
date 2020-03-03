using System;
using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetLegalNameRequest: IRequest<RoatpGatewayPageViewModel>
    {
        public Guid ApplicationId { get; }
        public GetLegalNameRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
