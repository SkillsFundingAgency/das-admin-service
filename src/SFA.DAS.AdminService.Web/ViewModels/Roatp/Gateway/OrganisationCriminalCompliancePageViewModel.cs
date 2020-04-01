using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class OrganisationCriminalCompliancePageViewModel : RoatpGatewayPageViewModel
    {
        //public string ApplyLegalName { get; set; } // We had it in RoatpGatewayPageViewModel

        //public string PageTitle { get; set; }
        public string QuestionText { get; set; }

        public string ComplianceCheckQuestionId { get; set; }
        public string FurtherInformationQuestionId { get; set; }

        public string ComplianceCheckAnswer { get; set; }
        public string FurtherInformationAnswer { get; set; }

        public string PostBackAction { get; set; }
    }
}
