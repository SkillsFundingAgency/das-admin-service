using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AdminService.Web.Resources;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    //public class RoatpGatewayPageViewModelValidator: AbstractValidator<RoatpGatewayPageViewModel>
    //{
    //    public RoatpGatewayPageViewModelValidator()
    //    {
    //            RuleFor(vm => vm).Custom((vm, context) =>
    //        {
    //            if (string.IsNullOrEmpty(vm.Value))
    //            {
    //                context.AddFailure("OptionPass.Value", "Please enter a choice");
    //            }
    //        });
    //    }

    //}
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
