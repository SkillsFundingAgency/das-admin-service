using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class FrameworkSearchResultsViewModelValidator : AbstractValidator<FrameworkLearnerSearchResultsViewModel>
    {
        public FrameworkSearchResultsViewModelValidator()
        {
            RuleFor(x => x.SelectedResult)
            .NotEmpty().WithMessage("Select a course");
        }
    }
}
