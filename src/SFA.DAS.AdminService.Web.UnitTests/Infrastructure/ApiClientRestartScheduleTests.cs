using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Infrastructure
{
    public class ApiClientRestartScheduleTests
    {
        private ScheduleApiClient _sut;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;

        [SetUp]
        public void Arrange()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("http://test/");

            _sut = new ScheduleApiClient(httpClient, Mock.Of<IAssessorTokenService>(), Mock.Of<ILogger<ApiClientBase>>());
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
