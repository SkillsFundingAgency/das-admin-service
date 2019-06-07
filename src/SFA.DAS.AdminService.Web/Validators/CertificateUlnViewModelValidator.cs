using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateUlnViewModelValidator : AbstractValidator<CertificateUlnViewModel>
    {
        public CertificateUlnViewModelValidator()
        {
            RuleFor(x => x.Uln)
                .NotEmpty().WithMessage("Enter the apprentice\'s ULN")
                .Matches(@"^\d{10}$").WithMessage("The apprentice\'s ULN should contain exactly 10 numbers");
        }
    }
}
