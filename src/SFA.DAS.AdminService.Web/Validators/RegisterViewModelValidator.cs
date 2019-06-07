using FluentValidation;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(vm => vm.SearchString).NotEmpty().WithMessage("Enter 2 or more characters")
                .Must(x => x?.Trim().Length > 1)
                .WithMessage("Enter 2 or more characters");

        }
    }
}
