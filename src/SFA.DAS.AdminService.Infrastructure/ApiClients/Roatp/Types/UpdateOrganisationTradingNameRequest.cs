namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    using System;
    using System.Runtime.Serialization;
    using MediatR;

    public class UpdateOrganisationTradingNameRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public string TradingName { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
