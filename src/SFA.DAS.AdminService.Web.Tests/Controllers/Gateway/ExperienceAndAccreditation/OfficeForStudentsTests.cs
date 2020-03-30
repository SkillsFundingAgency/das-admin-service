using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class OfficeForStudentsTests
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
        public async Task office_for_students_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new OfficeForStudentsViewModel();
            
            _orchestrator.Setup(x => x.GetOfficeForStudentsViewModel(It.Is<GetOfficeForStudentsRequest>(y => y.ApplicationId == applicationId && y.UserName == username))).ReturnsAsync(expectedViewModel);
                
            var result = await _controller.OfficeForStudents(applicationId);
            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task saving_office_for_students_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OfficeForStudents;

            var vm = new OfficeForStudentsViewModel {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = "Some pass text"
            };

            _gatewayValidator.Setup(v => v.Validate(vm)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateOfficeForStudentsPage(vm);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, vm.OptionPassText));
        }

        [Test]
        public async Task saving_office_for_students_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OfficeForStudents;

            var vm = new OfficeForStudentsViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ApplicationId = applicationId,
                PageId = pageId
            };

            _gatewayValidator.Setup(v => v.Validate(vm))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            await _controller.EvaluateOfficeForStudentsPage(vm);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
