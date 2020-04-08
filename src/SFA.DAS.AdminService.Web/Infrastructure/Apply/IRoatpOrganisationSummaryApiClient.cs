using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.Infrastructure.Apply
{
    public interface IRoatpOrganisationSummaryApiClient
    {
        Task<string> GetTypeOfOrganisation(Guid applicationId);
        Task<string> GetCompanyNumber(Guid applicationId);
        Task<string> GetCharityNumber(Guid applicationId);
        Task<List<PersonInControl>> GetDirectorsFromSubmitted(Guid applicationId);
        Task<List<PersonInControl>> GetDirectorsFromCompaniesHouse(Guid applicationId);

        Task<List<PersonInControl>> GetPscsFromSubmitted(Guid applicationId);
        Task<List<PersonInControl>> GetPscsFromCompaniesHouse(Guid applicationId);

        Task<List<PersonInControl>> GetTrusteesFromSubmitted(Guid applicationId);
        Task<List<PersonInControl>> GetTrusteesFromCharityCommission(Guid applicationId);

        Task<List<PersonInControl>> GetWhosInControlFromSubmitted(Guid applicationId);
    }
}
