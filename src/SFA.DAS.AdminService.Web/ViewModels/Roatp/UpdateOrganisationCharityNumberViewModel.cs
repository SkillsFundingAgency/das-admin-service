﻿using System;
using System.Runtime.Serialization;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp
{
    public class UpdateOrganisationCharityNumberViewModel
    {
        [DataMember]
        public string CharityNumber { get; set; }

        [DataMember]
        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
