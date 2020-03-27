using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class WebsiteViewModel : RoatpGatewayPageViewModel
    {
        public DateTime? ApplicationSubmittedOn { get; set; }
        public DateTime? SourcesCheckedOn { get; set; }

        public string SubmittedWebsite { get; set; }
        public string UkrlpWebsite { get; set; }
    }
}
