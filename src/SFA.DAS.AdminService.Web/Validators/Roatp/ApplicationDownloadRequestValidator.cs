using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public class ApplicationDownloadRequestValidator: AbstractValidator<ApplicationDownloadViewModel>
    {
        public ApplicationDownloadRequestValidator()
        {
            RuleFor(x => x.FromDate).NotEmpty().WithMessage("Select a from date")
                .DependentRules(() =>
                {
                    RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate)
                        .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
                        .WithMessage("Date must be on or after the from date");
                });

            RuleFor(x => x.ToDate).NotEmpty().WithMessage("Select a to date");
        }
    }
}
