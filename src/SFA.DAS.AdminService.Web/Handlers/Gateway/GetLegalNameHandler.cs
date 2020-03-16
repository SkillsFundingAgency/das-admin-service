using MediatR;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetLegalNameHandler : IRequestHandler<GetLegalNameRequest, LegalNamePageViewModel>
    {

        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApiClient _roatpApiClient;
 
        private readonly ICompaniesHouseApiClient _companiesHouseApiClient;
        private readonly ICharityCommissionApiClient _charityCommissionApiClient;

        private readonly ILogger<GetLegalNameHandler> _logger;

        public GetLegalNameHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IRoatpApiClient roatpApiClient, ICompaniesHouseApiClient companiesHouseApiClient, ICharityCommissionApiClient charityCommissionApiClient, ILogger<GetLegalNameHandler> logger)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _roatpApiClient = roatpApiClient;
            _companiesHouseApiClient = companiesHouseApiClient;
            _charityCommissionApiClient = charityCommissionApiClient;
            _logger = logger;
        }

        public async Task<LegalNamePageViewModel> Handle(GetLegalNameRequest request, CancellationToken cancellationToken)

        {
            var pageId = GatewayPageIds.LegalName;

            var model = new LegalNamePageViewModel { ApplicationId = request.ApplicationId, PageId = pageId};
 
            var currentRecord = await _applyApiClient.GetGatewayPageAnswer(request.ApplicationId, pageId);
            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);

            var gatewayReviewStatus = string.Empty;
            if (applicationDetails?.GatewayReviewStatus != null)
            {
                gatewayReviewStatus = applicationDetails.GatewayReviewStatus;
            }

            if (currentRecord?.GatewayPageData != null)
            {
                model = JsonConvert.DeserializeObject<LegalNamePageViewModel>(currentRecord.GatewayPageData);
                model.Status = currentRecord.Status;
                model.GatewayReviewStatus = gatewayReviewStatus;
                return model;
            }

            model.GatewayReviewStatus = gatewayReviewStatus;
            if (applicationDetails?.ApplyData?.ApplyDetails?.ApplicationSubmittedOn != null)
                model.ApplicationSubmittedOn = applicationDetails.ApplyData.ApplyDetails.ApplicationSubmittedOn;

            model.SourcesCheckedOn = DateTime.Now;

            var ukprn = await _qnaApiClient.GetQuestionTag(request.ApplicationId, RoatpQnaConstants.QnaQuestionTags.Ukprn);
            model.Ukprn = ukprn;

            model.ApplyLegalName = await _qnaApiClient.GetQuestionTag(request.ApplicationId, RoatpQnaConstants.QnaQuestionTags.UKRLPLegalName);
            var companyNumber = string.Empty;
            var charityNumber = string.Empty;


            try { companyNumber = await _qnaApiClient.GetQuestionTag(request.ApplicationId, RoatpQnaConstants.QnaQuestionTags.UKRLPVerificationCompanyNumber); }
            catch
            { // not robust to tag not being present, throws a 404
            }

            try
            { charityNumber = await _qnaApiClient.GetQuestionTag(request.ApplicationId, RoatpQnaConstants.QnaQuestionTags.UKRLPVerificationCharityRegNumber); }
            catch
            { // not robust to tag not being present, throws a 404
            }


            var ukrlpLegalName = "";

            var ukrlpData = await _roatpApiClient.GetUkrlpProviderDetails(ukprn);
            if (ukrlpData.Any())
            {
                var ukrlpDetail = ukrlpData.First();
                ukrlpLegalName = ukrlpDetail.ProviderName;
            }
            model.UkrlpLegalName = ukrlpLegalName;


            if (!string.IsNullOrEmpty(companyNumber))
            {
                var companyDetails = await _companiesHouseApiClient.GetCompanyDetails(companyNumber);

                if (companyDetails != null && !string.IsNullOrEmpty(companyDetails.CompanyName))
                    model.CompaniesHouseLegalName = companyDetails.CompanyName;
            }

            if (!string.IsNullOrEmpty(charityNumber) && int.TryParse(charityNumber, out var charityNumberNumeric))
            {
                var charityDetails = await _charityCommissionApiClient.GetCharityDetails(charityNumberNumeric);

                if (!string.IsNullOrEmpty(charityDetails?.Response?.Name))
                    model.CharityCommissionLegalName = charityDetails.Response.Name;
            }

            var pageData = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"GetLegalNameHandler-SubmitGatewayPageAnswer - ApplicationId '{model.ApplicationId}' - PageId '{model.PageId}' - Status '{model.Status}' - UserName '{request.UserName}' - PageData '{pageData}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(model.ApplicationId, pageId, model.Status, request.UserName, pageData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLegalNameHandler - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }

            return model;
        }

        
        
    }
}
