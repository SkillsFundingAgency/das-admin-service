using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Infrastructure;
using System.Linq;
using SFA.DAS.AdminService.Common.Helpers;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterAddOrganisationStandardViewModelValidator : AbstractValidator<RegisterAddOrganisationStandardViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IRegisterValidator _registerValidator;
        private readonly IControllerSession _controllerSession;

        public RegisterAddOrganisationStandardViewModelValidator(IOrganisationsApiClient apiClient,
            IRegisterValidator registerValidator, 
            IControllerSession controllerSession)
        {
            _apiClient = apiClient;
            _registerValidator = registerValidator;
            _controllerSession = controllerSession;

            var errorInEffectiveFrom = false;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResultEffectiveFrom = _registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveFromDay, 
                    vm.EffectiveFromMonth, 
                    vm.EffectiveFromYear, "EffectiveFromDay", 
                    "EffectiveFromMonth", "EffectiveFromYear", "EffectiveFrom", "Effective From");
        
                errorInEffectiveFrom = validationResultEffectiveFrom.Errors.Count > 0;

                var validationResultEffectiveTo = _registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveToDay,
                    vm.EffectiveToMonth,
                    vm.EffectiveToYear, "EffectiveToDay",
                    "EffectiveToMonth", "EffectiveToYear", "EffectiveTo", "Effective To");

                vm.EffectiveFrom = DateHelper.ConstructDate(vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear);
                vm.EffectiveTo = DateHelper.ConstructDate(vm.EffectiveToDay, vm.EffectiveToMonth, vm.EffectiveToYear);

                CreateFailuresInContext(validationResultEffectiveFrom.Errors, context);
                CreateFailuresInContext(validationResultEffectiveTo.Errors, context);

                var versionData = _controllerSession.AddOrganisationStandardViewModel.Versions;
                
                if (!versionData.Any(v => v.EffectiveFrom.HasValue))
                {
                    context.AddFailure("Versions", "Add at least one standard version");
                }

                var deliveryAreas = vm.DeliveryAreas ?? new List<int>();

                var validationResultExternals = _apiClient.ValidateCreateOrganisationStandard(vm.OrganisationId, vm.StandardId, vm.EffectiveFrom,
                        vm.EffectiveTo, vm.ContactId, deliveryAreas).Result;

                if (validationResultExternals.IsValid) 
                    return;

                foreach (var error in validationResultExternals.Errors)
                {
                    if (errorInEffectiveFrom==false || error.Field !="EffectiveFrom")
                        context.AddFailure(error.Field, error.ErrorMessage);
                }

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
