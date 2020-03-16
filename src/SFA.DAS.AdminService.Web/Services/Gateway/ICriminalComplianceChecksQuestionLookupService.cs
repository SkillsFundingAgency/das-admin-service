
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public interface ICriminalComplianceChecksQuestionLookupService
    {
        CriminalCompliancePageDetails GetPageDetailsForGatewayCheckPageId(string gatewayCheckPageId);
    }
}
