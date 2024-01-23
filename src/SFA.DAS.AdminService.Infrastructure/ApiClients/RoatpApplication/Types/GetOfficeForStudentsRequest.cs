using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetOfficeForStudentsRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetOfficeForStudentsRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
