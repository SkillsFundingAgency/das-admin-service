using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetLegalNameRequest
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
