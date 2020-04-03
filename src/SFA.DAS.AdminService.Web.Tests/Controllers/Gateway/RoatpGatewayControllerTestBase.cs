using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway
{
    public abstract class RoatpGatewayControllerTestBase<T>
    {
        protected Mock<IRoatpApplicationApiClient> ApplyApiClient;
        protected Mock<IHttpContextAccessor> ContextAccessor;
        protected Mock<IRoatpGatewayPageViewModelValidator> GatewayValidator;
        protected Mock<ILogger<T>> Logger;

        protected string Username => "user name";
        protected string GivenName => "user";
        protected string Surname => "name";


        protected void CoreSetup()
        {
            ApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            ContextAccessor = new Mock<IHttpContextAccessor>();
            GatewayValidator = new Mock<IRoatpGatewayPageViewModelValidator>();
            Logger = new Mock<ILogger<T>>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", Username),
                new Claim(ClaimTypes.GivenName, GivenName),
                new Claim(ClaimTypes.Surname, Surname)
            }));

            var context = new DefaultHttpContext { User = user };
            GatewayValidator.Setup(v => v.Validate(It.IsAny<RoatpGatewayPageViewModel>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                }
                );
            ContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        }
    }
}
