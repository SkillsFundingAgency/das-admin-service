using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateFirstNameViewModelValidator : AbstractValidator<CertificateFirstNameViewModel>
    {
        public CertificateFirstNameViewModelValidator()
        {
            RuleFor(vm => vm.FirstName).NotEmpty().WithMessage("Enter the apprentice’s first name");
        }
    }
}
