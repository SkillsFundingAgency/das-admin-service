using System;
using System.Collections.Generic;
using System.Globalization;
using FluentValidation;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;


namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public class
        AddApplicationDeterminedDateViewModelValidator : AbstractValidator<AddApplicationDeterminedDateViewModel>
    {
        private readonly IApplicationDeterminedDateValidationService _applicationDeterminedDateValidationService;

        public AddApplicationDeterminedDateViewModelValidator(
            IApplicationDeterminedDateValidationService applicationDeterminedDateValidationService)
        {
            _applicationDeterminedDateValidationService = applicationDeterminedDateValidationService;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = IsaValidDeterminedDate(vm);
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }

        private ValidationResponse IsaValidDeterminedDate(AddApplicationDeterminedDateViewModel viewModel)
        {
            return _applicationDeterminedDateValidationService.ValidateApplicationDeterminedDate(viewModel.Day,
                viewModel.Month, viewModel.Year);
        }
    }
}