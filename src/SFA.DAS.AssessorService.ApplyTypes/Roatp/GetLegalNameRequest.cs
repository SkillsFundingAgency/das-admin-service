using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
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
