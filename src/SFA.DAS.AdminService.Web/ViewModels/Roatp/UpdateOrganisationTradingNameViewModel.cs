namespace SFA.DAS.AdminService.Web.ViewModels.Roatp
{
    using System;

    public class UpdateOrganisationTradingNameViewModel
    {
        public string LegalName { get; set; }
        public Guid OrganisationId { get; set; }
        public string TradingName { get; set; }
        public string UpdatedBy { get; set; }
    }
}
