using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class GetOrganisationStatusRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetOrganisationStatusRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
