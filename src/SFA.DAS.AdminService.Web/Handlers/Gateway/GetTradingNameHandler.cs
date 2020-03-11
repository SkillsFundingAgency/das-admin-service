using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetTradingNameHandler : IRequestHandler<GetTradingNameRequest, TradingNamePageViewModel>
    {

        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApiClient _roatpApiClient;


        public GetTradingNameHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IRoatpApiClient roatpApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _roatpApiClient = roatpApiClient;
        }


        public async Task<TradingNamePageViewModel> Handle(GetTradingNameRequest request, CancellationToken cancellationToken)

        {
            var pageId = GatewayPageIds.TradingName;

            var model = new TradingNamePageViewModel { ApplicationId = request.ApplicationId, PageId = pageId };

            var currentRecord = await _applyApiClient.GetGatewayPageAnswer(request.ApplicationId, pageId);
            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);

            var gatewayReviewStatus = string.Empty;
            if (applicationDetails?.GatewayReviewStatus != null)
            {
                gatewayReviewStatus = applicationDetails.GatewayReviewStatus;
            }

            if (currentRecord?.GatewayPageData != null)
            {
                model = JsonConvert.DeserializeObject<TradingNamePageViewModel>(currentRecord.GatewayPageData);
                model.Status = currentRecord.Status;
                model.GatewayReviewStatus = gatewayReviewStatus;
                return model;
            }

            model.GatewayReviewStatus = gatewayReviewStatus;
            if (applicationDetails?.ApplyData?.ApplyDetails?.ApplicationSubmittedOn != null)
                model.ApplicationSubmittedOn = applicationDetails.ApplyData.ApplyDetails.ApplicationSubmittedOn;

            var ukprn = await _qnaApiClient.GetQuestionTag(request.ApplicationId, "UKPRN");
            model.Ukprn = ukprn;

            var tradingNameAndWebsitePage = await _qnaApiClient.GetPageBySectionNo(request.ApplicationId, 0, 1, "1");
            model.ApplyTradingName = tradingNameAndWebsitePage?.PageOfAnswers?.SelectMany(a => a.Answers)?.FirstOrDefault(a => a.QuestionId == "PRE-46")?.Value;

            var ukrlpData = await _roatpApiClient.GetUkrlpProviderDetails(ukprn);
            if (ukrlpData != null && ukrlpData.Any())
            {
                var ukrlpDetail = ukrlpData.First();
                if (ukrlpDetail.ProviderAliases != null && ukrlpDetail.ProviderAliases.Count > 0)
                {
                    model.UkrlpTradingName = ukrlpDetail.ProviderAliases.First().Alias;
                }
            }

            var pageData = JsonConvert.SerializeObject(model);

            await _applyApiClient.SubmitGatewayPageAnswer(model.ApplicationId, pageId, model.Status, request.UserName, pageData);

            return model;
        }



    }
}
