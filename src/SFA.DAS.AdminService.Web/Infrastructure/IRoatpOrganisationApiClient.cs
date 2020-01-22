using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IRoatpOrganisationApiClient
    {
        Task<Organisation> GetOrganisation(Guid id);
        Task<List<Contact>> GetOrganisationContacts(Guid organisationId);
    }
}
