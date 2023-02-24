using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationSectionStatus
    {
        public const string Submitted = "Submitted";
        public const string InProgress = "In Progress";
        public const string Graded = "Graded";
        public const string Evaluated = "Evaluated";
    }
}
