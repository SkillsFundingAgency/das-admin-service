﻿
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public interface IRoatpApiClient
    {
        Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister();
        Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory();
        Task<IEnumerable<ProviderType>> GetProviderTypes();
        Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId);
        Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses(int? providerTypeId);
        Task<IEnumerable<RemovedReason>> GetRemovedReasons();
        Task<bool> CreateOrganisation(CreateRoatpOrganisationRequest organisationRequest);
        Task<DuplicateCheckResponse> DuplicateUKPRNCheck(Guid organisationId, long ukprn);
        Task<DuplicateCheckResponse> DuplicateCompanyNumberCheck(Guid organisationId, string companyNumber);
        Task<DuplicateCheckResponse> DuplicateCharityNumberCheck(Guid organisationId, string charityNumber);
        Task<OrganisationSearchResults> Search(string searchTerm);
        Task<bool> UpdateOrganisationLegalName(UpdateOrganisationLegalNameRequest request);
        Task<bool> UpdateOrganisationTradingName(UpdateOrganisationTradingNameRequest request);
        Task<bool> UpdateOrganisationStatus(UpdateOrganisationStatusRequest request);
        Task<bool> UpdateOrganisationType(UpdateOrganisationTypeRequest request);
        Task<bool> UpdateOrganisationParentCompanyGuarantee(UpdateOrganisationParentCompanyGuaranteeRequest request);
        Task<bool> UpdateOrganisationFinancialTrackRecord(UpdateOrganisationFinancialTrackRecordRequest request);
        Task<bool> UpdateOrganisationProviderType(UpdateOrganisationProviderTypeRequest request);
        Task<bool> UpdateOrganisationUkprn(UpdateOrganisationUkprnRequest request);
        Task<bool> UpdateOrganisationCompanyNumber(UpdateOrganisationCompanyNumberRequest request);
        Task<bool> UpdateOrganisationCharityNumber(UpdateOrganisationCharityNumberRequest request);
        Task<bool> UpdateApplicationDeterminedDate(UpdateOrganisationApplicationDeterminedDateRequest request);        
        Task<IEnumerable<ProviderDetails>> GetUkrlpProviderDetails(string ukprn);
        Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn);
    }
}
