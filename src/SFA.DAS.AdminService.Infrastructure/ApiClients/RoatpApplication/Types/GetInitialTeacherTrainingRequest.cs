using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetInitialTeacherTrainingRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetInitialTeacherTrainingRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
