using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Api.Client;
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
            ApiClient apiClient = SetUpApiClient();
            _controller = new BatchSearchController(Mock.Of<ILogger<BatchSearchController>>(), apiClient, Mock.Of<ISessionService>());
        }

        [Test]
        public async Task batch_log_view_model_is_correctly_populated_from_staff_batch_log_result()
        {
            var result = await _controller.Index() as ViewResult;

            var viewModel = result.Model as BatchSearchViewModel<StaffBatchLogResult>;
        
            Assert.IsNull(viewModel.BatchNumber);
            Assert.AreEqual(1, viewModel.Page);
            Assert.AreEqual(_staffBatchSearchResponse.Results.Items.Count, viewModel.PaginatedList.Items.Count);
            viewModel.PaginatedList.Items.ForEach(x => VerifyStaffBatchLogResult(_staffBatchLogResult.Items.First(y => y.BatchNumber == x.BatchNumber), x));
        }

        [Test]
        public async Task staff_batch_result_view_model_is_correctly_populated_from_staff_batch_search_result()
        {
            var result = await _controller.Results(1,1) as ViewResult;

            var viewModel = result.Model as BatchSearchViewModel<StaffBatchSearchResult>;

            Assert.AreEqual(1, viewModel.BatchNumber);
            Assert.AreEqual(1, viewModel.Page);
            Assert.AreEqual(1, viewModel.BatchNumber);
            Assert.AreEqual(_staffBatchSearchResponse.Results.Items.Count, viewModel.PaginatedList.Items.Count);
            viewModel.PaginatedList.Items.ForEach(x => Verify(_staffBatchSearchResponse.Results.Items.First(y => y.BatchNumber == x.BatchNumber), x));
        }

        private ApiClient SetUpApiClient()
        {
            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            mockHttp.When($"/api/v1/staffsearch/batchlog?page=1")
              .Respond("application/json", JsonSerializer.Serialize(_staffBatchLogResult));

            mockHttp.When($"/api/v1/staffsearch/batch?batchNumber=1&page=1")
               .Respond("application/json", JsonSerializer.Serialize(_staffBatchSearchResponse));

            var apiClient = new ApiClient(client, Mock.Of<ITokenService>());
            return apiClient;
        }

        private StaffBatchSearchResponse SetUpBatchSearchResponse()
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

        private PaginatedList<StaffBatchLogResult> SetupStaffBatchLogResult()
        {
            int batchNumberGenerator = 1;
            var staffBatchLogResult = Builder<StaffBatchLogResult>.CreateListOfSize(4)
                .All()
                .With(x => x.BatchNumber = batchNumberGenerator++)
                .Build()
                .ToList();
            return new PaginatedList<StaffBatchLogResult>(staffBatchLogResult, staffBatchLogResult.Count, 1, 10, 10);
        }

        private void VerifyStaffBatchLogResult(StaffBatchLogResult expectedStaffBatchLogResult, StaffBatchLogResult actualStaffBatchLogResult)
        {
            Assert.AreEqual(expectedStaffBatchLogResult.BatchNumber, actualStaffBatchLogResult.BatchNumber);
            Assert.AreEqual(expectedStaffBatchLogResult.NumberOfCertificatesPrinted, actualStaffBatchLogResult.NumberOfCertificatesPrinted);
            Assert.AreEqual(expectedStaffBatchLogResult.NumberOfCertificatesSent, actualStaffBatchLogResult.NumberOfCertificatesSent);
            Assert.AreEqual(expectedStaffBatchLogResult.PrintedDate, actualStaffBatchLogResult.PrintedDate);
            Assert.AreEqual(expectedStaffBatchLogResult.ScheduledDate, actualStaffBatchLogResult.ScheduledDate);
            Assert.AreEqual(expectedStaffBatchLogResult.SentToPrinterDate, actualStaffBatchLogResult.SentToPrinterDate);
        }

        private void Verify(StaffBatchSearchResult expectedStaffBatchLogResult, StaffBatchSearchResult actualStaffBatchLogResult)
        {
            Assert.AreEqual(expectedStaffBatchLogResult.BatchNumber, actualStaffBatchLogResult.BatchNumber);
            Assert.AreEqual(expectedStaffBatchLogResult.StatusAt, actualStaffBatchLogResult.StatusAt);
            Assert.AreEqual(expectedStaffBatchLogResult.CertificateReference, actualStaffBatchLogResult.CertificateReference);
            Assert.AreEqual(expectedStaffBatchLogResult.Uln, actualStaffBatchLogResult.Uln);
            Assert.AreEqual(expectedStaffBatchLogResult.StandardCode, actualStaffBatchLogResult.StandardCode);
        }
    }
}
