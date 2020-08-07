using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class PeopleInControlPageViewModel : RoatpGatewayPageViewModel
    {
        public string TypeOfOrganisation { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public PeopleInControlData CompanyDirectorsData { get; set; }
        public PeopleInControlData PscData { get; set; }
        public PeopleInControlData TrusteeData { get; set; }
        public PeopleInControlData WhosInControlData { get; set; }
    }
}
