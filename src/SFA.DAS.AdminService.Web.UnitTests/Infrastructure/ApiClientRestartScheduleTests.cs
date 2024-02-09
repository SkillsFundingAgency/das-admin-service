using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
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
            
            var client = new HttpClient(_mockHttpMessageHandler.Object);
            client.BaseAddress = new Uri("http://test/");

            var clientFactory = new Mock<IAssessorApiClientFactory>();
            clientFactory.Setup(x => x.CreateHttpClient())
                .Returns(client);

            _sut = new ScheduleApiClient(clientFactory.Object, Mock.Of<ILogger<ScheduleApiClient>>());
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
