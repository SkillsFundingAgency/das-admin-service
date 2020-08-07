using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class GetPeopleInControlRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetPeopleInControlRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}

