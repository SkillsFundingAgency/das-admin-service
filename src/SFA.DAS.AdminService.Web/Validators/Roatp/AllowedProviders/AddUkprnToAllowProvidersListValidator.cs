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

            RuleFor(x => x.Ukprn).NotEmpty().WithMessage("Enter UKPRN")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ukprn).CustomAsync(async (ukprnInput, context, cancellationToken) =>
                    {
                        if (!int.TryParse(ukprnInput, out var ukprn))
                        {
                            context.AddFailure("Enter a valid UKPRN");
                        }
                        else if (ukprn < 10000000 || ukprn > 19999999)
                        {
                            context.AddFailure("Enter a UKPRN using 8 numbers");
                        }
                        else if (await _applyApiClient.GetAllowedProviderDetails(ukprn) != null)
                        {
                            context.AddFailure("UKPRN exists in the allow list");
                        }
                    });
                });

            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Enter start date")
                .DependentRules(() =>
                {
                    RuleFor(x => x.StartDate).LessThanOrEqualTo(x => x.EndDate)
                        .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                        .WithMessage("The start date must be before the end date");
                });

            RuleFor(x => x.EndDate).NotEmpty().WithMessage("Enter end date");
        }
    }
}
