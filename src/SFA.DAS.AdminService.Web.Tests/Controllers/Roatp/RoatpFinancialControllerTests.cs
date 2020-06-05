using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Roatp
{

    [TestFixture]
    public class RoatpFinancialControllerTests
    {
        private Mock<IRoatpOrganisationApiClient> _roatpOrganisationApiClient;
        private Mock<IRoatpApplicationApiClient> _applicationApplyApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;
        private RoatpFinancialController _controller;
        private readonly Guid _applicationId = Guid.NewGuid();
        private string _emailAddress = "Test@test.com";

        [SetUp]
        public void Before_each_test()
        {
            _roatpOrganisationApiClient = new Mock<IRoatpOrganisationApiClient>();
            _applicationApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            _qnaApiClient = new Mock<IQnaApiClient>();

            _controller = new RoatpFinancialController(_roatpOrganisationApiClient.Object,
                _applicationApplyApiClient.Object,
                _qnaApiClient.Object,
                Mock.Of<IHttpContextAccessor>());

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

        }

        [Test]
        public void ViewApplication_creates_correct_view_model_with_email()
        {
            _applicationApplyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(
                new RoatpApplicationResponse {ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData {ApplyDetails =  new RoatpApplyDetails
                {
                    OrganisationName = "org name",
                    UKPRN = "12344321",
                    ReferenceNumber = "3443",
                    ProviderRouteName = "main",
                    ApplicationSubmittedOn = DateTime.Today
                },Sequences = new List<RoatpApplySequence>
                    {
                        new RoatpApplySequence
                        {
                            SequenceNo = 5,
                            NotRequired = true
                        }
                    }}});

            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());
            _qnaApiClient
                .Setup(x => x.GetQuestionTag(_applicationId, RoatpQnaConstants.QnaQuestionTags.HasParentCompany))
                .ReturnsAsync("No");
            _applicationApplyApiClient.Setup(x => x.GetContactForApplication(_applicationId))
                .ReturnsAsync(new AssessorService.Domain.Entities.Contact {Email = _emailAddress});

            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section {ApplicationId = _applicationId, QnAData = new QnAData()});
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section {ApplicationId = _applicationId, QnAData = new QnAData()});

            var result = _controller.ViewApplication(_applicationId).GetAwaiter().GetResult();
            result.Should().BeAssignableTo<ViewResult>();

            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as RoatpFinancialApplicationViewModel;

            viewModel.ApplicantEmailAddress.Should().Be(_emailAddress);
        }
    }
}