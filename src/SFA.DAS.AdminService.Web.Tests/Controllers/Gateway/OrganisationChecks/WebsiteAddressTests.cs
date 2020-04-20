using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.OrganisationChecks
{
    [TestFixture]
    public class WebsiteAddressTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;

        private Mock<ILogger<RoatpGatewayOrganisationChecksController>> _logger;

        private string username = "john smith";
        private string givenName = "john";
        private string surname = "smith";
        private string comment = "test comment";
        private string viewname = "~/Views/Roatp/Apply/Gateway/pages/Website.cshtml";


        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOrganisationChecksOrchestrator>();
             _controller = new RoatpGatewayOrganisationChecksController(ApplyApiClient.Object, ContextAccessor.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);

        }

        [Test]
        public void check_website_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            var vm = new WebsiteViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _orchestrator.Setup(x => x.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, username))).ReturnsAsync(vm);

            var result = _controller.GetWebsitePage(applicationId).Result;
            var viewResult = result as ViewResult;
            Assert.AreEqual(viewname, viewResult.ViewName);
        }

        [Test]
        public void post_website_address_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.WebsiteAddress;

            var vm = new WebsiteViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            ApplyApiClient.Setup(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, comment));

            var result = _controller.EvaluateWebsitePage(vm).Result;

            _orchestrator.Verify(x => x.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, username)), Times.Never());
        }

        [Test]
        public void post_website_address_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.WebsiteAddress;

            var vm = new WebsiteViewModel
            {
                ApplicationId = applicationId,
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            GatewayValidator.Setup(v => v.Validate(It.IsAny<WebsiteViewModel>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            _orchestrator.Setup(x => x.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, username)))
                .ReturnsAsync(vm)
                .Verifiable("view model not returned");

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, comment));

            var result = _controller.EvaluateWebsitePage(vm).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, comment), Times.Never);
            _orchestrator.Verify(x => x.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, username)), Times.Never());
        }
    }
}
