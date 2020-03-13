﻿using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class TradingNamePageViewModel: RoatpGatewayPageViewModel
    {
        public DateTime? ApplicationSubmittedOn { get; set; }
        public DateTime SourcesCheckedOn { get; set; }
        public string ApplyTradingName { get; set; }
        public string UkrlpTradingName { get; set; }
    }
}
