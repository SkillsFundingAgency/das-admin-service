using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Models;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class HomeControllerTest
    {
        private HomeController _controller;
        private Mock<IWebConfiguration> _mockWebConfiguration;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            _mockWebConfiguration = new Mock<IWebConfiguration>();
            _mockConfiguration = new Mock<IConfiguration>();
        }


        [TestCase(true)]
        [TestCase(false)]
        public void Then_Index_Returns_ViewModel(bool useDfESignIn)
        {
            //arrange
            _mockWebConfiguration.Setup(c => c.UseDfESignIn).Returns(useDfESignIn);
            _controller = new HomeController(_mockWebConfiguration.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
            };

            //sut
            var result = _controller.Index();

            //assert
            result.Should().NotBeNull();
            var resultModel = result.Should().BeOfType<ViewResult>().
                Which.Model.Should().BeOfType<HomeViewModel>().Which;
            resultModel.UseDfESignIn.Should().Be(useDfESignIn);
        }


        [Test]
        public void When_DfESignIn_True_And_User_Authenticated_Then_Index_Returns_Redirect()
        {
            //arrange
            _mockWebConfiguration.Setup(c => c.UseDfESignIn).Returns(true);
            var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "somename")
            }, "mock"));
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.User).Returns(authorizedUser);
            _controller = new HomeController(_mockWebConfiguration.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
            };

            //sut
            var result = _controller.Index();

            //assert
           using (new AssertionScope())
           {
                result.Should().NotBeNull();
                var actualResult = (RedirectToActionResult)result;
                actualResult.ControllerName.Should().Be("Dashboard");
                actualResult.ActionName.Should().Be("Index");
           }
        }

        [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
        [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
        [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        public void When_InvalidRole_Then_ViewIsReturned(string env, string helpLink, bool useDfESignIn)
        {
            //arrange
            _mockWebConfiguration.Setup(x => x.UseDfESignIn).Returns(useDfESignIn);
            _mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
            _controller = new HomeController(_mockWebConfiguration.Object, _mockConfiguration.Object);

            //sut
            var result = (ViewResult)_controller.InvalidRole();

            //assert
           Assert.Multiple(() => 
           { 
               Assert.That(result, Is.Not.Null);

               var actualModel = result.Model as Error403ViewModel;
               Assert.That(actualModel?.HelpPageLink, Is.EqualTo(helpLink));
               Assert.That(actualModel?.UseDfESignIn, Is.EqualTo(useDfESignIn));
           });
        }
    }
}
