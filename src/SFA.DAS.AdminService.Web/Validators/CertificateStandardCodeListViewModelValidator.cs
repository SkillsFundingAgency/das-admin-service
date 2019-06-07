using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateStandardCodeListViewModelValidator : AbstractValidator<CertificateStandardCodeListViewModel>
    {
        public CertificateStandardCodeListViewModelValidator()
        {
            RuleFor(vm => vm.SelectedStandardCode).NotEmpty().WithMessage("Select the standard");
        }
    }
}
