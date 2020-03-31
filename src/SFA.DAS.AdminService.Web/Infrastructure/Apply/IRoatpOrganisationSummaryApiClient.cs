using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Models;


namespace SFA.DAS.AdminService.Web.Infrastructure.Apply
{
    public interface IRoatpOrganisationSummaryApiClient
    {
        Task<string> GetTypeOfOrganisation(Guid applicationId);
        Task<List<PersonInControl>> GetDirectors(Guid applicationId);
        Task<List<PersonInControl>> GetDirectorsFromApplyData(Guid applicationId);

        Task<TabularData> GetPersonsWithSignificantControl(Guid applicationId);
        Task<TabularData> GetTrustees(Guid applicationId);
        Task<TabularData> GetPeopleInControl(Guid applicationId);
        Task<TabularData> GetPartners(Guid applicationId);

        Task<string> GetSoleTraderDob(Guid applicationId);

    }
}
