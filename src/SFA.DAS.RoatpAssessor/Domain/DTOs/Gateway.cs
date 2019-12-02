using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Domain.DTOs
{
    public class Gateway
    {
        public Guid ApplicationId { get; set; }
        public string Status { get; set; }
        public string AssignedToName { get; set; }
        public List<Outcome> Outcomes { get; set; }
        
        //todo: These values will be part of the Application so need to join Gateway to Applications table
        public string OrganisationName { get; set; } = "My TODO ORG";
        public string Ukprn { get; set; } = "12345678";
        public string ApplicationRef { get; set; } = "APR00000001";
        public string ProviderRoute { get; set; } = "Main (todo)";
        public DateTime SubmittedAt { get; set; } = DateTime.Parse("2018-01-01");
    }
}
