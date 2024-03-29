﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    using System;
    using System.Runtime.Serialization;
    using MediatR;

    public class UpdateOrganisationTypeRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public int OrganisationTypeId { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
