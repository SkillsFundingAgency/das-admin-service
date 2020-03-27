using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class PeopleInControlPageViewModel : RoatpGatewayPageViewModel
    {
        public DateTime? ApplicationSubmittedOn { get; set; }
        public DateTime? SourcesCheckedOn { get; set; }
        public string ApplyLegalName { get; set; }
        public string TypeOfOrganisation { get; set; }

        public TabularData CompanyDirectors { get; set; }
        public TabularData PeopleWithSignificantControl { get; set; }
        public TabularData Trustees { get; set; }
        public TabularData WhosInControl { get; set; }
    }
}
