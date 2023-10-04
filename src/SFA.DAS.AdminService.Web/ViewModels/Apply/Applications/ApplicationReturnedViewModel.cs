using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationReturnedViewModel : BackViewModel
    {
        public int SequenceNo { get; }
        public string StandardDescription { get; }
        public string ReturnType { get; }
        public List<string> WarningMessages { get; set; }
        public string OrganisationName { get; }

        public ApplicationReturnedViewModel(int sequenceNo, string standardDescription, string returnType, string organisationName, List<string> warningMessages, string backAction, string backController, string backOrganisationId)
            : base (backAction, backController, backOrganisationId)
        {
            SequenceNo = sequenceNo;
            StandardDescription = standardDescription;
            ReturnType = returnType;
            WarningMessages = warningMessages;
            OrganisationName = organisationName;
        }
    }
}
