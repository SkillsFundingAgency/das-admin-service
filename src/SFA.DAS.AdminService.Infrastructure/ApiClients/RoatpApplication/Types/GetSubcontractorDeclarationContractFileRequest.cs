using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetSubcontractorDeclarationContractFileRequest
    {
        public Guid ApplicationId { get; }

        public GetSubcontractorDeclarationContractFileRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
