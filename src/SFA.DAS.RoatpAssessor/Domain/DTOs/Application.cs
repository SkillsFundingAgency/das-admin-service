using System;

namespace SFA.DAS.RoatpAssessor.Domain.DTOs
{
    public class Application
    {
        public Guid Id { get; set; }
        public string ApplicationStatus { get; set; }

        //todo: These values need to be trapped at point of application submission
        public string OrganisationName { get; set; } = "My TODO ORG";
        public string Ukprn { get; set; } = "12345678";
        public string ApplicationRef { get; set; } = "APR00000001";
        public string ProviderRoute { get; set; } = "Main (todo)";
        public DateTime SubmittedAt { get; set; } = DateTime.Parse("2018-01-01");
    }
}
