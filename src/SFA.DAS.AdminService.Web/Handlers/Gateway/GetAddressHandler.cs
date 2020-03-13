using MediatR;
using Microsoft.Extensions.Logging;
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

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetAddressHandler : IRequestHandler<GetAddressRequest, AddressCheckViewModel>
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApiClient _roatpApiClient;

        //private readonly ICompaniesHouseApiClient _companiesHouseApiClient;
        //private readonly ICharityCommissionApiClient _charityCommissionApiClient;

        private readonly ILogger<GetOrganisationStatusHandler> _logger;

        public GetAddressHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IRoatpApiClient roatpApiClient, ILogger<GetOrganisationStatusHandler> logger)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _roatpApiClient = roatpApiClient;
            _logger = logger;
        }

        public async Task<AddressCheckViewModel> Handle(GetAddressRequest request, CancellationToken cancellationToken)
        {
            var pageId = GatewayPageIds.AddressCheck;

            var model = new AddressCheckViewModel { ApplicationId = request.ApplicationId, PageId = pageId };
            model.Caption = RoatpGatewayConstants.Captions.OrganisationChecks;
            model.Heading = RoatpGatewayConstants.Headings.AddressCheck;

            var currentRecord = await _applyApiClient.GetGatewayPageAnswer(request.ApplicationId, pageId);
            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);

            var gatewayReviewStatus = string.Empty;
            if (applicationDetails?.GatewayReviewStatus != null)
            {
                gatewayReviewStatus = applicationDetails.GatewayReviewStatus;
            }

            if (currentRecord?.GatewayPageData != null)
            {
                model = JsonConvert.DeserializeObject<AddressCheckViewModel>(currentRecord.GatewayPageData);
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
            model.UkrlpLegalName = await _qnaApiClient.GetQuestionTag(request.ApplicationId, RoatpQnaConstants.QnaQuestionTags.UKRLPLegalName);

            var PreamblePage = await _qnaApiClient.GetPageBySectionNo(request.ApplicationId, 0, 1, RoatpQnaConstants.RoatpSections.Preamble.PageId);
            var applyAddressLine1 = PreamblePage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKRLPLegalAddressLine1).FirstOrDefault().Value;
            var applyAddressLine2 = PreamblePage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKRLPLegalAddressLine2).FirstOrDefault().Value;
            var applyAddressLine3 = PreamblePage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKRLPLegalAddressLine3).FirstOrDefault().Value;
            var applyAddressLine4 = PreamblePage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKRLPLegalAddressLine4).FirstOrDefault().Value;
            var applyTown = PreamblePage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKRLPLegalAddressTown).FirstOrDefault().Value;
            var applyPostcode = PreamblePage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKRLPLegalAddressPostcode).FirstOrDefault().Value;

            var applyAarray = new[] { applyAddressLine1, applyAddressLine2, applyAddressLine3, applyAddressLine4, applyTown, applyPostcode };
            var applyAddress = string.Join(", ", applyAarray.Where(s => !string.IsNullOrEmpty(s)));
            model.SubmittedApplicationAddress = applyAddress;

            var ukrlpData = await _roatpApiClient.GetUkrlpProviderDetails(ukprn);
            if (ukrlpData.Any())
            {
                var ukrlpAddressLine1 = ukrlpData.FirstOrDefault().ContactDetails.FirstOrDefault().ContactAddress.Address1;
                var ukrlpAddressLine2 = ukrlpData.FirstOrDefault().ContactDetails.FirstOrDefault().ContactAddress.Address2;
                var ukrlpAddressLine3 = ukrlpData.FirstOrDefault().ContactDetails.FirstOrDefault().ContactAddress.Address3;
                var ukrlpAddressLine4 = ukrlpData.FirstOrDefault().ContactDetails.FirstOrDefault().ContactAddress.Address4;
                var ukrlpTown = ukrlpData.FirstOrDefault().ContactDetails.FirstOrDefault().ContactAddress.Town;
                var ukrlpPostCode = ukrlpData.FirstOrDefault().ContactDetails.FirstOrDefault().ContactAddress.PostCode;

                var ukrlpAarray = new[] { ukrlpAddressLine1, ukrlpAddressLine2, ukrlpAddressLine3, ukrlpAddressLine4, ukrlpTown, ukrlpPostCode };
                var ukrlpAddress = string.Join(", ", ukrlpAarray.Where(s => !string.IsNullOrEmpty(s)));
                model.UkrlpAddress = ukrlpAddress; 
            }

            var pageData = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"GetOrganisationStatusHandler-SubmitGatewayPageAnswer - ApplicationId '{model.ApplicationId}' - PageId '{model.PageId}' - Status '{model.Status}' - UserName '{request.UserName}' - PageData '{pageData}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(model.ApplicationId, pageId, model.Status, request.UserName, pageData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOrganisationStatusHandler - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }

            return model;
        }
    }
}
