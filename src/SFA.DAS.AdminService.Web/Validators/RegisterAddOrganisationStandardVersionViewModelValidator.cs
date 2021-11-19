using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterAddOrganisationStandardVersionViewModelValidator : AbstractValidator<RegisterAddStandardVersionViewModel>
    {
        public RegisterAddOrganisationStandardVersionViewModelValidator(IRegisterValidator registerValidator) 
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResultEffectiveFrom = registerValidator.CheckDateIsNotEmptyAndIsValid(
                    vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear,
                    "EffectiveFromDay", "EffectiveFromMonth", "EffectiveFromYear",
                    "EffectiveFromDate", "effective from");
      
                var validationResultEffectiveTo = registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveToDay,
                    vm.EffectiveToMonth,
                    vm.EffectiveToYear, "EffectiveToDay",
                    "EffectiveToMonth", "EffectiveToYear", "EffectiveTo", "Effective To");

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
