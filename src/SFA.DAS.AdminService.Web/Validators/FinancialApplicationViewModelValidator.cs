using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class FinancialApplicationViewModelValidator  : AbstractValidator<FinancialApplicationViewModel>
    {
        public FinancialApplicationViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Exempt)
                {
                    return;
                }
                
                if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate && string.IsNullOrWhiteSpace(vm.Grade.InadequateMoreInformation))
                {
                    context.AddFailure("Grade.InadequateMoreInformation", "Enter your comments");
                }
                else if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Outstanding 
                         || vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Good 
                         || vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Satisfactory
                         || vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Monitoring)
                {
                    switch (vm.Grade.SelectedGrade)
                    {
                        case FinancialApplicationSelectedGrade.Outstanding:
                            ProcessDate(vm.OutstandingFinancialDueDate, "OutstandingFinancialDueDate", context);
                            break;
                        case FinancialApplicationSelectedGrade.Good:
                            ProcessDate(vm.GoodFinancialDueDate, "GoodFinancialDueDate", context);
                            break;
                        case FinancialApplicationSelectedGrade.Satisfactory:
                            ProcessDate(vm.SatisfactoryFinancialDueDate, "SatisfactoryFinancialDueDate", context);
                            break;
                        case FinancialApplicationSelectedGrade.Monitoring:
                            ProcessDate(vm.MonitoringFinancialDueDate, "MonitoringFinancialDueDate", context);
                            break;
                    }
                }
                else if (string.IsNullOrWhiteSpace(vm.Grade.SelectedGrade))
                {
                    context.AddFailure("Grade.SelectedGrade", "Select a grade for this application");
                }
            });
        }

        private void ProcessDate(FinancialDueDate dueDate, string propertyName, ValidationContext<FinancialApplicationViewModel> context) //CustomContext context)
        {
            if (string.IsNullOrWhiteSpace(dueDate.Day) || string.IsNullOrWhiteSpace(dueDate.Month) || string.IsNullOrWhiteSpace(dueDate.Year))
            {
                context.AddFailure(propertyName, "Enter the financial due date");
                return;
            }

            if (!int.TryParse(dueDate.Day, out int _) || !int.TryParse(dueDate.Month, out int _) || !int.TryParse(dueDate.Year, out int _))
            {
                context.AddFailure(propertyName, "Enter a correct financial due date");
                return;
            }
                    
            var day = dueDate.Day;
            var month = dueDate.Month;
            var year = dueDate.Year;

            var isValidDate = DateTime.TryParse($"{day}/{month}/{year}", out var parsedDate);

            if (!isValidDate)
            {
                context.AddFailure(propertyName, "Enter a correct financial due date");
                return;
            }

            if (parsedDate <= DateTime.Today)
            {
                context.AddFailure(propertyName, "Financial due date must be a future date");
            }
        }
    }
}