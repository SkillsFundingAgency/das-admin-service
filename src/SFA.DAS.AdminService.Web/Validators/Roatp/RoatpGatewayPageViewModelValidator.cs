using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AdminService.Application.ViewModels;


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
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass", "Please enter a value"));
            }
            else
            {
                if (vm.Value =="Fail" && string.IsNullOrEmpty(vm.OptionFailText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                        "Please  enter fail text"));
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
