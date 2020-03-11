using System;
using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetLegalNameRequest: IRequest<LegalNamePageViewModel>
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetLegalNameRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
