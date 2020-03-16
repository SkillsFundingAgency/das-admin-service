using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetPeopleInControlRequest: IRequest<PeopleInControlPageViewModel>
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetPeopleInControlRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
