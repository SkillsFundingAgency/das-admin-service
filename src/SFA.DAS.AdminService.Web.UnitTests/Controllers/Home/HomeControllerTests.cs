using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Models;
using System.Security.Claims;
using FluentAssertions.Execution;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private Mock<IWebConfiguration> _mockWebConfiguration;

        [SetUp]
        public void Setup()
        {
            _mockWebConfiguration = new Mock<IWebConfiguration>();
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
            _controller = new HomeController(_mockWebConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
            };

            //act
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

        [Test, MoqAutoData]
        public void When_InvalidRole_Then_ViewIsReturned(string helpLink, bool useDfESignIn)
        {
            //arrange
            _mockWebConfiguration.Setup(x => x.UseDfESignIn).Returns(useDfESignIn);
            _mockWebConfiguration.Setup(x => x.DfESignInServiceHelpUrl).Returns(helpLink);
            _controller = new HomeController(_mockWebConfiguration.Object);

            //act
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
