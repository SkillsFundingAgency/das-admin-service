using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardVersion
    {
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateVersionApproved { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
    }
}
