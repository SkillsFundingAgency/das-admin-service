using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetCriminalComplianceCheckRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public string PageId { get; set; }

        public GetCriminalComplianceCheckRequest(Guid applicationId, string pageId, string username)
        {
            ApplicationId = applicationId;
            PageId = pageId;
            UserName = username;
        }
    }
}
