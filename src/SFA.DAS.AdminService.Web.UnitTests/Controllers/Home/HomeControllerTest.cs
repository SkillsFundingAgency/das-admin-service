using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Models;
using System.Security.Claims;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class HomeControllerTest
    {
        private HomeController _controller;
        private Mock<IWebConfiguration> _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new Mock<IWebConfiguration>();
        }


        [TestCase(true)]
        [TestCase(false)]
        public void Then_Index_Returns_ViewModel(bool useDfESignIn)
        {
            //arrange
            _configuration.Setup(c => c.UseDfESignIn).Returns(useDfESignIn);
            _controller = new HomeController(_configuration.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() },
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
            _configuration.Setup(c => c.UseDfESignIn).Returns(true);
            var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "somename")
            }, "mock"));
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.User).Returns(authorizedUser);
            var controller = new HomeController(_configuration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
            };

            //sut
            var result = controller.Index();

            //assert
            result.Should().NotBeNull();
            var actualResult = (RedirectToActionResult)result;
            actualResult.ControllerName.Should().Be("Dashboard");
            actualResult.ActionName.Should().Be("Index");
        }
    }
}
