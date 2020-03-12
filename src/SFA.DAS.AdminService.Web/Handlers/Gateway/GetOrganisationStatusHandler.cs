using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Extensions;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetOrganisationStatusHandler : IRequestHandler<GetOrganisationStatusRequest, OrganisationStatusViewModel>
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApiClient _roatpApiClient;

        private readonly ICompaniesHouseApiClient _companiesHouseApiClient;
        private readonly ICharityCommissionApiClient _charityCommissionApiClient;

        private readonly ILogger<GetOrganisationStatusHandler> _logger;

        public GetOrganisationStatusHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IRoatpApiClient roatpApiClient, ICompaniesHouseApiClient companiesHouseApiClient, ICharityCommissionApiClient charityCommissionApiClient, ILogger<GetOrganisationStatusHandler> logger)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _roatpApiClient = roatpApiClient;
            _companiesHouseApiClient = companiesHouseApiClient;
            _charityCommissionApiClient = charityCommissionApiClient;
            _logger = logger;
        }

        public async Task<OrganisationStatusViewModel> Handle(GetOrganisationStatusRequest request, CancellationToken cancellationToken)
        {
            var pageId = GatewayPageIds.OrganisationStatus;

            var model = new OrganisationStatusViewModel { ApplicationId = request.ApplicationId, PageId = pageId };

            var currentRecord = await _applyApiClient.GetGatewayPageAnswer(request.ApplicationId, pageId);
            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);

            var gatewayReviewStatus = string.Empty;
            if (applicationDetails?.GatewayReviewStatus != null)
            {
                gatewayReviewStatus = applicationDetails.GatewayReviewStatus;
            }

            if (currentRecord?.GatewayPageData != null)
            {
                model = JsonConvert.DeserializeObject<OrganisationStatusViewModel>(currentRecord.GatewayPageData);
                model.Status = currentRecord.Status;
                model.GatewayReviewStatus = gatewayReviewStatus;
                return model;
            }

            model.GatewayReviewStatus = gatewayReviewStatus;
            if (applicationDetails?.ApplyData?.ApplyDetails?.ApplicationSubmittedOn != null)
                model.ApplicationSubmittedOn = applicationDetails.ApplyData.ApplyDetails.ApplicationSubmittedOn;

            model.SourcesCheckedOn = DateTime.Now;

            var ukprn = await _qnaApiClient.GetQuestionTag(request.ApplicationId, "UKPRN");
            model.Ukprn = ukprn;

            var companyNumber = string.Empty;
            var charityNumber = string.Empty;


            try { companyNumber = await _qnaApiClient.GetQuestionTag(request.ApplicationId, "UKRLPVerificationCompanyNumber"); }
            catch
            { // not robust to tag not being present, throws a 404
            }


            try
            { charityNumber = await _qnaApiClient.GetQuestionTag(request.ApplicationId, "UKRLPVerificationCharityRegNumber"); }
            catch
            { // not robust to tag not being present, throws a 404
            }

            var ukrlpData = await _roatpApiClient.GetUkrlpProviderDetails(ukprn);
            if (ukrlpData.Any())
            {
                model.UkrlpStatus = ukrlpData.FirstOrDefault().ProviderStatus.CapitaliseFirstLetter(); // 
            }

            if (!string.IsNullOrEmpty(companyNumber))
            {
                var companyDetails = await _companiesHouseApiClient.GetCompanyDetails(companyNumber);

                if (companyDetails != null && !string.IsNullOrEmpty(companyDetails.Status))
                    model.CompaniesHouseStatus = companyDetails.Status.CapitaliseFirstLetter();
            }

            if (!string.IsNullOrEmpty(charityNumber) && int.TryParse(charityNumber, out var charityNumberNumeric))
            {
                var charityDetails = await _charityCommissionApiClient.GetCharityDetails(charityNumberNumeric);

                if (!string.IsNullOrEmpty(charityDetails?.Response?.Status))
                    model.CharityCommissionStatus = charityDetails.Response.Status.CapitaliseFirstLetter();
            }

            var pageData = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"GetOrganisationStatusHandler-SubmitGatewayPageAnswer - ApplicationId '{model.ApplicationId}' - PageId '{model.PageId}' - Status '{model.Status}' - UserName '{request.UserName}' - PageData '{pageData}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(model.ApplicationId, pageId, model.Status, request.UserName, pageData);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "GetOrganisationStatusHandler - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }

            return model;
        }
    }
}
