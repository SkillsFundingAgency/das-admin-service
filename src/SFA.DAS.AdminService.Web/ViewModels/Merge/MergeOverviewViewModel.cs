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

        public MergeOverviewViewModel() { }
        public MergeOverviewViewModel(MergeRequest mergeRequest)
        {
            PrimaryEpaoId = mergeRequest?.PrimaryEpao?.Id;
            PrimaryEpaoName = mergeRequest?.PrimaryEpao?.Name;
            SecondaryEpaoId = mergeRequest?.SecondaryEpao?.Id;
            SecondaryEpaoName = mergeRequest?.SecondaryEpao?.Name;
            SecondaryEpaoEffectiveTo = mergeRequest?.SecondaryEpaoEffectiveTo;

            var previousCommand = mergeRequest?.PreviousCommand;

            if (previousCommand.CommandName == SessionCommands.ConfirmPrimaryEpao)
            {
                BackLinkType = "primary";
                BackLinkAction = "ConfirmEpao";
                BackLinkEpaoId = previousCommand.EpaoId;
            }
            else if (previousCommand.CommandName == SessionCommands.ConfirmSecondaryEpao)
            {
                BackLinkType = "secondary";
                BackLinkAction = "ConfirmEpao";
                BackLinkEpaoId = previousCommand.EpaoId;
            }
            else if (previousCommand.CommandName == SessionCommands.SetSecondaryEpaoEffectiveTo)
            {
                BackLinkAction = "SetSecondaryEpaoEffectiveToDate";
            }
        }
    }
}
