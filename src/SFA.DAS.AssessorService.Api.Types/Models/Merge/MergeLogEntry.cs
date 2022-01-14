using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Merge
{
    public class MergeLogEntry
    {
        public string PrimaryEpaoId { get; set; }
        public string PrimaryEpaoName { get; set; }
        public string SecondaryEpaoId { get; set; }
        public string SecondaryEpaoName { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime EffectiveTo { get; set; }
    }
}
