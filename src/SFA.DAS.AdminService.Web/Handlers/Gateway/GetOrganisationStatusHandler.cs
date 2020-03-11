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

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetOrganisationStatusHandler : IRequestHandler<GetOrganisationStatusRequest, OrganisationStatusViewModel>
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApiClient _roatpApiClient;

        private readonly ICompaniesHouseApiClient _companiesHouseApiClient;
        private readonly ICharityCommissionApiClient _charityCommissionApiClient;

        public GetOrganisationStatusHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IRoatpApiClient roatpApiClient, ICompaniesHouseApiClient companiesHouseApiClient, ICharityCommissionApiClient charityCommissionApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _roatpApiClient = roatpApiClient;
            _companiesHouseApiClient = companiesHouseApiClient;
            _charityCommissionApiClient = charityCommissionApiClient;
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

            var companyNumber = await _qnaApiClient.GetQuestionTag(request.ApplicationId, "UKRLPVerificationCompanyNumber");
            var charityNumber = await _qnaApiClient.GetQuestionTag(request.ApplicationId, "UKRLPVerificationCharityRegNumber");

            var ukrlpData = await _roatpApiClient.GetUkrlpProviderDetails(ukprn);
            if (ukrlpData.Any())
            {
                model.UkrlpData = ukrlpData.FirstOrDefault().ProviderStatus.CapitaliseFirstLetter(); // 
            }

            if (!string.IsNullOrEmpty(companyNumber))
            {
                var companyDetails = await _companiesHouseApiClient.GetCompanyDetails(companyNumber);

                if (companyDetails != null && !string.IsNullOrEmpty(companyDetails.Status))
                    model.CompaniesHouseData = companyDetails.Status.CapitaliseFirstLetter();
            }

            if (!string.IsNullOrEmpty(charityNumber) && int.TryParse(charityNumber, out var charityNumberNumeric))
            {
                var charityDetails = await _charityCommissionApiClient.GetCharityDetails(charityNumberNumeric);

                if (!string.IsNullOrEmpty(charityDetails?.Response?.Status))
                    model.CharityCommissionData = charityDetails.Response.Status.CapitaliseFirstLetter();
            }

            var pageData = JsonConvert.SerializeObject(model);
            await _applyApiClient.SubmitGatewayPageAnswer(model.ApplicationId, pageId, model.Status, request.UserName, pageData);

            return model;
        }
    }
}
