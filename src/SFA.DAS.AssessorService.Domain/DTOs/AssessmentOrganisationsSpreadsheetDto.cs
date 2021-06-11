using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.DTOs
{
    public class AssessmentOrganisationsSpreadsheetDto
        {
            public List<DeliveryArea> DeliveryAreas { get; set; }
            public List<TypeOfOrganisation> OrganisationTypes { get; set; }
            public List<EpaOrganisation> Organisations { get; set; }
            public List<EpaOrganisationStandard> OrganisationStandards { get; set; }
            public List<EpaOrganisationStandardDeliveryArea> OrganisationStandardDeliveryAreas { get; set; }
            public List<OrganisationContact> Contacts { get; set; }
        }
}