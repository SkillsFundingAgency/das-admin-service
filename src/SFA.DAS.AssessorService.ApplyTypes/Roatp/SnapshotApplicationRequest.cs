using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class SnapshotApplicationRequest
    {
        public Guid ApplicationId { get; set; }
        public Guid SnapshotApplicationId { get; set; }
        public List<RoatpApplySequence> Sequences { get; set; }
    }
}
