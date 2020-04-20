using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
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