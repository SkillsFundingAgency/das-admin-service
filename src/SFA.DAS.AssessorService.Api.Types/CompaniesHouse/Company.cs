using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.CompaniesHouse
{
    public class Company
    {
        public string CompanyNumber { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Address RegisteredOfficeAddress { get; set; }
        public IEnumerable<string> NatureOfBusiness { get; set; }
        public DateTime? IncorporatedOn { get; set; }
        public DateTime? DissolvedOn { get; set; }
        public bool? IsLiquidated { get; set; }
        public IEnumerable<string> PreviousNames { get; set; }

        public Accounts Accounts { get; set; }
        public IEnumerable<Officer> Officers { get; set; }
        public IEnumerable<PersonWithSignificantControl> PeopleWithSignificantControl { get; set; }
    }
}
