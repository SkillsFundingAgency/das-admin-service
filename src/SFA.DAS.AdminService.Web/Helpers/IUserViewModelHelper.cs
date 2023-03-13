using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;
using System;

namespace SFA.DAS.AdminService.Web.Helpers
{
    public interface IUserViewModelHelper
    {
        Task<RegisterViewAndEditUserViewModel> GetUserViewModel(Guid contactId);
    }
}
