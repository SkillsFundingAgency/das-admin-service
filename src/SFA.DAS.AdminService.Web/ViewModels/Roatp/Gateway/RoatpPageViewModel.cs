using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoatpPageViewModel : RoatpGatewayPageViewModel
    {
        public string ApplyProviderRoute { get; set; }

        public bool RoatpUkprnOnRegister { get; set; }
        public string RoatpProviderRoute { get; set; }
        public DateTime? RoatpStatusDate { get; set; }
        public string RoatpStatus { get; set; }
        public string RoatpRemovedReason { get; set; }
    }
}
