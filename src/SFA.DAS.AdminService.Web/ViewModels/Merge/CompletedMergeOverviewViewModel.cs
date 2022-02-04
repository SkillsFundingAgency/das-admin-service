using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class CompletedMergeOverviewViewModel
    {
        public string PrimaryEpaoName { get; set; }
        public string SecondaryEpaoName { get; set; }
        public DateTime SecondaryEpaoEffectiveToDate { get; set; }
        public DateTime CompletionDate { get; set; }
    }
}
