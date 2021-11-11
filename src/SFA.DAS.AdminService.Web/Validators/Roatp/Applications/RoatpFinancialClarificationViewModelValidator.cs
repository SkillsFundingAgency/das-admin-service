using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Financial;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public class RoatpFinancialClarificationViewModelValidator : IRoatpFinancialClarificationViewModelValidator
    {
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;
        private const string MaxFileSizeExceeded = "The selected file must be smaller than 5MB";
        private const string FileMustBePdf = "The selected file must be a PDF";
        private const string ClarificationFile = "ClarificationFile";
        private const string ClarificationResponse = "ClarificationResponse";

        public ValidationResponse Validate(RoatpFinancialClarificationViewModel vm,
            bool isClarificationFilesUpload, bool isClarificationOutcome)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (isClarificationFilesUpload && vm.FilesToUpload != null)
            {
                foreach (var file in vm.FilesToUpload)
                {

                    if (!FileContentIsValidForPdfFile(file))
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail(ClarificationFile, FileMustBePdf));

                        break;
                    }
                    else if (file.Length > MaxFileSizeInBytes)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail(ClarificationFile,
                            MaxFileSizeExceeded));
                        break;
                    }
                } 
                if (vm.FilesToUpload.Count==0)
                    validationResponse.Errors.Add(new ValidationErrorDetail(ClarificationFile, "Select a file"));
            }

            if (isClarificationOutcome)
            {
                if (string.IsNullOrWhiteSpace(vm.ClarificationResponse))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail(ClarificationResponse,
                        "Enter clarification response"));
                }
                else if (HasExceededWordCount(vm.ClarificationResponse))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail(ClarificationResponse,
                        "Your comments must be 500 words or less"));
                }

                if (vm?.FinancialReviewDetails is null ||
                    string.IsNullOrWhiteSpace(vm.FinancialReviewDetails.SelectedGrade))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("FinancialReviewDetails.SelectedGrade",
                        "Select the outcome of this financial health assessment"));
                }
                else
                    switch (vm.FinancialReviewDetails.SelectedGrade)
                    {
                        case FinancialApplicationSelectedGrade.Exempt:
                            break;
                        case FinancialApplicationSelectedGrade.Inadequate:
                            if (string.IsNullOrWhiteSpace(vm.InadequateComments))
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("InadequateComments", "Enter internal comments"));
                            }
                            else if (HasExceededWordCount(vm.InadequateComments))
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("InadequateComments", "Your internal comments must be 500 words or less"));
                            }

                            if(string.IsNullOrWhiteSpace(vm.InadequateExternalComments))
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("InadequateExternalComments", "Enter external comments"));
                            }
                            else if(HasExceededWordCount(vm.InadequateExternalComments))
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("InadequateExternalComments", "Your external comments must be 500 words or less"));
                            }
                            break;
                        case FinancialApplicationSelectedGrade.Outstanding:
                        case FinancialApplicationSelectedGrade.Good:
                        case FinancialApplicationSelectedGrade.Satisfactory:
                            switch (vm.FinancialReviewDetails.SelectedGrade)
                            {
                                case FinancialApplicationSelectedGrade.Outstanding:
                                    ProcessDate(vm.OutstandingFinancialDueDate, "OutstandingFinancialDueDate", validationResponse);
                                    break;
                                case FinancialApplicationSelectedGrade.Good:
                                    ProcessDate(vm.GoodFinancialDueDate, "GoodFinancialDueDate", validationResponse);
                                    break;
                                case FinancialApplicationSelectedGrade.Satisfactory:
                                    ProcessDate(vm.SatisfactoryFinancialDueDate, "SatisfactoryFinancialDueDate", validationResponse);
                                    break;
                            }
                            break;
                    }
            }

            return validationResponse;
        }




        private static void ProcessDate(FinancialDueDate dueDate, string propertyName,
            ValidationResponse validationResponse)
        {
            if (string.IsNullOrWhiteSpace(dueDate.Day) || string.IsNullOrWhiteSpace(dueDate.Month) ||
                string.IsNullOrWhiteSpace(dueDate.Year))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(propertyName, "Enter the financial due date"));
                return;
            }

            if (!int.TryParse(dueDate.Day, out int _) || !int.TryParse(dueDate.Month, out int _) ||
                !int.TryParse(dueDate.Year, out int _))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(propertyName,
                    "Enter a correct financial due date"));
                return;
            }

            var day = dueDate.Day;
            var month = dueDate.Month;
            var year = dueDate.Year;

            var isValidDate = DateTime.TryParseExact($"{day}/{month}/{year}", "d/M/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var parsedDate);

            if (!isValidDate)
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(propertyName,
                    "Enter a correct financial due date"));
                return;
            }

            if (parsedDate <= DateTime.Today)
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(propertyName,
                    "Financial due date must be a future date"));
            }
        }

        private static bool HasExceededWordCount(string input, int maxWordcount = 500)
        {
            bool hasExceeded = false;

            var text = input?.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                var wordCount = text.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length;

                hasExceeded = (wordCount > maxWordcount);
            }

            return hasExceeded;
        }

        private static bool FileContentIsValidForPdfFile(IFormFile file)
        {
            var pdfHeader = new byte[] {0x25, 0x50, 0x44, 0x46};

            using (var fileContents = file.OpenReadStream())
            {
                var headerOfActualFile = new byte[pdfHeader.Length];
                fileContents.Read(headerOfActualFile, 0, headerOfActualFile.Length);
                fileContents.Position = 0;

                return headerOfActualFile.SequenceEqual(pdfHeader);
            }
        }
    }
}

