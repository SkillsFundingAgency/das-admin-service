using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using System;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using System.Globalization;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    public class RoatpFinancialApplicationViewModelValidator : AbstractValidator<RoatpFinancialApplicationViewModel>
    {
        public RoatpFinancialApplicationViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm?.FinancialReviewDetails is null || string.IsNullOrWhiteSpace(vm.FinancialReviewDetails.SelectedGrade))
                {
                    context.AddFailure("FinancialReviewDetails.SelectedGrade", "Select the outcome of this financial health assessment");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate && string.IsNullOrWhiteSpace(vm.InadequateComments))
                {
                    context.AddFailure("InadequateComments", "Enter internal comments");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate && HasExceededWordCount(vm.InadequateComments))
                {
                    context.AddFailure("InadequateComments", "Your internal comments must be 500 words or less");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate && string.IsNullOrWhiteSpace(vm.InadequateExternalComments))
                {
                    context.AddFailure("InadequateExternalComments", "Enter external comments");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate && HasExceededWordCount(vm.InadequateExternalComments))
                {
                    context.AddFailure("InadequateExternalComments", "Your external comments must be 500 words or less");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Clarification && string.IsNullOrWhiteSpace(vm.ClarificationComments))
                {
                    context.AddFailure("ClarificationComments", "Enter internal comments");
                }
                else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Clarification && HasExceededWordCount(vm.ClarificationComments))
                {
                    context.AddFailure("ClarificationComments", "Your comments must be 500 words or less");
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

            var isValidDate = DateTime.TryParseExact($"{day}/{month}/{year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);

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

        private static bool HasExceededWordCount(string input, int maxWordcount = 500)
        {
            bool hasExceeded = false;

            var text = input?.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                var wordCount = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;

                hasExceeded = (wordCount > maxWordcount);
            }

            return hasExceeded;
        }
    }
}
