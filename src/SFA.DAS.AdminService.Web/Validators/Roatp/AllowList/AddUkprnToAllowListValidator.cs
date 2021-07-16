using FluentValidation;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowList;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.AllowList
{
    public class AddUkprnToAllowListValidator : AbstractValidator<AddUkprnToAllowListViewModel>
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;

        public AddUkprnToAllowListValidator(IRoatpApplicationApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;

            RuleFor(x => x.Ukprn).NotEmpty().WithMessage("Enter UKPRN")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ukprn).CustomAsync(async (ukprnInput, context, cancellationToken) =>
                    {
                        if(!long.TryParse(ukprnInput, out var ukprn))
                        {
                            context.AddFailure("Enter a valid UKPRN");
                        }
                        else if (ukprn < 10000000 || ukprn > 19999999)
                        {
                            context.AddFailure("Enter a UKPRN using 8 numbers");
                        }
                        else
                        {
                            var allowList = await _applyApiClient.GetAllowedUkprns(null, null);

                            if(allowList.Any(x => x.Ukprn == ukprn.ToString()))
                            {
                                context.AddFailure("UKPRN exists in the allow list");
                            }
                        }
                        await Task.CompletedTask;
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
