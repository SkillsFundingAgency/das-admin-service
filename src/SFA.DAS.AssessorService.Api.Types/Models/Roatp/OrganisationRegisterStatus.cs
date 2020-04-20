using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class OrganisationRegisterStatus
    {
        public bool UkprnOnRegister { get; set; }
        public Guid? OrganisationId { get; set; }
        public int? ProviderTypeId { get; set; }
        public int? StatusId { get; set; }
        public int? RemovedReasonId { get; set; }
        public DateTime? StatusDate { get; set; }
    }
}
