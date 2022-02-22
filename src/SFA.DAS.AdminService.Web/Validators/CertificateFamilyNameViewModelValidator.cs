using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateFamilyNameViewModelValidator : AbstractValidator<CertificateFamilyNameViewModel>
    {
        public CertificateFamilyNameViewModelValidator()
        {
            RuleFor(vm => vm.FamilyName).NotEmpty().WithMessage("Enter the apprentice's family name");
        }
    }
}
