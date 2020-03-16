using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetPeopleInControlHandler : IRequestHandler<GetPeopleInControlRequest, PeopleInControlPageViewModel>
    {
    }
}
