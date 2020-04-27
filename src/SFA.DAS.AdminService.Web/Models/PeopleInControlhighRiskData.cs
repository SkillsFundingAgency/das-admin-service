using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Models
{
    public class PeopleInControlHighRiskData
    {
        public string Heading { get; set; }
        public List<PersonInControl> PeopleInControl { get; set; }
    }
}
