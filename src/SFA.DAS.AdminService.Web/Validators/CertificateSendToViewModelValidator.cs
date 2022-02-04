using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateSendToViewModelValidator : AbstractValidator<CertificateSendToViewModel>
    {
        public CertificateSendToViewModelValidator()
        {
            RuleFor(vm => vm.SendTo).NotEmpty()
                .WithMessage("Select apprentice or employer");
        }
    }
}