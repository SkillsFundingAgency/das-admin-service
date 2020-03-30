using Moq;
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

            _orchestrator.Setup(x => x.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, Username)))
                .ReturnsAsync(new WebsiteViewModel())
                .Verifiable("view model not returned");

            var _result = _controller.GetWebsitePage(applicationId).Result;
            _orchestrator.Verify(x => x.GetWebsiteViewModel(It.IsAny<GetWebsiteRequest>()), Times.Once());
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

            ApplyApiClient.Setup(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, It.IsAny<string>()));

            var result = _controller.EvaluateWebsitePage(vm).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _orchestrator.Verify(x => x.GetWebsiteViewModel(It.IsAny<GetWebsiteRequest>()), Times.Never());
        }

        [Test]
        public void post_website_address_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.WebsiteAddress;

            var vm = new WebsiteViewModel
            {
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

            vm.ApplicationId = applicationId;
            vm.PageId = vm.PageId;
            vm.SourcesCheckedOn = DateTime.Now;

            _orchestrator.Setup(x => x.GetWebsiteViewModel(It.IsAny<GetWebsiteRequest>()))
                .ReturnsAsync(vm)
                .Verifiable("view model not returned");

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, It.IsAny<string>()));

            var result = _controller.EvaluateWebsitePage(vm).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _orchestrator.Verify(x => x.GetWebsiteViewModel(It.IsAny<GetWebsiteRequest>()), Times.Never());
        }

    }
}
