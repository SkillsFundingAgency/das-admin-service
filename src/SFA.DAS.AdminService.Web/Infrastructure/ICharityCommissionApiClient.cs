using System.Threading.Tasks;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface ICharityCommissionApiClient
    {
        Task<ApiResponse<Charity>> GetCharityDetails(int charityNumber);
    }
}
