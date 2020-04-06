using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoatpGatewayConfirmOutcomeViewModel
    {
        public Guid ApplicationId { get; set; }
        public string GatewayReviewStatus { get; set; }
        public string GatewayReviewComment { get; set; }
    }
}
