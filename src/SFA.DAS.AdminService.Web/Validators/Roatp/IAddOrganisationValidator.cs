namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    using System.Threading.Tasks;
    using SFA.DAS.AdminService.Common.Validation;
    using ViewModels.Roatp;

    public interface IAddOrganisationValidator
    {
        Task<ValidationResponse> ValidateOrganisationDetails(AddOrganisationViewModel viewModel);
    }
}
