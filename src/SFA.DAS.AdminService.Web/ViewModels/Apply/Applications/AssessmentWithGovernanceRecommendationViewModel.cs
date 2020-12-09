using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class AssessmentWithGovernanceRecommendationViewModel : BackViewModel
    {
        public GovernanceRecommendation Recommendation { get; set; }
    }
}
