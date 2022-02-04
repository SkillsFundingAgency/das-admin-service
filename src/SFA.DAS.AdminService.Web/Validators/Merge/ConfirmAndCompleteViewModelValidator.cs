using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Validators.Merge
{
    public class ConfirmAndCompleteViewModelValidator : AbstractValidator<ConfirmAndCompleteViewModel>
    {
        public ConfirmAndCompleteViewModelValidator()
        {
            RuleFor(vm => vm.AcceptWarning).Equal(true).WithMessage("Select the checkbox");
        }
    }
}
