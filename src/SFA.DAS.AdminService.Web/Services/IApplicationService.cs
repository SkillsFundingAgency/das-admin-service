using SFA.DAS.AdminService.Web.Domain.Apply;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IApplicationService
    {
        Task<WithdrawalApplicationDetails> GetWithdrawalApplicationDetails(Guid applicationId);
        Task<ApplicationDetails> GetApplicationsDetails(Guid applicationId);
    }
}