namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    using System;
    using System.Runtime.Serialization;
    using MediatR;

    public class UpdateOrganisationParentCompanyGuaranteeRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public bool ParentCompanyGuarantee { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
