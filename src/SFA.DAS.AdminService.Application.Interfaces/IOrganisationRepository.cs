using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Application.Interfaces
{
    public interface IOrganisationRepository
    {     
        Task<Organisation> CreateNewOrganisation(Organisation organisation);
        Task<Organisation> UpdateOrganisation(Organisation updateOrganisationDomainModel);
        Task Delete(string endPointAssessorOrganisationId);
    }
}