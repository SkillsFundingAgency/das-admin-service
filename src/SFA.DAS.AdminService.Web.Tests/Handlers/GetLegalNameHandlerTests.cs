using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Handlers
{ 
[TestFixture]
    public class GetLegalNameHandlerTests
    {
        private GetLegalNameHandler _handler;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<IRoatpApiClient> _roatpApiClient;

        private Mock<ICompaniesHouseApiClient> _companiesHouseApiClient;
        private Mock<ICharityCommissionApiClient> _charityCommissionApiClient;
        private Mock<ILogger<GetLegalNameHandler>> _logger;
        private static string PageId => "1-10";
        private GatewayPageAnswer _gatewayPageAnswer;
        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";

        private static string CompanyNumber => "654321";
        private static string CharityNumber => "123456";

        private static string ProviderName => "Mark's other workshop";
        private static string CompanyName => "Companies House Name";

        private static string CharityName => "Charity commission Name";

        private CompaniesHouseSummary CompaniesHouseSummary => new CompaniesHouseSummary {CompanyName = CompanyName};

       
        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _qnaApiClient = new Mock<IQnaApiClient>();
            _roatpApiClient = new Mock<IRoatpApiClient>();
            _companiesHouseApiClient = new Mock<ICompaniesHouseApiClient>();
            _charityCommissionApiClient = new Mock<ICharityCommissionApiClient>();
            _gatewayPageAnswer = new GatewayPageAnswer();
            _logger  = new Mock<ILogger<GetLegalNameHandler>>();
            _handler = new GetLegalNameHandler(_applyApiClient.Object,_qnaApiClient.Object,_roatpApiClient.Object,_companiesHouseApiClient.Object,_charityCommissionApiClient.Object, _logger.Object);
        }

        [Test]
        public void check_legal_name_handler_builds_with_company_and_charity_details()
        {
            var applicationId = Guid.NewGuid();
            var username = "mark";
            var applicationSubmittedOn = DateTime.Today;
            _gatewayPageAnswer.ApplicationId = applicationId;
            _gatewayPageAnswer.PageId = PageId;
            var applicationResponse = new RoatpApplicationResponse
            {
                GatewayReviewStatus = GatewayReviewStatus.New,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails {ApplicationSubmittedOn = applicationSubmittedOn}
                }
            };
            var providerDetails = new List<ProviderDetails> {new ProviderDetails {ProviderName = ProviderName}};

            var charity = new Charity {Name = CharityName};
            var charityApiResponse = new ApiResponse<Charity> {Response = charity};

            _applyApiClient.Setup(c => c.GetGatewayPageAnswer(applicationId, PageId)).ReturnsAsync(_gatewayPageAnswer);
            _applyApiClient.Setup(c => c.GetApplication(applicationId)).ReturnsAsync(applicationResponse);

            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKPRN")).ReturnsAsync(ukprn);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPLegalName")).ReturnsAsync(UKRLPLegalName);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPVerificationCompanyNumber")).ReturnsAsync(CompanyNumber);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPVerificationCharityRegNumber")).ReturnsAsync(CharityNumber);
            _roatpApiClient.Setup(c => c.GetUkrlpProviderDetails(ukprn)).ReturnsAsync(providerDetails);
            _companiesHouseApiClient.Setup(c => c.GetCompanyDetails(CompanyNumber)).ReturnsAsync(CompaniesHouseSummary);
            _charityCommissionApiClient.Setup(c => c.GetCharityDetails(Int32.Parse(CharityNumber)))
                .ReturnsAsync(charityApiResponse);

            var request = new GetLegalNameRequest(applicationId,username);

            var response = _handler.Handle(request, new CancellationToken());

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName,viewModel.ApplyLegalName);
            Assert.AreEqual(ProviderName, viewModel.UkrlpLegalName);
            Assert.AreEqual(CompanyName, viewModel.CompaniesHouseLegalName);
            Assert.AreEqual(CharityName, viewModel.CharityCommissionLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }

        [Test]
        public void check_legal_name_handler_builds_with_company_details_only()
        {
            var applicationId = Guid.NewGuid();
            var username = "mark";
            var applicationSubmittedOn = DateTime.Today;
            _gatewayPageAnswer.ApplicationId = applicationId;
            _gatewayPageAnswer.PageId = PageId;
            var applicationResponse = new RoatpApplicationResponse
            {
                GatewayReviewStatus = GatewayReviewStatus.New,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails { ApplicationSubmittedOn = applicationSubmittedOn }
                }
            };
            var providerDetails = new List<ProviderDetails> { new ProviderDetails { ProviderName = ProviderName } };

            var charity = new Charity { Name = CharityName };
            var charityApiResponse = new ApiResponse<Charity> { Response = null };

            _applyApiClient.Setup(c => c.GetGatewayPageAnswer(applicationId, PageId)).ReturnsAsync(_gatewayPageAnswer);
            _applyApiClient.Setup(c => c.GetApplication(applicationId)).ReturnsAsync(applicationResponse);

            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKPRN")).ReturnsAsync(ukprn);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPLegalName")).ReturnsAsync(UKRLPLegalName);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPVerificationCompanyNumber")).ReturnsAsync(CompanyNumber);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPVerificationCharityRegNumber")).ReturnsAsync(CharityNumber);
            _roatpApiClient.Setup(c => c.GetUkrlpProviderDetails(ukprn)).ReturnsAsync(providerDetails);
            _companiesHouseApiClient.Setup(c => c.GetCompanyDetails(CompanyNumber)).ReturnsAsync(CompaniesHouseSummary);
            _charityCommissionApiClient.Setup(c => c.GetCharityDetails(Int32.Parse(CharityNumber)))
                .ReturnsAsync(charityApiResponse);

            var request = new GetLegalNameRequest(applicationId, username);

            var response = _handler.Handle(request, new CancellationToken());

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ProviderName, viewModel.UkrlpLegalName);
            Assert.AreEqual(CompanyName, viewModel.CompaniesHouseLegalName);
            Assert.AreEqual(null, viewModel.CharityCommissionLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }

        [Test]
        public void check_legal_name_handler_builds_with_charity_details_only()
        {
            var applicationId = Guid.NewGuid();
            var username = "mark";
            var applicationSubmittedOn = DateTime.Today;
            _gatewayPageAnswer.ApplicationId = applicationId;
            _gatewayPageAnswer.PageId = PageId;
            var applicationResponse = new RoatpApplicationResponse
            {
                GatewayReviewStatus = GatewayReviewStatus.New,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails { ApplicationSubmittedOn = applicationSubmittedOn }
                }
            };
            var providerDetails = new List<ProviderDetails> { new ProviderDetails { ProviderName = ProviderName } };

            var charity = new Charity { Name = CharityName };
            var charityApiResponse = new ApiResponse<Charity> { Response = charity };

            _applyApiClient.Setup(c => c.GetGatewayPageAnswer(applicationId, PageId)).ReturnsAsync(_gatewayPageAnswer);
            _applyApiClient.Setup(c => c.GetApplication(applicationId)).ReturnsAsync(applicationResponse);

            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKPRN")).ReturnsAsync(ukprn);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPLegalName")).ReturnsAsync(UKRLPLegalName);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPVerificationCompanyNumber")).ReturnsAsync(CompanyNumber);
            _qnaApiClient.Setup(c => c.GetQuestionTag(applicationId, "UKRLPVerificationCharityRegNumber")).ReturnsAsync(CharityNumber);
            _roatpApiClient.Setup(c => c.GetUkrlpProviderDetails(ukprn)).ReturnsAsync(providerDetails);
            _companiesHouseApiClient.Setup(c => c.GetCompanyDetails(CompanyNumber)).ReturnsAsync(new CompaniesHouseSummary { CompanyName = null });
            _charityCommissionApiClient.Setup(c => c.GetCharityDetails(Int32.Parse(CharityNumber)))
                .ReturnsAsync(charityApiResponse);

            var request = new GetLegalNameRequest(applicationId, username);

            var response = _handler.Handle(request, new CancellationToken());

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ProviderName, viewModel.UkrlpLegalName);
            Assert.AreEqual(null, viewModel.CompaniesHouseLegalName);
            Assert.AreEqual(CharityName, viewModel.CharityCommissionLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }
    }
}
