using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    public class RoatpFinancialApplicationViewModelValidator : AbstractValidator<RoatpFinancialApplicationViewModel>
    {
        public RoatpFinancialApplicationViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Exempt)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(vm.FinancialReviewDetails.SelectedGrade))
                {
                    context.AddFailure("FinancialReviewDetails.SelectedGrade", "Select a grade for this application");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate && string.IsNullOrWhiteSpace(vm.FinancialReviewDetails.Comments))
                {
                    context.AddFailure("FinancialReviewDetails.Comments", "Enter why the application was graded inadequate");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Clarification && string.IsNullOrWhiteSpace(vm.FinancialReviewDetails.Comments))
                {
                    context.AddFailure("FinancialReviewDetails.Comments", "Enter why the application requires clarification");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Outstanding
                         || vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Good
                         || vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Satisfactory)
                {
                    switch (vm.FinancialReviewDetails.SelectedGrade)
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
                    }
                }
            });
        }

        private void ProcessDate(FinancialDueDate dueDate, string propertyName, CustomContext context)
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

            if (parsedDate < DateTime.Today)
            {
                context.AddFailure(propertyName, "Financial due date must be a future date");
            }
        }
    }
}
