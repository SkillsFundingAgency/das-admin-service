using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Commands
{
    public class CreateOrganisationStandardCommand
    {
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }

        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public List <string> StandardVersions { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime DateStandardApprovedOnRegister { get; } = DateTime.Now.Date;
        public List<string> DeliveryAreas { get; set; }

        public Guid ApplyingContactId { get; set; }
        public string ApplicationType{ get; set; }

        public CreateOrganisationStandardCommand(Guid organisationId, string endPointAssessorOrganisationId,
            int standardCode, string standardReference, List<string> standardVersions, DateTime effectiveFrom, List<string> deliveryAreas, Guid applyingContactId, string applicationType)
        {
            OrganisationId = organisationId;
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
            StandardCode = standardCode;
            StandardReference = standardReference;
            StandardVersions = standardVersions;
            EffectiveFrom = effectiveFrom;
            DeliveryAreas = deliveryAreas;
            ApplyingContactId = applyingContactId;
            ApplicationType = applicationType;
        }

        public CreateOrganisationStandardCommand()
        {
        }
    }
}
