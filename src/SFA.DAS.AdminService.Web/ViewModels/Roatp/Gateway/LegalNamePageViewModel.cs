using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class LegalNamePageViewModel: RoatpGatewayPageViewModel
    {
        public DateTime? ApplicationSubmittedOn { get; set; }
        public DateTime? SourcesCheckedOn { get; set; }
        public string ApplyLegalName { get; set; }
        public string UkrlpLegalName { get; set; }
        public string CompaniesHouseLegalName { get; set; }
        public string CharityCommissionLegalName { get; set; }
    }
}
