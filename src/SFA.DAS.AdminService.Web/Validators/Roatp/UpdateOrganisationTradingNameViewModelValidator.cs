﻿namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    using System.Linq;
    using FluentValidation;
    using SFA.DAS.AdminService.Web.ViewModels.Roatp;

    public class UpdateOrganisationTradingNameViewModelValidator : AbstractValidator<UpdateOrganisationTradingNameViewModel>
    {
        private IRoatpOrganisationValidator _validator;

        public UpdateOrganisationTradingNameViewModelValidator(IRoatpOrganisationValidator validator)
        {
            _validator = validator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationErrors = _validator.IsValidTradingName(vm.TradingName);
                if (!validationErrors.Any()) return;
                foreach (var error in validationErrors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
