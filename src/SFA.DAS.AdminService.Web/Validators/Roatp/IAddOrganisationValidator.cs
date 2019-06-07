namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    using System.Threading.Tasks;
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;
    using ViewModels.Roatp;

    public interface IAddOrganisationValidator
    {
        Task<ValidationResponse> ValidateOrganisationDetails(AddOrganisationViewModel viewModel);
    }
}
