using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetTradingNameRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetTradingNameRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
