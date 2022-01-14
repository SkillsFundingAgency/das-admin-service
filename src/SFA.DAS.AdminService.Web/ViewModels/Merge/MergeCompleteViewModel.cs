using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class MergeCompleteViewModel
    {
        public string PrimaryEpaoName { get; set; }
        public string SecondaryEpaoName { get; set; }
        public DateTime SecondaryEpaoEffectiveTo { get; set; }
    }
}
