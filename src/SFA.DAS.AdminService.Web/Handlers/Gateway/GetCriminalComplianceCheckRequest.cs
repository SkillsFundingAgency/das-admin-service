using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using System;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetCriminalComplianceCheckRequest : IRequest<OrganisationCriminalCompliancePageViewModel>
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
