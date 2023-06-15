using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;
using System;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public interface IRegisterUserOrchestrator
    {
        Task<RegisterViewAndEditUserViewModel> GetUserViewModel(Guid contactId);
    }
}
