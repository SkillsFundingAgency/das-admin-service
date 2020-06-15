using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Resources;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public class UpdateOrganisationCompanyNumberValidator : IUpdateOrganisationCompanyNumberValidator
    {
        private IRoatpApiClient _apiClient;

        public UpdateOrganisationCompanyNumberValidator(IRoatpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<ValidationErrorDetail> IsDuplicateCompanyNumber(UpdateOrganisationCompanyNumberViewModel viewModel)
        {
            var errorMessages = new List<ValidationErrorDetail>();

         
            var duplicateCheckResponse = _apiClient.DuplicateCompanyNumberCheck(viewModel.OrganisationId, viewModel.CompanyNumber).Result;

            if (duplicateCheckResponse == null || !duplicateCheckResponse.DuplicateFound) return errorMessages;

            var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.CompanyNumberDuplicateMatch,
                duplicateCheckResponse.DuplicateOrganisationName);
            errorMessages.Add(new ValidationErrorDetail("CompanyNumber", duplicateErrorMessage));
          

            return errorMessages;
        }
    }
}
