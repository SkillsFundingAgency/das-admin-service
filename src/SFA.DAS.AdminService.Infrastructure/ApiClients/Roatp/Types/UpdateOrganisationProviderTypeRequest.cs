namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    using MediatR;
    using System;
    using System.Runtime.Serialization;

    public class UpdateOrganisationProviderTypeRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public int ProviderTypeId { get; set; }
        [DataMember]
        public int OrganisationTypeId { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
