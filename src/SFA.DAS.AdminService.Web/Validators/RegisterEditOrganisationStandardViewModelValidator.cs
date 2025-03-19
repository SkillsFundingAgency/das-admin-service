using FluentValidation;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterEditOrganisationStandardViewModelValidator : AbstractValidator<RegisterViewAndEditOrganisationStandardViewModel>
    {
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IRegisterValidator _registerValidator;

        public RegisterEditOrganisationStandardViewModelValidator(IOrganisationsApiClient organisationsApiClient,
            IRegisterValidator registerValidator)
        {
            _organisationsApiClient = organisationsApiClient;
            _registerValidator = registerValidator;
            var errorInEffectiveFrom = false;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResultEffectiveFrom = registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveFromDay,
                    vm.EffectiveFromMonth,
                    vm.EffectiveFromYear, "EffectiveFromDay",
                    "EffectiveFromMonth", "EffectiveFromYear", "EffectiveFrom", "Effective From");

                errorInEffectiveFrom = validationResultEffectiveFrom.Errors.Count > 0;

                var validationResultEffectiveTo = registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveToDay,
                    vm.EffectiveToMonth,
                    vm.EffectiveToYear, "EffectiveToDay",
                    "EffectiveToMonth", "EffectiveToYear", "EffectiveTo", "Effective To");

                vm.EffectiveFrom = DateExtensions.ConstructDate(vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear);
                vm.EffectiveTo = DateExtensions.ConstructDate(vm.EffectiveToDay, vm.EffectiveToMonth, vm.EffectiveToYear);

                CreateFailuresInContext(validationResultEffectiveFrom.Errors, context);
                CreateFailuresInContext(validationResultEffectiveTo.Errors, context);
                
                var deliveryAreas = vm.DeliveryAreas ?? new List<int>();
                var validationResultExternals = _organisationsApiClient
                    .ValidateUpdateOrganisationStandard(vm.OrganisationId, vm.OrganisationStandardId, vm.StandardId, vm.EffectiveFrom,
                        vm.EffectiveTo, vm.ContactId, deliveryAreas, vm.ActionChoice, vm.Status, vm.OrganisationStatus).Result;
                if (validationResultExternals.IsValid) return;
                foreach (var error in validationResultExternals.Errors)
                {
                    if (errorInEffectiveFrom == false || error.Field != "EffectiveFrom")
                        context.AddFailure(error.Field, error.ErrorMessage);
                }

            });
        }

        private static void CreateFailuresInContext(IEnumerable<ValidationErrorDetail> errs, ValidationContext<RegisterViewAndEditOrganisationStandardViewModel> context) //CustomContext context)
        {
            foreach (var error in errs)
            {
                context.AddFailure(error.Field, error.ErrorMessage);
            }
        }
    }
}
