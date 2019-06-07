using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateRecipientViewModelValidator : AbstractValidator<CertificateRecipientViewModel>
    {
        public CertificateRecipientViewModelValidator()
        {
            RuleFor(vm => vm.Name).NotEmpty().WithMessage("Enter a name");           
        }
    }
}