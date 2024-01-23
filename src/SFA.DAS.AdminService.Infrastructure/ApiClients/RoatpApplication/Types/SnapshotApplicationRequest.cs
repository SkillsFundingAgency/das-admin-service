using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.Apply;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class SnapshotApplicationRequest
    {
        public Guid ApplicationId { get; set; }
        public Guid SnapshotApplicationId { get; set; }
        public List<RoatpApplySequence> Sequences { get; set; }
    }
}
