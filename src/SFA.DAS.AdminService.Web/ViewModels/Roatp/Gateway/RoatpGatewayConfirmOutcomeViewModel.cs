using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoatpGatewayConfirmOutcomeViewModel
    {
        public Guid ApplicationId { get; set; }
        public string GatewayReviewStatus { get; set; }
        public string GatewayReviewComment { get; set; }

        [Required(ErrorMessage = "Select if you are sure you want to pass this application")]
        public string ConfirmGatewayOutcome { get; set; }
        public string CssFormGroupError { get; set; }
    }
}
