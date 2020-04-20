using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.ViewModels.Register;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterAddContactViewModelValidator : AbstractValidator<RegisterAddContactViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

       public RegisterAddContactViewModelValidator(IOrganisationsApiClient apiClient)
        {
            _apiClient = apiClient;
          
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult =  _apiClient.ValidateCreateContact(vm.FirstName, vm.LastName, vm.EndPointAssessorOrganisationId, vm.Email, vm.PhoneNumber).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}