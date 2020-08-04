using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class BatchSearchController : Controller
    {
        private readonly ILogger<BatchSearchController> _logger;
        private readonly ApiClient _apiClient;
        private readonly ISessionService _sessionService;

        public BatchSearchController(ILogger<BatchSearchController> logger, ApiClient apiClient, ISessionService sessionService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? batchNumber = null, int page = 1)
        {
            var searchResults = await _apiClient.BatchLog(page);
            var batchLogViewModel = new BatchSearchViewModel<StaffBatchLogResult>
            {
                PaginatedList = searchResults,
                BatchNumber = batchNumber,
                Page = page
            };

            return View(batchLogViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Results(int batchNumber, int page = 1)
        {
            var searchResponse = await _apiClient.BatchSearch(batchNumber, page);
            var batchSearchViewModel = new BatchSearchViewModel<StaffBatchSearchResult>
            {
                SentToPrinterDate = searchResponse.SentToPrinterDate,
                PrintedDate = searchResponse.PrintedDate,
                PaginatedList = searchResponse.Results,
                BatchNumber = batchNumber,
                Page = page
            };

            return View(batchSearchViewModel);
        }
    }
}