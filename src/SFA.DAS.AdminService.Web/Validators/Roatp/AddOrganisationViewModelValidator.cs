namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    using FluentValidation;
    using SFA.DAS.AdminService.Web.ViewModels.Roatp;

    public class AddOrganisationViewModelValidator : AbstractValidator<AddOrganisationViewModel>
    {
        private IAddOrganisationValidator _validator;

        public AddOrganisationViewModelValidator(IAddOrganisationValidator validator)
        {
            _validator = validator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = _validator.ValidateOrganisationDetails(vm).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
