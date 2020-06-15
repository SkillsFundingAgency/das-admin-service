using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public class RoatpGatewayPageValidator : IRoatpGatewayPageValidator
    {
        private const string FailDetailsRequired = "Enter comments";
        private const string TooManyWords = "Your comments must be 150 words or less";

        public async Task<ValidationResponse> Validate(SubmitGatewayPageAnswerCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(command.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass", NoSelectionErrorMessages.Errors[command.PageId]));
            }
            else
            {
                if (command.Status == SectionReviewStatus.Fail && string.IsNullOrEmpty(command.OptionFailText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                        FailDetailsRequired));
                }
            }

            if (validationResponse.Errors.Any())
            {
                return await Task.FromResult(validationResponse);
            }
       
            switch (command.Status)
            {
                case SectionReviewStatus.Pass when !string.IsNullOrEmpty(command.OptionPassText):
                {
                    var wordCount = command.OptionPassText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionPassText",
                            TooManyWords));
                    }

                    break;
                }
                case SectionReviewStatus.Fail when !string.IsNullOrEmpty(command.OptionFailText):
                {
                    var wordCount = command.OptionFailText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                            TooManyWords));
                    }

                    break;
                }
                case SectionReviewStatus.InProgress when !string.IsNullOrEmpty(command.OptionInProgressText):
                {
                    var wordCount = command.OptionInProgressText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                        .Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionInProgressText",
                            TooManyWords));
                    }

                    break;
                }
            }

            return await Task.FromResult(validationResponse);
        }      
    }

    public interface IRoatpGatewayPageValidator
    {
        Task<ValidationResponse> Validate(SubmitGatewayPageAnswerCommand command);
    }
}
