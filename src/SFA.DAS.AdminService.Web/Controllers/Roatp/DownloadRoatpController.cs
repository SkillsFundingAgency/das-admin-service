using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Models.Roatp;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    [Route("")]
    public class DownloadRoatpController : Controller
    {
        private readonly IRoatpApiClient _apiClient;
        private readonly IDataTableHelper _dataTableHelper;
        private readonly ILogger<DownloadRoatpController> _logger;
        private readonly ICsvExportService _csvExportService;
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IMapper _mapper;

        private const string CompleteRegisterWorksheetName = "Providers";
        private const string AuditHistoryWorksheetName = "Provider history";
        private const string ExcelFileName = "_RegisterOfApprenticeshipTrainingProviders.xlsx";

        public DownloadRoatpController(IRoatpApiClient apiClient, IDataTableHelper dataTableHelper,
            ILogger<DownloadRoatpController> logger, ICsvExportService csvExportService, IRoatpApplicationApiClient applyApiClient, IMapper mapper)
        {
            ExcelPackage.License.SetNonCommercialOrganization("Department for Education");
            _apiClient = apiClient;
            _dataTableHelper = dataTableHelper;
            _logger = logger;
            _csvExportService = csvExportService;
            _applyApiClient = applyApiClient;
            _mapper = mapper;
        }

        [Route("download-register")]
        public async Task<IActionResult> Download()
        {
            using (var package = new ExcelPackage())
            {
                var completeRegisterWorkSheet = package.Workbook.Worksheets.Add(CompleteRegisterWorksheetName);
                GetAllOrganisationsResponse registerData = await _apiClient.GetCompleteRegister();
                if (registerData != null && registerData.Organisations.Any())
                {
                    IEnumerable<ProviderRegisterModel> items = registerData.Organisations.Select(r => (ProviderRegisterModel)r).OrderBy(r => r.LegalName);
                    completeRegisterWorkSheet.Cells.LoadFromCollection(items, true);
                }
                else
                {
                    _logger.LogError("Unable to retrieve register data from RoATP API");
                }

                var auditHistoryWorksheet = package.Workbook.Worksheets.Add(AuditHistoryWorksheetName);
                var auditHistoryData = await _apiClient.GetAuditHistory();
                if (auditHistoryData != null && auditHistoryData.Any())
                {
                    auditHistoryWorksheet.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(auditHistoryData), true);
                }
                else
                {
                    _logger.LogError("Unable to retrieve audit history data from RoATP API");
                }
                byte[] fileContent = await package.GetAsByteArrayAsync();
                return File(fileContent, "application/excel", $"{DateTime.Now.ToString("yyyyMMdd")}{ExcelFileName}");
            }
        }

        [Route("application-download")]
        public IActionResult ApplicationDownload()
        {
            return View("~/Views/Roatp/ApplicationDownload.cshtml");
        }

        [Route("application-download/download")]
        public async Task<IActionResult> ApplicationDownloadCsv(ApplicationDownloadViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/ApplicationDownload.cshtml");
            }

            var applications = await _applyApiClient.GetApplicationOversightDetailsForDownload(viewModel.FromDate.Value, viewModel.ToDate.Value);

            var exportModel = _mapper.Map<List<RoatpOversightOutcomeExportViewModel>>(applications);

            var bytearray = _csvExportService
                .WriteCsvToByteArray<RoatpOversightOutcomeExportViewModel, RoatpOversightOutcomeExportCsvMap>(exportModel);

            var fileName = $"outcomes_{viewModel.FromDate:ddMMyy}_to_{viewModel.ToDate.Value:ddMMyy}.csv";

            return File(bytearray, "text/csv", fileName);
        }
    }
}
