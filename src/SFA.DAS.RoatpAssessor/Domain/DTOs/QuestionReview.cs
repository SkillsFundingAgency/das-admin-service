using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.RoatpAssessor.Configuration;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Domain.DTOs
{
    public class QuestionReview
    {
        public string Ukprn {get;set;}
        public string OrganisationName { get; set; }
        public QuestionConfig QuestionConfig { get; set; }
        public List<List<Answer>> Answers { get; set; }
        public Outcome Outcome { get; set; }
    }
}
