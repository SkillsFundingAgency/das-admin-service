﻿using FluentValidation;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Web.Resources;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public class AddOrganisationViaUkprnViewModelValidator : AbstractValidator<AddOrganisationViaUkprnViewModel>
    {
        private readonly IRoatpOrganisationValidator _validator;
        private readonly IRoatpApiClient _apiClient;

        public AddOrganisationViaUkprnViewModelValidator(IRoatpOrganisationValidator validator, IRoatpApiClient apiClient)
        {
            _validator = validator;
            _apiClient = apiClient;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = IsValidUkprn(vm);
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }

        private ValidationResponse IsValidUkprn(AddOrganisationViaUkprnViewModel vm)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            var fieldValidationErrors = _validator.IsValidUKPRN(vm.UKPRN);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(vm.UKPRN))
                {
                    fieldValidationErrors = IsDuplicateUkprn(vm.OrganisationId, vm.UKPRN);
                    if (fieldValidationErrors.Any())
                    {
                        validationResponse.Errors.AddRange(fieldValidationErrors);
                    }
                }
            }

            return validationResponse;
        }


        private List<ValidationErrorDetail> IsDuplicateUkprn(Guid organisationId, string ukprn)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            long ukprnValue = 0;
            var isParsed = long.TryParse(ukprn, out ukprnValue);

            var duplicateCheckResponse = _apiClient.DuplicateUKPRNCheck(organisationId, ukprnValue).Result;

            if (duplicateCheckResponse != null && duplicateCheckResponse.DuplicateFound)
            {
                var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.UKPRNDuplicateMatch,
                    duplicateCheckResponse.DuplicateOrganisationName);
                errorMessages.Add(new ValidationErrorDetail("UKPRN", duplicateErrorMessage));
            }

            return errorMessages;
        }
    }
}
