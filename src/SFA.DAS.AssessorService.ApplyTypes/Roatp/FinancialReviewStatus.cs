﻿namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public static class FinancialReviewStatus
    {
        public const string Draft = "Draft";
        public const string New = "New";
        public const string InProgress = "In Progress";

        public const string ClarificationSent = "Clarification Sent";
        public const string Resubmitted = "Resubmitted";

        public const string Pass = "Pass";       
        public const string Fail = "Fail";
        public const string Exempt = "Exempt";
    }
}
