using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateAddressViewModelValidator : AbstractValidator<CertificateAddressViewModel>
    {
        public CertificateAddressViewModelValidator()
        {
            When(vm => vm.SendTo == AssessorService.Domain.JsonData.CertificateSendTo.Employer, () =>
            {
                RuleFor(vm => vm.Name).NotEmpty()
                    .WithMessage("Enter recipient's name");

                RuleFor(vm => vm.Employer).NotEmpty()
                .WithMessage("Enter an employer");
            });

            RuleFor(vm => vm.AddressLine1).NotEmpty()
                .WithMessage("Enter a building or street");

            RuleFor(vm => vm.City).NotEmpty()
                .WithMessage("Enter a town or city");

            RuleFor(vm => vm.Postcode).NotEmpty()
                .WithMessage("Enter a postcode");

            RuleFor(vm => vm.Postcode)
                .Matches("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$")
                .WithMessage("Enter a valid UK postcode");
        }
    }
}