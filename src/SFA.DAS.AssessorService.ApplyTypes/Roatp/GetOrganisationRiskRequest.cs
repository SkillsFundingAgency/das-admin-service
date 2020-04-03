using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class GetOrganisationRiskRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetOrganisationRiskRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
