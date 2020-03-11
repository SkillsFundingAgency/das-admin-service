using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class OrganisationStatusViewModel : RoatpGatewayPageViewModel
    {
        public DateTime? ApplicationSubmittedOn { get; set; }
        public DateTime SourcesCheckedOn { get; set; }

        public string UkrlpData { get; set; }
        public string CompaniesHouseData { get; set; }
        public string CharityCommissionData { get; set; }
    }
}
