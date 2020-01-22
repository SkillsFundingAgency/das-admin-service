using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpOrganisationApiClient : IRoatpOrganisationApiClient
    {
        public Task<Organisation> GetOrganisation(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Contact>> GetOrganisationContacts(Guid organisationId)
        {
            throw new NotImplementedException();
        }
    }
}
