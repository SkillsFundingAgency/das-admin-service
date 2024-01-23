using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetPeopleInControlHighRiskRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetPeopleInControlHighRiskRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}