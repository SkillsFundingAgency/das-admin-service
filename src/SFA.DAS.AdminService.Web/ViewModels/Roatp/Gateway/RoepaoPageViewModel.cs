using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoepaoPageViewModel : RoatpGatewayPageViewModel
    {
        public DateTime? ApplicationSubmittedOn { get; set; }
        public DateTime? SourcesCheckedOn { get; set; }

        public string ApplyLegalName { get; set; }
    }
}
