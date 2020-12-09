using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateLastRunStatusRequest
    {
        public Guid ScheduleRunId { get; set; }
        public LastRunStatus LastRunStatus { get; set; }
    }
}
