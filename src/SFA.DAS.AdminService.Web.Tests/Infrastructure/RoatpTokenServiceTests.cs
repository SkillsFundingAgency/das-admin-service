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
        private const string roAtpApiV1Resource = @"https://citizenazuresfabisgov.onmicrosoft.com/das-at-roatpapi-as-ar";

        [Test]
        public void GetToken_Returns_Valid_Jwt_Token()
        {
            var sut = CreateSut();

            var token = sut.GetToken();

            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.That(tokenHandler.CanReadToken(token), Is.True);
        }

        private static RoatpTokenService CreateSut()
        {
            var webConfiguration = new Mock<IWebConfiguration>();
            var authentication = new ClientApiAuthentication
            {
                ResourceId = roAtpApiV1Resource
            };
            webConfiguration.SetupGet(x => x.RoatpApiAuthentication).Returns(authentication);

            return new RoatpTokenService(webConfiguration.Object, new Mock<IHostEnvironment>().Object);
        }
    }
}
