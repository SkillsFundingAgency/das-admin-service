using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Domain.Consts;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ApiClient _apiClient;
        private readonly ISessionService _sessionService;

        public SearchController(ILogger<SearchController> logger, ApiClient apiClient, ISessionService sessionService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("results")]
        public async Task<IActionResult> Results(string searchString, int page = 1)
        {
            EpaOrganisation org=null;
            var searchResults = await _apiClient.Search(searchString, page);

            if(!string.IsNullOrEmpty(searchResults?.EndpointAssessorOrganisationId))
                org = await _apiClient.GetEpaOrganisation(searchResults.EndpointAssessorOrganisationId);

            var searchViewModel = new SearchViewModel
            {
                OrganisationName = org?.Name??string.Empty,
                StaffSearchResult = searchResults,
                SearchString = searchString,
                Page = page
            };

            return View(searchViewModel);
        }

        [HttpGet("select")]
        public async Task<IActionResult> Select(int stdCode,
            long uln,
            string searchString,
            int page = 1,
            bool allLogs = false,
            int? batchNumber = null)
        {
            var learner = await _apiClient.GetLearner(stdCode, uln, allLogs);

            var vm = new LearnerDetailViewModel
            {
                Learner = learner,
                SearchString = searchString,
                Page = page,
                ShowDetail = !allLogs,
                BatchNumber = batchNumber
            };

            return View(vm);
        }
    }

    public class LearnerDetailViewModel
    {
        public LearnerDetail Learner { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public bool ShowDetail { get; set; }
        public int? BatchNumber { get; set; }

        public bool CanRequestDuplicate => CertificateStatus.CanRequestDuplicateCertificate(Learner.CertificateStatus);
        public bool CanAmendCertificate => CertificateStatus.CanAmendCertificate(Learner.CertificateStatus);
        public bool CanDeleteCertificate => Learner.CertificateStatus != CertificateStatus.Deleted;
    }

    public class SearchViewModel
    {
        public string SearchString { get; set; }
        public string OrganisationName { get; set; }
        public int Page { get; set; }
        public StaffSearchResult StaffSearchResult { get; set; }
    }
}