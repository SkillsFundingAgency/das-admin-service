using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetApplicationOverviewRequest : IRequest<RoatpGatewayApplicationViewModel>
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetApplicationOverviewRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
