using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationReturnedViewModel
    {
        public Guid ApplicationId { get; }
        public string ApplicationType { get; }
        public int SequenceNo { get; }

        public List<string> WarningMessages { get; set; }
        public ApplicationReturnedViewModel(Guid applicationId, string applicationType, int sequenceNo, List<string> warningMessages)
        {
            ApplicationId = applicationId;
            ApplicationType = applicationType;
            SequenceNo = sequenceNo;
            WarningMessages = warningMessages;
        }
    }
}
