using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Register
{
    public class RegisterAddStandardVersionViewModel
    {
        public string OrganisationId { get; set; }
        public int OrganisationStandardId { get; set; }
        public string Title { get; set; }
        public string IfateReferenceNumber { get; set; }
        public string Version { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateApproved { get; set; }
        public string Status { get; set; }

        public string EffectiveFromDay { get; set; }
        public string EffectiveFromMonth { get; set; }
        public string EffectiveFromYear { get; set; }
        public string EffectiveToDay { get; set; }
        public string EffectiveToMonth { get; set; }
        public string EffectiveToYear { get; set; }
    }
}
