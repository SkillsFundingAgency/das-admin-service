using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Attributes;
using FluentAssertions;
using Moq;
using System;

namespace SFA.DAS.AdminService.Web.Tests.Attributes
{
    [TestFixture]
    public class ExternalApiExceptionFilterTests
    {
        private ActionContext _actionContext;

        [SetUp]
        public void Before_each_test()
        {
            var httpContext = new Mock<HttpContext>();
            var serviceProvider = new ExceptionFilterServiceProvider();
            httpContext.SetupGet(x => x.RequestServices).Returns(serviceProvider);

            _actionContext = new ActionContext
            {
                HttpContext = httpContext.Object,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
        }

        [Test]
        public void Filter_redirects_to_shutter_page_for_api_exceptions()
        {
            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new ExternalApiException("Error calling API"),
                Result = new Mock<IActionResult>().Object
            };

            var filter = new ExternalApiExceptionFilter();
            filter.OnException(exceptionContext);

            var redirectToActionResult = exceptionContext.Result as RedirectToActionResult;
            redirectToActionResult.Should().NotBeNull();
            redirectToActionResult.ActionName.Should().Be("ExternalApisUnavailable");
            redirectToActionResult.ControllerName.Should().Be("RoatpShutterPage");
        }

        [Test]
        public void Filter_does_not_redirect_for_other_exception_types()
        {
            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new NotImplementedException("Exception that we are not trappig"),
                Result = new Mock<IActionResult>().Object
            };

            var filter = new ExternalApiExceptionFilter();
            filter.OnException(exceptionContext);

            var redirectToActionResult = exceptionContext.Result as RedirectToActionResult;
            redirectToActionResult.Should().BeNull();
        }
    }
}
