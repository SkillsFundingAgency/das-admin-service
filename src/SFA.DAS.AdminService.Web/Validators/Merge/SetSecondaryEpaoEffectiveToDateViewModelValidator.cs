using FluentValidation;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using System;

namespace SFA.DAS.AdminService.Web.Validators.Merge
{
    public class SetSecondaryEpaoEffectiveToDateViewModelValidator : AbstractValidator<SetSecondaryEpaoEffectiveToDateViewModel>
    {
        public SetSecondaryEpaoEffectiveToDateViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm.Day == null && vm.Month == null && vm.Year == null)
                {
                    context.AddFailure("Date", "Enter an Effective to date");
                }
                else
                {
                    int.TryParse(vm.Day, out var day);
                    int.TryParse(vm.Month, out var month);
                    int.TryParse(vm.Year, out var year);

                    if (day == 0)
                    {
                        context.AddFailure("Day", "Effective to date must include a day");
                    }

                    if (month == 0)
                    {
                        context.AddFailure("Month", "Effective to date must include a month");
                    }

                    if (year == 0)
                    {
                        context.AddFailure("Year", "Effective to date must include a year");
                    }

                    if (day != 0 && month != 0 && year != 0)
                    {
                        var date = ValidatorExtensions.ConstructDate(vm.Day, vm.Month, vm.Year);

                        if (date == null)
                        {
                            context.AddFailure("Date", "Effective to date must be a real date");
                        }
                        else if (date < DateTime.Now.Date)
                        {
                            context.AddFailure("Date", "Effective to date must be in the future");
                        }
                    }
                }
            });
        }
    }
}
