using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
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
