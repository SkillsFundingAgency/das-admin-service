using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class OrganisationStatusViewModel : RoatpGatewayPageViewModel
    {
        public string UkrlpStatus { get; set; }
        public string CompaniesHouseStatus { get; set; }
        public string CharityCommissionStatus { get; set; }
    }
}
