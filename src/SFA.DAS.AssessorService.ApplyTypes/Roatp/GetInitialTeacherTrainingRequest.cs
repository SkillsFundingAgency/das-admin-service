using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
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
