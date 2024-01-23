using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
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
