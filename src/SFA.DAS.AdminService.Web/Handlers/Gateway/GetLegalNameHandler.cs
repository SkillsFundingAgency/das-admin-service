using MediatR;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetLegalNameHandler : IRequestHandler<GetLegalNameRequest, LegalNamePageViewModel>
    {

        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGetTagFromApplyDataService _getTagFromApplyDataService;
        private readonly IApplyServicePassthroughApiClient _applyServicePassthroughApiClient;
        public GetLegalNameHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IRoatpApiClient roatpApiClient, IHttpContextAccessor contextAccessor, IGetTagFromApplyDataService getTagFromApplyDataService, IApplyServicePassthroughApiClient applyServicePassthroughApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _roatpApiClient = roatpApiClient;
            _contextAccessor = contextAccessor;
            _getTagFromApplyDataService = getTagFromApplyDataService;
            _applyServicePassthroughApiClient = applyServicePassthroughApiClient;
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

            var qnaApplyData = await _qnaApiClient.GetApplicationData(model.ApplicationId);

            var ukprn = _getTagFromApplyDataService.GetValueFromQuestionTag(qnaApplyData, "UKPRN");
            model.Ukprn = ukprn;
            model.ApplyLegalName = _getTagFromApplyDataService.GetValueFromQuestionTag(qnaApplyData, "UKRLPLegalName");
            var companyNumber = _getTagFromApplyDataService.GetValueFromQuestionTag(qnaApplyData, "UKRLPVerificationCompanyNumber");
            var charityNumber = _getTagFromApplyDataService.GetValueFromQuestionTag(qnaApplyData, "UKRLPVerificationCharityRegNumber");

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
                var companyDetails = await _applyServicePassthroughApiClient.GetCompanyDetails(companyNumber);

                if (companyDetails != null && !string.IsNullOrEmpty(companyDetails.CompanyName))
                    model.CompaniesHouseLegalName  = companyDetails.CompanyName;
            }

            if (!string.IsNullOrEmpty(charityNumber))
            {
                var charityDetails = await _applyServicePassthroughApiClient.GetCharityDetails(charityNumber);

                if (charityDetails != null && !string.IsNullOrEmpty(charityDetails.Name))
                    model.CharityCommissionLegalName = charityDetails.Name;
            }

            var pageData = JsonConvert.SerializeObject(model);

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            await _applyApiClient.SubmitGatewayPageAnswer(model.ApplicationId, pageId, model.Status, username, pageData);
            
            return model;
        }

        
        
    }
}
