using FluentValidation;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.AllowedProviders
{
    public class AddUkprnToAllowProvidersListValidator : AbstractValidator<AddUkprnToAllowedProvidersListViewModel>
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;

        public AddUkprnToAllowProvidersListValidator(IRoatpApplicationApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;

            RuleFor(x => x.Ukprn).NotEmpty().WithMessage("Enter a UKPRN")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ukprn).Custom((ukprnInput, context) =>
                    {
                        if (!int.TryParse(ukprnInput, out var ukprn))
                        {
                            context.AddFailure("Enter a valid UKPRN");
                        }
                        else if (ukprn < 10000000 || ukprn > 19999999)
                        {
                            context.AddFailure("Enter a valid UKPRN using 8 numbers");
                        }
                        else if (_applyApiClient.GetAllowedProviderDetails(ukprn).Result != null)
                        {
                            context.AddFailure($"UKPRN {ukprn} exists in the allow list");
                        }
                    });
                });

            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Select an invitation start date")
                .DependentRules(() =>
                {
                    RuleFor(x => x.StartDate).LessThanOrEqualTo(x => x.EndDate)
                        .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                        .WithMessage("The invitation end date must be on or after the start date");
                });

            RuleFor(x => x.EndDate).NotEmpty().WithMessage("Select an invitation end date");
        }
    }
}
