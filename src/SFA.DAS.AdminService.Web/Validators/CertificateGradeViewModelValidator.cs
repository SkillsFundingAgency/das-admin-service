using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class CertificateGradeViewModelValidator : AbstractValidator<CertificateGradeViewModel>
    {
        public CertificateGradeViewModelValidator()
        {
            RuleFor(vm => vm.SelectedGrade).NotEmpty().WithMessage("Select the grade the apprentice achieved");
        }
    }
}