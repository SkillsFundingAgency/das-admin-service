using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class FrameworkMultipleResultsViewModelValidator : AbstractValidator<FrameworkMultipleResultsViewModel>
    {
        public FrameworkMultipleResultsViewModelValidator()
        {
            RuleFor(x => x.SelectedResult)
            .NotEmpty().WithMessage("Select a course");
        }
    }
}
