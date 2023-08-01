using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.ViewModels.Register;

namespace SFA.DAS.AdminService.Web.Validators
{   
    public class RegisterUpdateOrganisationViewModelValidator : AbstractValidator<RegisterViewAndEditOrganisationViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

        public RegisterUpdateOrganisationViewModelValidator(IOrganisationsApiClient apiClient)
        {
            _apiClient = apiClient;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = _apiClient.ValidateUpdateOrganisation(vm.OrganisationId, vm.Name, vm.Ukprn, 
                                                vm.OrganisationTypeId, vm.Address1, vm.Address2, vm.Address3,vm.Address4,vm.Postcode,vm.Status, vm.ActionChoice,
                                                vm.CompanyNumber, vm.CharityNumber, vm.RecognitionNumber).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
