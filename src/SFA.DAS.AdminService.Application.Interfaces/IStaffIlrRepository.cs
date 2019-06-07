using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Application.Interfaces
{
    public interface IStaffIlrRepository
    {
        Task<IEnumerable<Ilr>> SearchForLearnerByCertificateReference(string certRef);
        Task<IEnumerable<Ilr>> SearchForLearnerByName(string learnerName, int page, int pageSize);
        Task<int> SearchForLearnerByNameCount(string learnerName);
        Task<StaffReposSearchResult> SearchForLearnerByEpaOrgId(StaffSearchRequest searchRequest);
        Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln, int page, int pageSize);
        Task<int> SearchForLearnerByUlnCount(long uln);
    }

    public class StaffSearchRequest : IRequest<StaffSearchResult>
    {
        public StaffSearchRequest(string searchQuery, int page)
        {
            SearchQuery = searchQuery;
            Page = page;
        }

        public string SearchQuery { get; set; }
        public int Page { get; }
    }

    public class StaffReposSearchResult
    {
        public IEnumerable<Ilr> PageOfResults { get; set; }
        public bool DisplayEpao { get; set; }
        public int TotalCount { get; set; }
    }
}