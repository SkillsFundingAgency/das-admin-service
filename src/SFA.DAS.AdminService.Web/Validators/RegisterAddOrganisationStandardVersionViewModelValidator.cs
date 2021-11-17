using FluentValidation;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterAddOrganisationStandardVersionViewModelValidator : AbstractValidator<RegisterAddStandardVersionViewModel>
    {
        public RegisterAddOrganisationStandardVersionViewModelValidator(IRegisterValidator registerValidator) 
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var date = ConstructDate(vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear);

                if (date == null)
                {
                    context.AddFailure("EffectiveFrom", "The Effective from date is required");
                }
            });
        }

        private static DateTime? ConstructDate(string dayString, string monthString, string yearString)
        {

            if (!int.TryParse(dayString, out var day) || !int.TryParse(monthString, out var month) ||
                !int.TryParse(yearString, out var year)) return null;

            if (!IsValidDate(year, month, day))
                return null;

            return new DateTime(year, month, day);
        }

        private static bool IsValidDate(int year, int month, int day)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                return false;

            if (month < 1 || month > 12)
                return false;

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }
    }
}
