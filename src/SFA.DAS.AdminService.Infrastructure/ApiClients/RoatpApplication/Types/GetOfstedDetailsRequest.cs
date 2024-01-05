using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetOfstedDetailsRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetOfstedDetailsRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
