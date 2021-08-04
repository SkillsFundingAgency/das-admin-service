using SFA.DAS.AssessorService.ApplyTypes.Roatp.AllowedProviders;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders
{
    public class RemoveUkprnFromAllowedProvidersListViewModel
    {
        public AllowedProvider AllowedProvider { get; set; }
        public bool? Confirm { get; set; }
    }
}
