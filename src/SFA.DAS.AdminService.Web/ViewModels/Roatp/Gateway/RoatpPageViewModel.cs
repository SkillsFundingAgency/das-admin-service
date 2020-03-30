﻿using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoatpPageViewModel : RoatpGatewayPageViewModel
    {
        public DateTime? ApplicationSubmittedOn { get; set; }
        public DateTime? SourcesCheckedOn { get; set; }

        public string ApplyLegalName { get; set; }
        public string ApplyProviderRoute { get; set; }

        public bool RoatpUkprnOnRegister { get; set; }
        public string RoatpProviderRoute { get; set; }
        public DateTime? RoatpStatusDate { get; set; }
        public string RoatpStatus { get; set; }
        public string RoatpRemoveReason { get; set; }
    }
}