using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationReturnedViewModel
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }

        public List<string> WarningMessages { get; set; }
        public ApplicationReturnedViewModel(Guid applicationId, int sequenceNo, List<string> warningMessages)
        {
            ApplicationId = applicationId;
            SequenceNo = sequenceNo;
            WarningMessages = warningMessages;
        }
    }
}
