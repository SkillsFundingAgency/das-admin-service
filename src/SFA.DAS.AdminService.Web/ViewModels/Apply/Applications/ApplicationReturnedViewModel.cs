using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationReturnedViewModel : BackViewModel
    {
        public int SequenceNo { get; }
        public string StandardDescription { get; }
        public string ReturnType { get; }
        public List<string> WarningMessages { get; set; }

        public ApplicationReturnedViewModel(int sequenceNo, string standardDescription, string returnType, List<string> warningMessages)
            : this(sequenceNo, standardDescription, returnType, warningMessages, string.Empty, string.Empty, string.Empty)
        {
        }

        public ApplicationReturnedViewModel(int sequenceNo, string standardDescription, string returnType, List<string> warningMessages, string backAction, string backController, string backOrganisationId)
            : base (backAction, backController, backOrganisationId)
        {
            SequenceNo = sequenceNo;
            StandardDescription = standardDescription;
            ReturnType = returnType;
            WarningMessages = warningMessages;
        }
    }
}
