using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class FrameworkLearnerSearchResultsViewModelValidator : AbstractValidator<FrameworkLearnerSearchResultsViewModel>
    {
        public FrameworkLearnerSearchResultsViewModelValidator()
        {
            RuleFor(x => x.SelectedResult)
            .NotEmpty().WithMessage("Select a course");
        }
    }
}
