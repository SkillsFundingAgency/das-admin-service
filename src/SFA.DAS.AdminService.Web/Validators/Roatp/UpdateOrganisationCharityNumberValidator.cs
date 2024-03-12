using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Web.Resources;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public class UpdateOrganisationCharityNumberValidator : IUpdateOrganisationCharityNumberValidator
    {
        private IRoatpApiClient _apiClient;

        public UpdateOrganisationCharityNumberValidator(IRoatpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<ValidationErrorDetail> IsDuplicateCharityNumber(UpdateOrganisationCharityNumberViewModel viewModel)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            var duplicateCheckResponse = _apiClient
                .DuplicateCharityNumberCheck(viewModel.OrganisationId, viewModel.CharityNumber).Result;

            if (duplicateCheckResponse == null || !duplicateCheckResponse.DuplicateFound) return errorMessages;

            var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.CharityNumberDuplicateMatch,
                duplicateCheckResponse.DuplicateOrganisationName);
            errorMessages.Add(new ValidationErrorDetail("CharityNumber", duplicateErrorMessage));

            return errorMessages;
        }
    }
}
