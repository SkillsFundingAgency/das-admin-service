using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class GetIcoNumberRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetIcoNumberRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
