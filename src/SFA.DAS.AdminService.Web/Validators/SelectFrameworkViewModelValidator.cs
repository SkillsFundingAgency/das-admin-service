using FluentValidation;
using SFA.DAS.AdminService.Web.Controllers;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class FrameworkSearchResultsViewModelValidator : AbstractValidator<FrameworkSearchResultsViewModel>
    {
        public FrameworkSearchResultsViewModelValidator()
        {
            RuleFor(vm => vm.SelectedResult)
                .Must(x => x > 0)
                .WithMessage("Select a course");
        }
    }
}
