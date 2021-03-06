﻿using System.Runtime.Serialization;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp
{
    using System;

    public class UpdateOrganisationLegalNameViewModel
    {
        public string CurrentLegalName { get; set; }
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public string LegalName { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
