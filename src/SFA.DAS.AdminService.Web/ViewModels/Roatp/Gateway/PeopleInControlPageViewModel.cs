using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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

        public TabularData PeopleWithSignificantControl { get; set; }
        public TabularData Trustees { get; set; }
        public TabularData WhosInControl { get; set; }

        public PeopleInControlData CompanyDirectorsData { get; set; }
        public PeopleInControlData PscData { get; set; }

        public PeopleInControlData TrusteeData { get; set; }

        public PeopleInControlData WhosInControlData { get; set; }
    }

    public class PeopleInControlData
    {
        public string Caption { get; set; }
        public string ExternalSourceHeading { get; set; }
        public string SubmittedApplicationHeading { get; set; }
        public List<PersonInControl> FromExternalSource { get; set; }
        public List<PersonInControl> FromSubmittedApplication { get; set; }
    }
}
