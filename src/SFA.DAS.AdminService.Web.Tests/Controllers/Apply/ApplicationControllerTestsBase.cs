using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Apply;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Apply
{
    public class ApplicationControllerTestsBase
    {
        protected ApplicationRefactoredController _controller;

        protected Mock<IHttpContextAccessor> _httpContextAccessor;
        protected Mock<ILogger<ApplicationController>> _logger;
        protected Mock<IMediator> _mediator;

        protected Fixture _fixture;

        [SetUp]
        public void BaseSetup()
        {
            _fixture = new Fixture();

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _logger = new Mock<ILogger<ApplicationController>>();
            _mediator = new Mock<IMediator>();

            var identity = new GenericIdentity("test user");
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "JOHN"));
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "DUNHILL"));

            _httpContextAccessor.Setup(a => a.HttpContext.User.Identities)
               .Returns(new List<ClaimsIdentity>() { identity });

            _controller = new ApplicationRefactoredController(_httpContextAccessor.Object, _mediator.Object);
        }

    }
}
