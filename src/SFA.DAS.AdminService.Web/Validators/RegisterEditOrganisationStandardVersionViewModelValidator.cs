using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterEditOrganisationStandardVersionViewModelValidator : AbstractValidator<RegisterEditOrganisationStandardVersionViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IRegisterValidator _registerValidator;

        public RegisterEditOrganisationStandardVersionViewModelValidator(IOrganisationsApiClient apiClient, IRegisterValidator registerValidator)
        {
            _registerValidator = registerValidator;
            _apiClient = apiClient;

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

                var validationResultExternals = _apiClient.ValidateUpdateOrganisationStandardVersion(vm.OrganisationStandardId, vm.Version, vm.EffectiveFrom, vm.EffectiveTo).Result;

                if (validationResultExternals.IsValid) return;
                foreach (var error in validationResultExternals.Errors)
                {
                    if (errorInEffectiveFrom == false || error.Field != "EffectiveFrom")
                        context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }

        private static void CreateFailuresInContext(IEnumerable<ValidationErrorDetail> errs, ValidationContext<RegisterEditOrganisationStandardVersionViewModel> context) //CustomContext context)
        {
            foreach (var error in errs)
            {
                context.AddFailure(error.Field, error.ErrorMessage);
            }
        }
    }
}
