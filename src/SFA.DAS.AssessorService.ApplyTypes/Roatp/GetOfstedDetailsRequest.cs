using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
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
