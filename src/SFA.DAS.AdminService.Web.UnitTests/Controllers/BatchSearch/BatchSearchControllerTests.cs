using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.BatchSearch
{
    [TestFixture]
    public class BatchSearchControllerTests
    {
        private BatchSearchController _controller;
        private PaginatedList<StaffBatchLogResult> _staffBatchLogResult;
        private StaffBatchSearchResponse _staffBatchSearchResponse;

        [SetUp]
        public void Setup()
        {
            _staffBatchLogResult = SetupStaffBatchLogResult();
            _staffBatchSearchResponse = SetUpBatchSearchResponse();
            StaffSearchApiClient staffSearchApiClient = SetUpApiClient();
            _controller = new BatchSearchController(Mock.Of<ILogger<BatchSearchController>>(), staffSearchApiClient, Mock.Of<ISessionService>());
        }

        [Test]
        public async Task batch_log_view_model_is_correctly_populated_from_staff_batch_log_result()
        {
            var result = await _controller.Index() as ViewResult;

            var viewModel = result.Model as BatchSearchViewModel<StaffBatchLogResult>;

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.BatchNumber, Is.Null);
                Assert.That(viewModel.Page, Is.EqualTo(1));
                Assert.That(viewModel.PaginatedList.Items.Count, Is.EqualTo(_staffBatchSearchResponse.Results.Items.Count));
                viewModel.PaginatedList.Items.ForEach(x => VerifyStaffBatchLogResult(_staffBatchLogResult.Items.First(y => y.BatchNumber == x.BatchNumber), x));
            });
        }

        [Test]
        public async Task staff_batch_result_view_model_is_correctly_populated_from_staff_batch_search_result()
        {
            var result = await _controller.Results(1, 1) as ViewResult;

            var viewModel = result.Model as BatchSearchViewModel<StaffBatchSearchResult>;

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.BatchNumber, Is.EqualTo(1));
                Assert.That(viewModel.Page, Is.EqualTo(1));
                Assert.That(viewModel.BatchNumber, Is.EqualTo(1));
                Assert.That(viewModel.PaginatedList.Items.Count, Is.EqualTo(_staffBatchSearchResponse.Results.Items.Count));
                viewModel.PaginatedList.Items.ForEach(x => Verify(_staffBatchSearchResponse.Results.Items.First(y => y.BatchNumber == x.BatchNumber), x));
            });
        }

        private StaffSearchApiClient SetUpApiClient()
        {
            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            var clientFactory = new Mock<IAssessorApiClientFactory>();
            clientFactory.Setup(x => x.CreateHttpClient()).Returns(client);

            mockHttp.When($"/api/v1/staffsearch/batchlog?page=1")
              .Respond("application/json", JsonConvert.SerializeObject(_staffBatchLogResult));

            mockHttp.When($"/api/v1/staffsearch/batch?batchNumber=1&page=1")
               .Respond("application/json", JsonConvert.SerializeObject(_staffBatchSearchResponse));

            var apiClient = new StaffSearchApiClient(clientFactory.Object, Mock.Of<ILogger<StaffSearchApiClient>>());
            return apiClient;
        }

        private static StaffBatchSearchResponse SetUpBatchSearchResponse()
        {
            int batchNumberGenerator = 1;
            var staffBatchSearchResult = Builder<StaffBatchSearchResult>.CreateListOfSize(4)
                .All()
                .With(x => x.BatchNumber = batchNumberGenerator++)
                .Build()
                .ToList();

            return new StaffBatchSearchResponse()
            {
                PrintedDate = DateTime.Now,
                SentToPrinterDate = DateTime.Now,
                Results = new PaginatedList<StaffBatchSearchResult>(staffBatchSearchResult, staffBatchSearchResult.Count, 1, 10, 10)
            };
        }

        private static PaginatedList<StaffBatchLogResult> SetupStaffBatchLogResult()
        {
            int batchNumberGenerator = 1;
            var staffBatchLogResult = Builder<StaffBatchLogResult>.CreateListOfSize(4)
                .All()
                .With(x => x.BatchNumber = batchNumberGenerator++)
                .Build()
                .ToList();
            return new PaginatedList<StaffBatchLogResult>(staffBatchLogResult, staffBatchLogResult.Count, 1, 10, 10);
        }

        private static void VerifyStaffBatchLogResult(StaffBatchLogResult expectedStaffBatchLogResult, StaffBatchLogResult actualStaffBatchLogResult)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actualStaffBatchLogResult.BatchNumber, Is.EqualTo(expectedStaffBatchLogResult.BatchNumber));
                Assert.That(actualStaffBatchLogResult.NumberOfCertificatesPrinted, Is.EqualTo(expectedStaffBatchLogResult.NumberOfCertificatesPrinted));
                Assert.That(actualStaffBatchLogResult.NumberOfCertificatesSent, Is.EqualTo(expectedStaffBatchLogResult.NumberOfCertificatesSent));
                Assert.That(actualStaffBatchLogResult.PrintedDate, Is.EqualTo(expectedStaffBatchLogResult.PrintedDate));
                Assert.That(actualStaffBatchLogResult.ScheduledDate, Is.EqualTo(expectedStaffBatchLogResult.ScheduledDate));
                Assert.That(actualStaffBatchLogResult.SentToPrinterDate, Is.EqualTo(expectedStaffBatchLogResult.SentToPrinterDate));
            });
        }

        private static void Verify(StaffBatchSearchResult expectedStaffBatchLogResult, StaffBatchSearchResult actualStaffBatchLogResult)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actualStaffBatchLogResult.BatchNumber, Is.EqualTo(expectedStaffBatchLogResult.BatchNumber));
                Assert.That(actualStaffBatchLogResult.StatusAt, Is.EqualTo(expectedStaffBatchLogResult.StatusAt));
                Assert.That(actualStaffBatchLogResult.CertificateReference, Is.EqualTo(expectedStaffBatchLogResult.CertificateReference));
                Assert.That(actualStaffBatchLogResult.Uln, Is.EqualTo(expectedStaffBatchLogResult.Uln));
                Assert.That(actualStaffBatchLogResult.StandardCode, Is.EqualTo(expectedStaffBatchLogResult.StandardCode));
            });
        }
    }
}
