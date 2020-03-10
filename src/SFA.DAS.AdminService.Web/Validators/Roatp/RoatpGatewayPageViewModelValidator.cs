using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;


namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public class RoatpGatewayPageViewModelValidator : IRoatpGatewayPageViewModelValidator
    {
        public async Task<ValidationResponse> Validate(RoatpGatewayPageViewModel vm)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(vm.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass", "Select the outcome of this check"));
            }
            else
            {
                if (vm.Status ==SectionReviewStatus.Fail && string.IsNullOrEmpty(vm.OptionFailText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                        "Enter the reasons this check was failed"));
                }
            }

            if (validationResponse.Errors.Any()) return await Task.FromResult(validationResponse);

            const string tooManyWordsMessage = "Your comments must be 150 words or less";
            switch (vm.Status)
            {
                case SectionReviewStatus.Pass when !string.IsNullOrEmpty(vm.OptionPassText):
                {
                    var wordCount = vm.OptionPassText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionPassText",
                            tooManyWordsMessage));
                    }

                    break;
                }
                case SectionReviewStatus.Fail when !string.IsNullOrEmpty(vm.OptionFailText):
                {
                    var wordCount = vm.OptionFailText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                            tooManyWordsMessage));
                    }

                    break;
                }
                case SectionReviewStatus.InProgress when !string.IsNullOrEmpty(vm.OptionInProgressText):
                {
                    var wordCount = vm.OptionInProgressText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                        .Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionInProgressText",
                            tooManyWordsMessage));
                    }

                    break;
                }
            }

            return await Task.FromResult(validationResponse);
        }
    }

    public interface IRoatpGatewayPageViewModelValidator
    {
        Task<ValidationResponse> Validate(RoatpGatewayPageViewModel vm);
    }
}
