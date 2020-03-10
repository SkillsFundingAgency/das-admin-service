using System.Threading.Tasks;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IApplyServicePassthroughApiClient
    {
        Task<CompaniesHouseSummary> GetCompanyDetails(string companiesHouseNumber);
        Task<Charity> GetCharityDetails(string charityNumber);
    }
}
