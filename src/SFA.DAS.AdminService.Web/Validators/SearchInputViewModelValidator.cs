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

                RuleFor(vm => vm.Date)
                    .Must(BeValidDate)
                    .WithMessage("Enter date of birth")
                    .Must(BeRealDate)
                    .WithMessage("The date must be a real date")
                    .Must(BeInPast)
                    .WithMessage("The date of birth must be in the past")
                    .Must(BeWithinRange)
                    .WithMessage("Check the year of your date of birth");
            });
        }

        private bool BeValidDate(SearchInputViewModel vm, DateTime? date, ValidationContext<SearchInputViewModel> context)
        {
            if (string.IsNullOrEmpty(vm.Day) || string.IsNullOrEmpty(vm.Month) || string.IsNullOrEmpty(vm.Year))
            {
                return false;
            }

            vm.Date = DateExtensions.ConstructDate(vm.Day, vm.Month, vm.Year);
            return true;
        }

        private bool BeRealDate(SearchInputViewModel vm, DateTime? date, ValidationContext<SearchInputViewModel> context)
        {
            return vm.Date != null;
        }

        private bool BeInPast(SearchInputViewModel vm, DateTime? date, ValidationContext<SearchInputViewModel> context)
        {
            return vm.Date.HasValue && vm.Date.Value.Date <= DateTime.Now.Date;
        }

        private bool BeWithinRange(SearchInputViewModel vm, DateTime? date, ValidationContext<SearchInputViewModel> context)
        {
            return vm.Date.HasValue && vm.Date.Value.Date >= minimumDate;
        }
    }
}