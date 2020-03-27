using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.ExperienceAndAccreditation
{
    [TestFixture]
    public class SubcontractorDeclarationTests
    {
        private RoatpGatewayExperienceAndAccreditationController _controller;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IHttpContextAccessor> _contextAccessor;
        private Mock<IRoatpGatewayPageViewModelValidator> _gatewayValidator;
        private Mock<IGatewayExperienceAndAccreditationOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayExperienceAndAccreditationController>> _logger;

        private string username => "user name";
        private string givenName => "user";
        private string surname => "name";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _gatewayValidator = new Mock<IRoatpGatewayPageViewModelValidator>();
            _logger = new Mock<ILogger<RoatpGatewayExperienceAndAccreditationController>>();
            _orchestrator = new Mock<IGatewayExperienceAndAccreditationOrchestrator>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", username),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surname)
            }));

            var context = new DefaultHttpContext { User = user };
            _gatewayValidator.Setup(v => v.Validate(It.IsAny<LegalNamePageViewModel>()))
                .ReturnsAsync(new ValidationResponse
                    {
                        Errors = new List<ValidationErrorDetail>()
                    }
                );
            _contextAccessor.Setup(_ => _.HttpContext).Returns(context);

            _controller = new RoatpGatewayExperienceAndAccreditationController(_applyApiClient.Object, _contextAccessor.Object, _gatewayValidator.Object, _orchestrator.Object, _logger.Object);
        }

        [Test]
        public void subcontractor_declaration_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new SubcontractorDeclarationViewModel();
            
            _orchestrator.Setup(x => x.GetSubcontractorDeclarationViewModel(It.Is<GetSubcontractorDeclarationRequest>(y => y.ApplicationId == applicationId && y.UserName == username))).ReturnsAsync(expectedViewModel);
                
            var result = _controller.SubcontractorDeclaration(applicationId).Result;
            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public void subcontractor_contract_file_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedContractFile = new FileStreamResult(new MemoryStream(), "application/pdf");

            _orchestrator.Setup(x => x.GetSubcontractorDeclarationContractFile(It.Is<GetSubcontractorDeclarationContractFileRequest>(y => y.ApplicationId == applicationId))).ReturnsAsync(expectedContractFile);

            var result = _controller.SubcontractorDeclarationContractFile(applicationId).Result;
            Assert.AreSame(expectedContractFile, result);
        }
    }
}
