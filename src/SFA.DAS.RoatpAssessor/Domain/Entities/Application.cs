using System;

namespace SFA.DAS.RoatpAssessor.Domain.Entities
{
    public class Application
    {
        public Guid Id { get; set; }
        public string ApplicationRef => Id.ToString(); //todo we need to create this at time of submit.
        public DateTime SubmittedAt => DateTime.Parse("2018-01-01"); //todo we need to create this at time of submit.
    }
}
