using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Register
{
    public class RegisterEditOrganisationStandardVersionViewModel
    {
        public string OrganisationId { get; set; }
        public string Standard { get; set; }
        public string Reference { get; set; }
        public string Version { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateApprovedOnRegister { get; set; }
        public string Status { get; set; }

        public int EffectiveFromDay { get; set; }
        public int EffectiveFromMonth { get; set; }
        public int EffectiveFromYear { get; set; }
        public int EffectiveToDay { get; set; }
        public int EffectiveToMonth { get; set; }
        public int EffectiveToYear { get; set; }
    }
}
