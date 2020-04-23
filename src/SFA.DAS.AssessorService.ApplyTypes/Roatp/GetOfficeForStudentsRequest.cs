using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
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
