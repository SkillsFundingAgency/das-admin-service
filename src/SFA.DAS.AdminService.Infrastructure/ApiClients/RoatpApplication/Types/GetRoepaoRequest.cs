using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetRoepaoRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetRoepaoRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
