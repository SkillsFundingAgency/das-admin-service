using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateGivenNamesViewModelValidator : AbstractValidator<CertificateGivenNamesViewModel>
    {
        public CertificateGivenNamesViewModelValidator()
        {
            RuleFor(vm => vm.GivenNames).NotEmpty().WithMessage("Enter the apprentice's given names");
        }
    }
}
