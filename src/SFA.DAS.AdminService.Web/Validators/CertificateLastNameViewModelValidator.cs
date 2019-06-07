using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateLastNameViewModelValidator : AbstractValidator<CertificateLastNameViewModel>
    {
        public CertificateLastNameViewModelValidator()
        {
            RuleFor(vm => vm.LastName).NotEmpty().WithMessage("Enter the apprentice’s last name");
        }
    }
}
