using System;

namespace SFA.DAS.RoatpAssessor.Domain.DTOs
{
    public class Outcome
    {
        public Guid SectionId { get; set; }
        public string PageId { get; set; }
        public string QuestionId { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }

    public static class OutcomeResult
    {
        public const string InProgress = "InProgress";
        public const string Pass = "Pass";
        public const string Reject = "Reject";
    }
}
