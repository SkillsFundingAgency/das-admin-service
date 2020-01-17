using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterAddOrganisationViewModelValidator : AbstractValidator<RegisterOrganisationViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

        public RegisterAddOrganisationViewModelValidator(ApiClientFactory<OrganisationsApiClient> apiClient)
        {
            _apiClient = apiClient.GetApiClient(ApplicationType.EPAO);
          
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult =  _apiClient.ValidateCreateOrganisation(vm.Name, vm.Ukprn, vm.OrganisationTypeId, vm.CompanyNumber, vm.CharityNumber).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
