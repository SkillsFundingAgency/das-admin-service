using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;
using System;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public interface IUserViewModelOrchestrator
    {
        Task<RegisterViewAndEditUserViewModel> GetUserViewModel(Guid contactId);
    }
}
