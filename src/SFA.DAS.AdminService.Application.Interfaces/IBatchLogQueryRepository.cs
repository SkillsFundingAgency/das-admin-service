using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Application.Interfaces
{
    public interface IBatchLogQueryRepository
    {
        Task<BatchLog> GetLastBatchLog();
    }
}