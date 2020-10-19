using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateScheduleRunStatusRequest
    {
        public Guid ScheduleRunId { get; set; }
        public ScheduleRunStatus ScheduleRunStatus { get; set; }
    }
}
