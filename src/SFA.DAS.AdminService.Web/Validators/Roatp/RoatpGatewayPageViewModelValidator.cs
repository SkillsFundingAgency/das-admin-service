using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;


namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public  class RoatpGatewayPageViewModelValidator : IRoatpGatewayPageViewModelValidator
    {
        private const string FailDetailsRequired = "Enter comments";
        private const string TooManyWords = "Your comments must be 150 words or less";

        public async Task<ValidationResponse> Validate(RoatpGatewayPageViewModel vm)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(vm.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass", vm.NoSelectionErrorMessage));
            }
            else
            {
                if (vm.Status ==SectionReviewStatus.Fail && string.IsNullOrEmpty(vm.OptionFailText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                        FailDetailsRequired));
                }
            }

            if (validationResponse.Errors.Any()) return await Task.FromResult(validationResponse);

       
            switch (vm.Status)
            {
                case SectionReviewStatus.Pass when !string.IsNullOrEmpty(vm.OptionPassText):
                {
                    var wordCount = vm.OptionPassText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionPassText",
                            TooManyWords));
                    }

                    break;
                }
                case SectionReviewStatus.Fail when !string.IsNullOrEmpty(vm.OptionFailText):
                {
                    var wordCount = vm.OptionFailText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                            TooManyWords));
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
                            TooManyWords));
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
