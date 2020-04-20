using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Models
{
    public class PeopleInControlData
    {
        public string Caption { get; set; }
        public string ExternalSourceHeading { get; set; }
        public List<PersonInControl> FromExternalSource { get; set; }
        public List<PersonInControl> FromSubmittedApplication { get; set; }
    }
}
