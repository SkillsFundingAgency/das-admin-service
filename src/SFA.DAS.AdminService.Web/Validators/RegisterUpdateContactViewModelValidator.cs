using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterUpdateContactViewModelValidator: AbstractValidator<RegisterViewAndEditContactViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;
       
        public RegisterUpdateContactViewModelValidator(ApiClientFactory<OrganisationsApiClient> apiClient)
        {
            _apiClient = apiClient.GetApiClient(ApplicationType.EPAO);
        RuleFor(vm => vm).Custom((vm, context) =>
        {
            var validationResult =  _apiClient.ValidateUpdateContact(vm.ContactId, vm.FirstName , vm.LastName, vm.Email, vm.PhoneNumber).Result;
            if (validationResult.IsValid) return;
            foreach (var error in validationResult.Errors)
            {
                context.AddFailure(error.Field, error.ErrorMessage);
            }
        });
    }
}}