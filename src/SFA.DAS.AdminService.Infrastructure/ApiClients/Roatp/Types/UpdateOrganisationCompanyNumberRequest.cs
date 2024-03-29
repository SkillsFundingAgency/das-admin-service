﻿using System;
using System.Runtime.Serialization;
using MediatR;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    public class UpdateOrganisationCompanyNumberRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public string CompanyNumber { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
