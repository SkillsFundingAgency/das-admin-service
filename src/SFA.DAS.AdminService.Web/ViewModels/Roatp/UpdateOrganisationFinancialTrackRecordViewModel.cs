using System;


namespace SFA.DAS.AdminService.Web.ViewModels.Roatp
{
    public class UpdateOrganisationFinancialTrackRecordViewModel
    {
        public Guid OrganisationId { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public string UpdatedBy { get; set; }
        public string LegalName { get; set; }
    }
}
