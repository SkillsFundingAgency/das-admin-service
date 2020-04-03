using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public interface IPeopleInControlOrchestrator
    {
        Task<PeopleInControlPageViewModel> GetPeopleInControlViewModel(GetPeopleInControlRequest getPeopleInControlRequest);
        Task<PeopleInControlHighRiskPageViewModel> GetPeopleInControlHighRiskViewModel(GetPeopleInControlRequest getPeopleInControlRequest);
    }
}
