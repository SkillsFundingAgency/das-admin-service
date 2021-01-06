using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Models.Roatp;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using OfficeOpenXml;
    using SFA.DAS.AdminService.Web.Domain;
    using SFA.DAS.AdminService.Web.Helpers;
    using SFA.DAS.AdminService.Web.Infrastructure;

    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class DownloadRoatpController : Controller
    {
        private IRoatpApiClient _apiClient;
        private IDataTableHelper _dataTableHelper;
        private ILogger<DownloadRoatpController> _logger;
        private ICsvExportService _csvExportService;
        private readonly IRoatpApplicationApiClient _applyApiClient;

        private const string CompleteRegisterWorksheetName = "Providers";
        private const string AuditHistoryWorksheetName = "Provider history";
        private const string ExcelFileName = "_RegisterOfApprenticeshipTrainingProviders.xlsx";
        private const string FatFileName = "roatp {0}.xlsx";
        private const string FatWorksheetName = "RoATP";

        public DownloadRoatpController(IRoatpApiClient apiClient, IDataTableHelper dataTableHelper,
            ILogger<DownloadRoatpController> logger, ICsvExportService csvExportService, IRoatpApplicationApiClient applyApiClient)
        {
            _apiClient = apiClient;
            _dataTableHelper = dataTableHelper;
            _logger = logger;
            _csvExportService = csvExportService;
            _applyApiClient = applyApiClient;
        }

        [Route("download-register")]
        public async Task<IActionResult> Download()
        {
            using (var package = new ExcelPackage())
            {
                var completeRegisterWorkSheet = package.Workbook.Worksheets.Add(CompleteRegisterWorksheetName);
                var registerData = await _apiClient.GetCompleteRegister();
                if (registerData != null && registerData.Any())
                {
                    completeRegisterWorkSheet.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(registerData), true);
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

                return File(package.GetAsByteArray(), "application/excel",
                    $"{DateTime.Now.ToString("yyyyMMdd")}{ExcelFileName}");
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

            var exportModel = Mapper.Map<List<RoatpOversightOutcomeExportViewModel>>(applications);

            var bytearray = _csvExportService
                .WriteCsvToByteArray<RoatpOversightOutcomeExportViewModel, RoatpOversightOutcomeExportCsvMap>(exportModel);

            var fileName = $"current_applications_{DateTime.UtcNow:ddMMyy}.csv";

            return File(bytearray, "text/csv", fileName);
        }
    }
}
