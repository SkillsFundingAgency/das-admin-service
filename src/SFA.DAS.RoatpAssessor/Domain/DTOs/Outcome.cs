using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Domain.DTOs
{
    public class Outcome
    {
        public Guid SectionId { get; set; }
        public string PageId { get; set; }
        public string QuestionId { get; set; }
        public List<Check> Checks { get; set; } 
        public string Result { get; set; }
        public string Message { get; set; }
    }

    public static class OutcomeResult
    {
        public const string InProgress = "InProgress";
        public const string Pass = "Pass";
        public const string Reject = "Reject";
    }

    public class Check
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public static class CheckValue
    {
        public static string Yes = "Yes";
        public static string No = "No";
    }
}
