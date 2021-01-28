
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public class RoatpApiClient : RoatpApiClientBase<RoatpApiClient>, IRoatpApiClient
    {
        public RoatpApiClient(ILogger<RoatpApiClient> logger, IRoatpTokenService tokenService, IWebConfiguration configuration) : base(configuration.RoatpApiClientBaseUrl, logger, tokenService)
        {
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
        {
            string url = $"/api/v1/download/audit";
            _logger.LogInformation($"Retrieving RoATP register audit history data from {url}");

            return await Get<List<IDictionary<string, object>>>(url);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            string url = $"/api/v1/download/complete";
            _logger.LogInformation($"Retrieving RoATP complete register data from {url}");
            return await Get<List<IDictionary<string, object>>>(url);
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId)
        {
            return await Get<List<OrganisationType>>($"/api/v1/lookupData/organisationTypes?providerTypeId={providerTypeId}");
        }

        public async Task<IEnumerable<ProviderType>> GetProviderTypes()
        {
            return await Get<List<ProviderType>>($"/api/v1/lookupData/providerTypes");
        }

        public async Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses(int? providerTypeId)
        {
            return await Get<List<OrganisationStatus>>($"/api/v1/lookupData/organisationStatuses?providerTypeId={providerTypeId}");
        }

        public async Task<IEnumerable<RemovedReason>> GetRemovedReasons()
        {
            return await Get<List<RemovedReason>>($"/api/v1/lookupData/removedReasons");
        }

        public async Task<bool> CreateOrganisation(CreateRoatpOrganisationRequest organisationRequest)
        {
           HttpStatusCode result = await Post<CreateRoatpOrganisationRequest>($"/api/v1/organisation/create", organisationRequest);

           return await Task.FromResult(result == HttpStatusCode.OK);
        }
        
        public async Task<DuplicateCheckResponse> DuplicateUKPRNCheck(Guid organisationId, long ukprn)
        {
            return await Get<DuplicateCheckResponse>($"/api/v1/duplicateCheck/ukprn?ukprn={ukprn}&organisationId={organisationId}");
        }

        public async Task<DuplicateCheckResponse> DuplicateCompanyNumberCheck(Guid organisationId, string companyNumber)
        {
            return await Get<DuplicateCheckResponse>($"/api/v1/duplicateCheck/companyNumber?companyNumber={companyNumber}&organisationId={organisationId}");
        }

        public async Task<DuplicateCheckResponse> DuplicateCharityNumberCheck(Guid organisationId, string charityNumber)
        {
            return await Get<DuplicateCheckResponse>($"/api/v1/duplicateCheck/charityNumber?charityNumber={charityNumber}&organisationId={organisationId}");
        }

        public async Task<OrganisationSearchResults> Search(string searchTerm)
        {
            return await Get<OrganisationSearchResults>($"/api/v1/search?searchTerm={searchTerm}");
        }

        public async Task<bool> UpdateOrganisationLegalName(UpdateOrganisationLegalNameRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationLegalNameRequest>($"/api/v1/updateOrganisation/legalName", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }
        
        public async Task<bool> UpdateOrganisationStatus(UpdateOrganisationStatusRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationStatusRequest>($"/api/v1/updateOrganisation/status", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateOrganisationType(UpdateOrganisationTypeRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationTypeRequest>($"/api/v1/updateOrganisation/type", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateOrganisationTradingName(UpdateOrganisationTradingNameRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationTradingNameRequest>($"/api/v1/updateOrganisation/tradingName", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }
        
        public async Task<bool> UpdateOrganisationParentCompanyGuarantee(UpdateOrganisationParentCompanyGuaranteeRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationParentCompanyGuaranteeRequest>($"/api/v1/updateOrganisation/parentCompanyGuarantee", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }
       
        public async Task<bool> UpdateOrganisationFinancialTrackRecord(UpdateOrganisationFinancialTrackRecordRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationFinancialTrackRecordRequest>($"/api/v1/updateOrganisation/financialTrackRecord", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateOrganisationProviderType(UpdateOrganisationProviderTypeRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationProviderTypeRequest>($"/api/v1/updateOrganisation/providerType", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateOrganisationUkprn(UpdateOrganisationUkprnRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationUkprnRequest>($"/api/v1/updateOrganisation/ukprn", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }
        
        public async Task<bool> UpdateOrganisationCompanyNumber(UpdateOrganisationCompanyNumberRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationCompanyNumberRequest>($"/api/v1/updateOrganisation/companyNumber", request);
            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateOrganisationCharityNumber(UpdateOrganisationCharityNumberRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationCharityNumberRequest>($"/api/v1/updateOrganisation/charityNumber", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateApplicationDeterminedDate(UpdateOrganisationApplicationDeterminedDateRequest request)
        {
            HttpStatusCode result = await Put<UpdateOrganisationApplicationDeterminedDateRequest>($"/api/v1/updateOrganisation/applicationDeterminedDate", request);

            return await Task.FromResult(result == HttpStatusCode.OK);
        }

        public async Task<IEnumerable<ProviderDetails>> GetUkrlpProviderDetails(string ukprn)
        {
            var res =
                await Get<UkprnLookupResponse>($"/api/v1/ukrlp/lookup/{ukprn}");

            return res.Results;
        }
        public async Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn)
        {
            return await Get<OrganisationRegisterStatus>($"/api/v1/Organisation/register-status?ukprn={ukprn}");
        }
    }
}
