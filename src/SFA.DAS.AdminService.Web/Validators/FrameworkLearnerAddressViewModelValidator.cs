using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.CompaniesHouse;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class FrameworkLearnerAddressViewModelValidator : AbstractValidator<FrameworkLearnerAddressViewModel>
    {
        private readonly string invalidCharacters = @"@#$^=+\\/<%>%";
        public FrameworkLearnerAddressViewModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(vm => vm.AddressLine1)
                .NotEmpty()
                .WithMessage("Enter address line 1, typically the building and street")
                .Must(BeValidCharacters)
                .WithMessage($"Address line 1 must not include any special characters: {invalidCharacters}");

            RuleFor(vm => vm.AddressLine2)
                .Must(BeValidCharacters)
                .WithMessage($"Address line must not include any special characters: {invalidCharacters}");

            RuleFor(vm => vm.TownOrCity)
                .NotEmpty()
                .WithMessage("Enter town or city")
                .Must(BeValidCharacters)
                .WithMessage($"Town or city must not include any special characters: {invalidCharacters}");

            RuleFor(vm => vm.County)
                .Must(BeValidCharacters)
                .WithMessage($"County must not include any special characters: {invalidCharacters}");;

            RuleFor(vm => vm.Postcode)
                .NotEmpty()
                .WithMessage("Enter postcode")
                .Must(BeValidCharacters)
                .WithMessage($"Postcode must not include any special characters: {invalidCharacters}")
                .Matches("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$")
                .WithMessage("Enter a valid postcode");
        }

        private bool BeValidCharacters(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
 
            return !value.Any(c => invalidCharacters.Contains(c));
        }
    }
}