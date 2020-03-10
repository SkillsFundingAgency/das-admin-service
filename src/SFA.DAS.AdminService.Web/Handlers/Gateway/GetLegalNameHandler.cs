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
        public GetLegalNameHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IRoatpApiClient roatpApiClient, IHttpContextAccessor contextAccessor, IGetTagFromApplyDataService getTagFromApplyDataService)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _roatpApiClient = roatpApiClient;
            _contextAccessor = contextAccessor;
            _getTagFromApplyDataService = getTagFromApplyDataService;
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

            // go get various question tags
            // use session cache?
            var ukprn = _getTagFromApplyDataService.GetValueFromQuestionTag(qnaApplyData, "UKPRN");
            model.Ukprn = ukprn;
            model.ApplyLegalName = _getTagFromApplyDataService.GetValueFromQuestionTag(qnaApplyData, "UKRLPLegalName");
            var companyNumber = _getTagFromApplyDataService.GetValueFromQuestionTag(qnaApplyData, "UKRLPVerificationCompanyNumber");
            var charityNumber = _getTagFromApplyDataService.GetValueFromQuestionTag(qnaApplyData, "UKRLPVerificationCharityRegNumber");


            // go get Legal name from ukrlp
            // use seesion cache?
            var ukrlpLegalName = "";

            var ukrlpData = await _roatpApiClient.GetUkrlpProviderDetails(ukprn);
            if (ukrlpData.Any())
            {
                var ukrlpDetail = ukrlpData.First();
                ukrlpLegalName = ukrlpDetail.ProviderName;
            }
            model.UkrlpLegalName = ukrlpLegalName;


            // get company name from company number
            // use session cache?
            if (!string.IsNullOrEmpty(companyNumber))
            {
                var companyDetails = await _applyApiClient.GetCompanyDetails(companyNumber);

                if (companyDetails != null && !string.IsNullOrEmpty(companyDetails.CompanyName))
                    model.CompaniesHouseLegalName  = companyDetails.CompanyName;
            }

            // get charity name from charity number
            // use session cache?
            if (!string.IsNullOrEmpty(charityNumber))
            {
                var charityDetails = await _applyApiClient.GetCharityDetails(charityNumber);

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
