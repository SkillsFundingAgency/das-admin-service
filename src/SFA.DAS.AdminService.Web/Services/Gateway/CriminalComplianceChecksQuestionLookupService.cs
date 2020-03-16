using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class CriminalComplianceChecksQuestionLookupService : ICriminalComplianceChecksQuestionLookupService
    {
        // TODO: consolidate this configuration information with the list of sequences / pages set up in GetApplicationOverviewHandler 
        // and store in config/database

        public CriminalCompliancePageDetails GetPageDetailsForGatewayCheckPageId(string gatewayCheckPageId)
        {
            switch(gatewayCheckPageId)
            {
                case GatewayPageIds.CCOrganisationCompositionCreditors:
                    {
                        return new CriminalCompliancePageDetails 
                        { 
                            PageId = RoatpQnaConstants.RoatpSections.CriminalComplianceChecks.PageIds.CompositionCreditors,
                            QuestionId = RoatpQnaConstants.RoatpSections.CriminalComplianceChecks.QuestionIds.CompositionCreditors,
                            Title = "Composition with creditors check"
                        };
                    }
                default:
                    return null;
            }
        }

    }
}
