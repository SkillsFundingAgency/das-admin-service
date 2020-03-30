﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class OrganisationRiskViewModel :  RoatpGatewayPageViewModel
    {
        public DateTime? ApplicationSubmittedOn { get; set; }
        public DateTime? SourcesCheckedOn { get; set; }

        public string OrganisationType { get; set; }
        public string TradingName { get; set; }
    }
}