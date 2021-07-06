using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterEditOrganisationStandardVersionViewModelValidator : AbstractValidator<RegisterEditOrganisationStandardVersionViewModel>
    {
        private readonly IRegisterValidator _registerValidator;

        public RegisterEditOrganisationStandardVersionViewModelValidator(IRegisterValidator registerValidator)
        {
            _registerValidator = registerValidator;
            var errorInEffectiveFrom = false;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResultEffectiveFrom = registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveFromDay,
                    vm.EffectiveFromMonth,
                    vm.EffectiveFromYear, "EffectiveFromDay",
                    "EffectiveFromMonth", "EffectiveFromYear", "EffectiveFrom", "Effective From");

                errorInEffectiveFrom = validationResultEffectiveFrom.Errors.Count > 0;

                var validationResultEffectiveTo = _registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveToDay,
                    vm.EffectiveToMonth,
                    vm.EffectiveToYear, "EffectiveToDay",
                    "EffectiveToMonth", "EffectiveToYear", "EffectiveTo", "Effective To");

                vm.EffectiveFrom = ValidatorExtensions.ConstructDate(vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear);
                vm.EffectiveTo = ValidatorExtensions.ConstructDate(vm.EffectiveToDay, vm.EffectiveToMonth, vm.EffectiveToYear);

                CreateFailuresInContext(validationResultEffectiveFrom.Errors, context);
                CreateFailuresInContext(validationResultEffectiveTo.Errors, context);
            });
        }

        private static void CreateFailuresInContext(IEnumerable<ValidationErrorDetail> errs, CustomContext context)
        {
            foreach (var error in errs)
            {
                context.AddFailure(error.Field, error.ErrorMessage);
            }
        }
    }
}
