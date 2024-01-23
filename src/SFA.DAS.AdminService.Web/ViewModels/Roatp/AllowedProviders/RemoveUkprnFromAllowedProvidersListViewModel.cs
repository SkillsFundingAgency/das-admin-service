using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.AllowedProviders;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders
{
    public class RemoveUkprnFromAllowedProvidersListViewModel
    {
        public AllowedProvider AllowedProvider { get; set; }
        public bool? Confirm { get; set; }
    }
}
