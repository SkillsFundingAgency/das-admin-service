using SFA.DAS.AdminService.Web.Models.Merge;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class MergeOverviewViewModel
    {
        public string PrimaryEpaoId { get; set; }
        public string PrimaryEpaoName { get; set; }

        public string SecondaryEpaoId { get; set; }
        public string SecondaryEpaoName { get; set; }

        public DateTime? SecondaryEpaoEffectiveTo { get; set; }

        public string BackLinkAction { get; set; }
        public string BackLinkType { get; set; }
        public string BackLinkEpaoId { get; set; }

        public SessionCommand PreviousCommand { get; set; }
    }
}
