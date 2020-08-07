﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.ExperienceAndAccreditation
{
    [TestFixture]
    public class OfstedDetailsTests : RoatpGatewayControllerTestBase<RoatpGatewayExperienceAndAccreditationController>
    {
        private RoatpGatewayExperienceAndAccreditationController _controller;
        private Mock<IGatewayExperienceAndAccreditationOrchestrator> _orchestrator;
        
        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayExperienceAndAccreditationOrchestrator>();
            _controller = new RoatpGatewayExperienceAndAccreditationController(ContextAccessor.Object, ApplyApiClient.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);
        }

        [Test]
        public async Task ofsted_details_are_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new OfstedDetailsViewModel();
            
            _orchestrator.Setup(x => x.GetOfstedDetailsViewModel(It.Is<GetOfstedDetailsRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModel);
                
            var result = await _controller.OfstedDetails(applicationId);
            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task saving_ofsted_details_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Ofsted;

            var vm = new OfstedDetailsViewModel {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = "Some pass text"
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            GatewayValidator.Setup(v => v.Validate(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateOfstedDetailsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, vm.OptionPassText));
        }

        [Test]
        public async Task saving_ofsted_details_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Ofsted;

            var vm = new OfstedDetailsViewModel()
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ApplicationId = applicationId,
                PageId = pageId
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            GatewayValidator.Setup(v => v.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            _orchestrator.Setup(x => x.GetOfstedDetailsViewModel(It.Is<GetOfstedDetailsRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.EvaluateOfstedDetailsPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
