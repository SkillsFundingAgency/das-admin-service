using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Infrastructure;
using System.IdentityModel.Tokens.Jwt;

namespace SFA.DAS.AdminService.Web.Tests.Infrastructure
{
    public class RoatpTokenServiceTests
    {
        [Test]
        public void GetToken_Returns_Valid_Jwt_Token()
        {
            const string resource = @"https://citizenazuresfabisgov.onmicrosoft.com/das-at-roatpapi-as-ar";
            var sut = CreateSut(resource);

            var token = sut.GetToken();
            var tokenHandler = new JwtSecurityTokenHandler();

            Assert.That(tokenHandler.CanReadToken(token), Is.True);
        }

        private static RoatpTokenService CreateSut(string resource)
        {
            var webConfiguration = new Mock<IWebConfiguration>();

            var authentication = new ClientApiAuthentication
            {
                ResourceId = resource
            };

            webConfiguration.SetupGet(x => x.RoatpApiAuthentication).Returns(authentication);
            var hostEnvironment = new Mock<IHostEnvironment>();

            return new RoatpTokenService(webConfiguration.Object, hostEnvironment.Object);
        }
    }
}
