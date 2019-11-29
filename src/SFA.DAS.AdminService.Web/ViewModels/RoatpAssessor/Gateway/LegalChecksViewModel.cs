namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway
{
    public class LegalChecksViewModel
    {
        public string LegalNameCheck { get; set; }
        public string StatusCheck { get; set; }
        public string AddressCheck { get; set; }

        public OutcomeViewModel Outcome { get; set; }
    }
}
