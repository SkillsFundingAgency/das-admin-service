﻿using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.ViewModels.Register;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class RegisterAddOrganisationViewModelValidator : AbstractValidator<RegisterOrganisationViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

        public RegisterAddOrganisationViewModelValidator(IOrganisationsApiClient apiClient)
        {
            _apiClient = apiClient;
          
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult =  _apiClient.ValidateCreateOrganisation(vm.Name, vm.Ukprn, vm.OrganisationTypeId, vm.CompanyNumber, vm.CharityNumber, vm.RecognitionNumber).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
