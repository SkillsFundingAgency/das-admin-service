using FluentValidation;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using System;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class SearchInputViewModelValidator : AbstractValidator<SearchInputViewModel>
    {
        private readonly DateTime minimumDate = new DateTime(1753, 1, 1);

        public SearchInputViewModelValidator()
        {
            When(vm => vm.SearchType == SearchTypes.Standards, () =>
            {
                RuleFor(vm => vm.SearchString)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Enter name or number")
                    .Must(x => x == null || x.Trim().Length > 1)
                    .WithMessage("Name or number must be at least 2 characters.");
            });

            When(vm => vm.SearchType == SearchTypes.Frameworks, () =>
            {
                RuleFor(vm => vm.FirstName)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Enter first name")
                    .Must(x => x == null || x.Trim().Length > 1)
                    .WithMessage("First name must be at least 2 characters.");

                RuleFor(vm => vm.LastName)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Enter last name")
                    .Must(x => x == null || x.Trim().Length > 1)
                    .WithMessage("Last name must be at least 2 characters.");

                RuleFor(vm => vm).Custom((vm, context) =>
                {
                    if (string.IsNullOrEmpty(vm.Day) || string.IsNullOrEmpty(vm.Month) || string.IsNullOrEmpty(vm.Year))
                    {
                        context.AddFailure(nameof(vm.Date), "Enter a date of birth");
                        return;
                    }

                    var date = DateExtensions.ConstructDate(vm.Day, vm.Month, vm.Year);
                    vm.Date = date; 

                    if (date == null)
                    {
                        context.AddFailure(nameof(vm.Date), "The date must be a real date");
                    }
                });

                RuleFor(vm => vm.Date)
                    .Must(date => date.HasValue && date.Value.Date <= DateTime.Now.Date).WithMessage("The date of birth must be in the past")
                    .When(vm => vm.Date.HasValue);

                RuleFor(vm => vm.Date)
                    .Must(date => date.HasValue && date.Value.Date >= minimumDate).WithMessage("Check the year of your date of birth")
                    .When(vm => vm.Date.HasValue);
            });
        }
    }
}