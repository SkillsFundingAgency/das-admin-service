using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AdminService.Application.ViewModels;


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

            if (string.IsNullOrWhiteSpace(vm.Value))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass", "Select the outcome of this check"));
            }
            else
            {
                if (vm.Value =="Fail" && string.IsNullOrEmpty(vm.OptionFailText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                        "Enter the reasons this check was failed"));
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
