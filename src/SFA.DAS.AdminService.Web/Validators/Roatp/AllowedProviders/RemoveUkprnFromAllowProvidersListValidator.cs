using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.AllowedProviders
{
    public class RemoveUkprnFromAllowProvidersListValidator : AbstractValidator<RemoveUkprnFromAllowedProvidersListViewModel>
    {
        public RemoveUkprnFromAllowProvidersListValidator()
        {
            RuleFor(x => x.AllowedProvider).NotEmpty().WithMessage("Could not locate UKPRN");

            RuleFor(x => x.Confirm).NotEmpty().OverridePropertyName("Confirm-Yes").WithMessage("Select if you're sure you want to remove this UKPRN");
        }
    }
}
