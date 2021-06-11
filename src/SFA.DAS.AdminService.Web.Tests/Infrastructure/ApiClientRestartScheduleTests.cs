using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Infrastructure
{
    public class ApiClientRestartScheduleTests
    {
        private ApiClient _sut;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;

        [SetUp]
        public void Arrange()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("http://test/");

            _sut = new ApiClient(httpClient, Mock.Of<ITokenService>());
        }


        [Test]
        public async Task ThenLastRunStatusRequestIsPosted()
        {
            //setup
            _mockHttpMessageHandler
             .Protected()
             .Setup<Task<HttpResponseMessage>>(
             "SendAsync",
             ItExpr.IsAny<HttpRequestMessage>(),
             ItExpr.IsAny<CancellationToken>()
             )
             .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("") })
             .Verifiable();

            //act
            await _sut.RestartSchedule(Guid.NewGuid());

            //assert
            _mockHttpMessageHandler
             .Protected()
             .Verify("SendAsync",
             Times.Exactly(1),
             ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.RequestUri.AbsolutePath == $"/api/v1/schedule/updatelaststatus"),
             ItExpr.IsAny<CancellationToken>());
        }
    }
}
