using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class PeopleInControlHighRiskPageViewModel : RoatpGatewayPageViewModel
    {
        public string TypeOfOrganisation { get; set; }
        public PeopleInControlHighRiskData CompanyDirectorsData { get; set; }
        public PeopleInControlHighRiskData PscData { get; set; }
        public PeopleInControlHighRiskData TrusteeData { get; set; }
        public PeopleInControlHighRiskData WhosInControlData { get; set; }
    }
}
