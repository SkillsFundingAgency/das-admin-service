using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationReturnedViewModel : BackViewModel
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }

        public List<string> WarningMessages { get; set; }

        public ApplicationReturnedViewModel(Guid applicationId, int sequenceNo, List<string> warningMessages)
            : this(applicationId, sequenceNo, warningMessages, string.Empty, string.Empty, string.Empty)
        {
        }

        public ApplicationReturnedViewModel(Guid applicationId, int sequenceNo, List<string> warningMessages, string backAction, string backController, string backOrganisationId)
            : base (backAction, backController, backOrganisationId)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            WarningMessages = warningMessages;
        }
    }
}
