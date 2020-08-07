﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Commands
{
    public class CreateOrganisationStandardCommand
    {
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }

        public int StandardCode { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime DateStandardApprovedOnRegister { get; } = DateTime.Now.Date;
        public List<string> DeliveryAreas { get; set; }

        public Guid ApplyingContactId { get; set; }

        public CreateOrganisationStandardCommand(Guid organisationId, string endPointAssessorOrganisationId,
            int standardCode, DateTime effectiveFrom, List<string> deliveryAreas, Guid applyingContactId)
        {
            OrganisationId = organisationId;
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
            StandardCode = standardCode;
            EffectiveFrom = effectiveFrom;
            DeliveryAreas = deliveryAreas;
            ApplyingContactId = applyingContactId;
        }

        public CreateOrganisationStandardCommand()
        {
        }
    }
}
