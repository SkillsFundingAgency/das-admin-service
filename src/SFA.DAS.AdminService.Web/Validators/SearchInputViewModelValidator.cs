﻿using FluentValidation;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using System;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class SearchInputViewModelValidator : AbstractValidator<SearchInputViewModel>
    {
        private readonly DateTime minimiumDate = new DateTime(1900, 1, 1);
        
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
                        context.AddFailure("Date", "Enter a date of birth");
                        return;
                    }

                    if (int.TryParse(vm.Day, out var day) && int.TryParse(vm.Month, out var month) && int.TryParse(vm.Year, out var year))
                    {
                        var date = DateExtensions.ConstructDate(vm.Day, vm.Month, vm.Year);

                        if (date == null)
                        {
                            context.AddFailure("Date", "The date must be a real date");
                        }
                        else if (date < minimiumDate)
                        { 
                            //Waiting on confirmation of the minimum date and error text
                            context.AddFailure("Date", "The date of birth must be after 1 January 1900");
                        }
                        else if (date > DateTime.Now.Date)
                        {
                            context.AddFailure("Date", "The date of birth must be in the past");
                        }
                    }
                    else
                    {
                        context.AddFailure("Date", "The date must be a real date");
                    }
                });
            });
        }
    }
}
