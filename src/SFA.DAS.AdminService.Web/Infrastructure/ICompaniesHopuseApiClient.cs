using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface ICompaniesHouseApiClient
    {
        Task<AssessorService.Domain.Entities.CompaniesHouseSummary> GetCompanyDetails(string companiesHouseNumber);
    }
}
