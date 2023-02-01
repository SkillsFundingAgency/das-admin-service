using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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

            var tokenAudience = new JwtSecurityTokenHandler().ReadJwtToken(token)
                                                             .Claims
                                                             .First()  // "aud" or audience - who or what the token is intended for
                                                             .Value;

            Assert.That(tokenAudience, Is.EqualTo(roAtpApiV1Resource));
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
