namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    using FluentValidation;
    using SFA.DAS.AdminService.Web.ViewModels.Roatp;

    public class OrganisationSearchViewModelValidator : AbstractValidator<OrganisationSearchViewModel>
    {
        public OrganisationSearchViewModelValidator(ISearchTermValidator validator)
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = validator.ValidateSearchTerm(vm.SearchTerm).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
