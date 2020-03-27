using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class GetTradingNameRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetTradingNameRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
