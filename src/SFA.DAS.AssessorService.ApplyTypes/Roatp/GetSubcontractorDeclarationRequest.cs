﻿using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class GetSubcontractorDeclarationRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        
        public GetSubcontractorDeclarationRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
