using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models.Register;

namespace SFA.DAS.AdminService.Web.Services
{
    public class AssessorValidationService : IAssessorValidationService
    {
        private readonly IApiClient _apiClient;

        public AssessorValidationService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ValidationResponse> ValidateNewOrganisationRequest(CreateEpaOrganisationRequest request)
        {
            var validationRequest = MapToCreateEpaOrganisationValidateRequest(request);
            return await _apiClient.CreateOrganisationValidate(validationRequest);
        }


        public async Task<ValidationResponse> ValidateNewContactRequest(CreateEpaOrganisationContactRequest request)
        {
            var validationRequest = MapToCreateEpaContactValidateRequest(request);
            return await _apiClient.CreateEpaContactValidate(validationRequest);
        }

        public async Task<ValidationResponse> ValidateNewOrganisationStandardRequest(CreateEpaOrganisationStandardRequest request)
        {
            var validationRequest = MapToCreateEpaOrganisationStandardValidateRequest(request);
            return await _apiClient.CreateOrganisationStandardValidate(validationRequest);
        }

        public async Task<ValidationResponse> ValidateUpdateOrganisationRequest(UpdateEpaOrganisationRequest request)
        {
            var validationRequest = MapToUpdateEpaOrganisationValidateRequest(request);
            return await _apiClient.UpdateOrganisationValidate(validationRequest);
        }

        private CreateEpaOrganisationValidationRequest MapToCreateEpaOrganisationValidateRequest(CreateEpaOrganisationRequest request)
        {
            return new CreateEpaOrganisationValidationRequest
            {
                Name = request?.Name,
                Ukprn = request?.Ukprn,
                OrganisationTypeId = request?.OrganisationTypeId,
                CompanyNumber = request?.CompanyNumber,
                CharityNumber = request?.CharityNumber
            };
        }

        private CreateEpaContactValidationRequest MapToCreateEpaContactValidateRequest(CreateEpaOrganisationContactRequest request)
        {
            return new CreateEpaContactValidationRequest
            {
                OrganisationId = request?.EndPointAssessorOrganisationId,
                FirstName = request?.FirstName,
                LastName = request?.LastName,
                Email = request?.Email,
                Phone = request?.PhoneNumber
            };
        }

        private CreateEpaOrganisationStandardValidationRequest MapToCreateEpaOrganisationStandardValidateRequest(CreateEpaOrganisationStandardRequest request)
        {
            return new CreateEpaOrganisationStandardValidationRequest
            {
                OrganisationId = request?.OrganisationId,
                StandardCode = request?.StandardCode ?? 0,
                EffectiveFrom = request?.EffectiveFrom,
                EffectiveTo = request?.EffectiveTo,
                ContactId = request?.ContactId,
                DeliveryAreas = request?.DeliveryAreas
            };
        }

        private UpdateEpaOrganisationValidationRequest MapToUpdateEpaOrganisationValidateRequest(UpdateEpaOrganisationRequest request)
        {
            return new UpdateEpaOrganisationValidationRequest
            {
                Name = request?.Name,
                Ukprn = request?.Ukprn,
                OrganisationTypeId = request?.OrganisationTypeId,
                OrganisationId = request?.OrganisationId,
                Address1 = request?.Address1,
                Address2 = request?.Address2,
                Address3 = request?.Address3,
                Address4 = request?.Address4,
                Postcode = request?.Postcode,
                CompanyNumber = request?.CompanyNumber,
                CharityNumber = request?.CharityNumber,
                Status = request?.Status,
                ActionChoice = request?.ActionChoice
            };
        }
    }
}
