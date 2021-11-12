using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardVersion
    {
        public string Title { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateVersionApproved { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public int LarsCode { get; set; }
        public string IFateReferenceNumber { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }
    }
}
