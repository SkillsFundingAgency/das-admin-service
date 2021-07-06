using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Register
{
    public class RegisterEditOrganisationStandardVersionViewModel
    {
        public string Standard { get; set; }
        public string Reference { get; set; }
        public string Version { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateApprovedOnRegister { get; set; }
        public string Status { get; set; }

        public string EffectiveFromDay { get; set; }
        public string EffectiveFromMonth { get; set; }
        public string EffectiveFromYear { get; set; }
        public string EffectiveToDay { get; set; }
        public string EffectiveToMonth { get; set; }
        public string EffectiveToYear { get; set; }
        public string OrganisationStandardId { get; set; }
    }
}
