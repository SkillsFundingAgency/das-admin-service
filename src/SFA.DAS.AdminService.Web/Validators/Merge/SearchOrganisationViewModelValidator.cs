using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Validators.Merge
{
    public class SearchOrganisationViewModelValidator : AbstractValidator<SearchOrganisationViewModel>
    {
        public SearchOrganisationViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (IsLessThanTwoCharacters(vm.SearchString))
                {
                    context.AddFailure("SearchString", "Enter 2 or more characters");
                }
            });
        }

        private static bool IsLessThanTwoCharacters(string input)
        {
            if (input == null)
            {
                return true;
            }

            return input.Length < 2;
        }
    }
}
