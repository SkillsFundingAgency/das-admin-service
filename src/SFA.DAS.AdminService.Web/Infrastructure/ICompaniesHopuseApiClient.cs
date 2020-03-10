using System.Threading.Tasks;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface ICompaniesHouseApiClient
    {
        Task<CompaniesHouseSummary> GetCompanyDetails(string companiesHouseNumber);
    }
}
