using MediatR;
using SFA.DAS.AdminService.Application.ViewModels;
using System;

namespace SFA.DAS.AdminService.Application.Gateway
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
