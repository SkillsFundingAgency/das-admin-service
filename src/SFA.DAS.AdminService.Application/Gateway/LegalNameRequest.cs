using MediatR;
using SFA.DAS.AdminService.Application.ViewModels;
using System;

namespace SFA.DAS.AdminService.Application.Gateway
{
    public class LegalNameRequest: IRequest<RoatpGatewayPageViewModel>
    {
        public Guid ApplicationId { get; }
        public LegalNameRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
