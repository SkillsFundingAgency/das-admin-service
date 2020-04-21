using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class GetRoatpRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetRoatpRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
