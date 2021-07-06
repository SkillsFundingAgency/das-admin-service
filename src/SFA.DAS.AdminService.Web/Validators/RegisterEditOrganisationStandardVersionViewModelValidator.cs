using FluentValidation;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterEditOrganisationStandardVersionViewModelValidator : AbstractValidator<RegisterEditOrganisationStandardVersionViewModel>
    {
        public RegisterEditOrganisationStandardVersionViewModelValidator()
        {

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                vm.EffectiveFrom = ValidatorExtensions.ConstructDate(vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear);
                vm.EffectiveTo = ValidatorExtensions.ConstructDate(vm.EffectiveToDay, vm.EffectiveToMonth, vm.EffectiveToYear);

                if (vm.EffectiveTo is null)
                {
                    context.AddFailure("EffectiveTo", "Enter a valid date");
                }
                else if (vm.EffectiveTo.Value < DateTime.UtcNow)
                {
                    context.AddFailure("EffectiveTo", "The date must be in the future");
                }
            });
        }
    }
}
