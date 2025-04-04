using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class BatchSearchController : Controller
    {
        private readonly ILogger<BatchSearchController> _logger;
        private readonly IStaffSearchApiClient _staffSearchApiClient;
        private readonly ISessionService _sessionService;
        private readonly IMapper _mapper;

        public BatchSearchController(ILogger<BatchSearchController> logger, IStaffSearchApiClient apiClient, ISessionService sessionService, IMapper mapper)
        {
            _logger = logger;
            _staffSearchApiClient = apiClient;
            _sessionService = sessionService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? batchNumber = null, int page = 1)
        {
            var searchResults = await _staffSearchApiClient.BatchLog(page);
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
            var searchResponse = await _staffSearchApiClient.BatchSearch(batchNumber, page);

            var batchSearchViewModel = _mapper.Map<BatchSearchViewModel<StaffBatchSearchResultViewModel>>(searchResponse);
            batchSearchViewModel.Page = page;
            batchSearchViewModel.BatchNumber = batchNumber;

            return View(batchSearchViewModel);
        }
    }
}