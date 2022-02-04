using System;

namespace SFA.DAS.AssessorService.Api.Types.Commands
{
    public class MergeOrganisationsCommand
    {
        public string PrimaryEndPointAssessorOrganisationId { get; set; }
        public string SecondaryEndPointAssessorOrganisationId { get; set; }
        public DateTime SecondaryStandardsEffectiveTo { get; set; }
        public string ActionedByUser { get; set; }
    }
}
