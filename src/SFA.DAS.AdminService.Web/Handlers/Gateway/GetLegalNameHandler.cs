using MediatR;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetLegalNameHandler : IRequestHandler<GetLegalNameRequest, LegalNamePageViewModel>
    {

        private readonly IRoatpApplicationApiClient _applyApiClient;
        

        private readonly ILogger<GetLegalNameHandler> _logger;

        public GetLegalNameHandler(IRoatpApplicationApiClient applyApiClient, ILogger<GetLegalNameHandler> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        public async Task<LegalNamePageViewModel> Handle(GetLegalNameRequest request,
            CancellationToken cancellationToken)

        {
            var pageId = GatewayPageIds.LegalName;

            var model = new LegalNamePageViewModel {ApplicationId = request.ApplicationId, PageId = pageId};

            //MFCMFC remove magic words
            model.GatewayReviewStatus = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId,
                request.UserName, "GatewayReviewStatus");

            model.ApplyLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId,
                request.UserName, "OrganisationName");
            model.Ukprn =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    "UKPRN");

            model.UkrlpLegalName =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    "UkrlpLegalName");


            var applicationSubmittedOn =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    "ApplicationSubmittedOn");

            if (applicationSubmittedOn != null && DateTime.TryParse(applicationSubmittedOn, out var submittedOn))
                model.ApplicationSubmittedOn = submittedOn;

            var sourcesCheckedOn =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    "SourcesCheckedOn");

            if (applicationSubmittedOn != null && DateTime.TryParse(sourcesCheckedOn, out var checkedOn))
                model.SourcesCheckedOn = checkedOn;

            model.CompaniesHouseLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                pageId,
                request.UserName, "CompaniesHouseName");


            model.CharityCommissionLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                pageId,
                request.UserName, "CharityCommissionName");


            var currentStatus = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId,
                request.UserName, "Status");

            model.Status = currentStatus;

            if (string.IsNullOrEmpty(currentStatus)) return model;
            switch (currentStatus)
            {
                case SectionReviewStatus.Pass:
                    model.OptionPassText = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                        pageId,
                        request.UserName, "OptionPassText");
                    break;
                case SectionReviewStatus.Fail:
                    model.OptionFailText = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                        pageId,
                        request.UserName, "OptionFailText");
                    break;
                case SectionReviewStatus.InProgress:
                    model.OptionInProgressText = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                        pageId,
                        request.UserName, "OptionInProgressText");
                    break;
            }

            return model;
        }
    }
}
