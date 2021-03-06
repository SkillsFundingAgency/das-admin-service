﻿namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    using System.Linq;
    using FluentValidation;
    using SFA.DAS.AdminService.Web.ViewModels.Roatp;

    public class UpdateOrganisationLegalNameViewModelValidator : AbstractValidator<UpdateOrganisationLegalNameViewModel>
    {
        private IRoatpOrganisationValidator _validator;

        public UpdateOrganisationLegalNameViewModelValidator(IRoatpOrganisationValidator validator)
        {
            _validator = validator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationErrors = _validator.IsValidLegalName(vm.LegalName);
                if (!validationErrors.Any()) return;
                foreach (var error in validationErrors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
