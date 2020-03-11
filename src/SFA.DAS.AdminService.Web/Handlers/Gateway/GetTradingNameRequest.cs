using System;
using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetTradingNameRequest : IRequest<TradingNamePageViewModel>
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetTradingNameRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = UserName;
        }
    }
}
