using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class
        RegisterAddOrganisationStandardViewModelValidator : AbstractValidator<RegisterAddOrganisationStandardViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IRegisterValidator _registerValidator;

        public RegisterAddOrganisationStandardViewModelValidator(IOrganisationsApiClient apiClient,
            IRegisterValidator registerValidator)
        {
            _apiClient = apiClient;
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

                vm.EffectiveFrom = ConstructDate(vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear);
                vm.EffectiveTo = ConstructDate(vm.EffectiveToDay, vm.EffectiveToMonth, vm.EffectiveToYear);

                CreateFailuresInContext(validationResultEffectiveFrom.Errors, context);
                CreateFailuresInContext(validationResultEffectiveTo.Errors, context);

                var deliveryAreas = vm.DeliveryAreas ?? new List<int>();
                var validationResultExternals = _apiClient
                    .ValidateCreateOrganisationStandard(vm.OrganisationId, vm.StandardId, vm.EffectiveFrom,
                        vm.EffectiveTo, vm.ContactId, deliveryAreas).Result;
                if (validationResultExternals.IsValid) return;
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

        private static DateTime? ConstructDate(string dayString, string monthString, string yearString)
        {

            if (!int.TryParse(dayString, out var day) || !int.TryParse(monthString, out var month) ||
                !int.TryParse(yearString, out var year)) return null;

            if (!IsValidDate(year, month, day))
                return null;

            return new DateTime(year, month, day);
        }

        public static bool IsValidDate(int year, int month, int day)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                return false;

            if (month < 1 || month > 12)
                return false;

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }
    }
}
