using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Roles.OperationsTeam + "," + Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam + "," + Roles.EpaoReportsOnlyTeam)]
    public class ReportsController : ExcelAwareController
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IStaffReportsApiClient _staffReportsApiClient;
        private readonly IDataTableHelper _dataTableHelper;

        public ReportsController(ILogger<ReportsController> logger, IStaffReportsApiClient staffReportsApiClient, IDataTableHelper dataTableHelper)
        {
            _logger = logger;
            _staffReportsApiClient = staffReportsApiClient;
            _dataTableHelper = dataTableHelper;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _staffReportsApiClient.GetReportList();

            var vm = new ReportViewModel {Reports = reports};

            return View(vm);
        }

        public async Task<IActionResult> Run(Guid reportId)
        {
            if (!ModelState.IsValid || reportId == Guid.Empty)
            {
                return RedirectToAction("Index");
            }

            var reports = await _staffReportsApiClient.GetReportList();
            var reportType = await _staffReportsApiClient.GetReportTypeFromId(reportId);

            if (reportType == ReportType.Download)
                return RedirectToAction("DirectDownload", new {reportId});


            var data = await _staffReportsApiClient.GetReport(reportId);
            var vm = new ReportViewModel {Reports = reports, ReportId = reportId, SelectedReportData = data};
            return View(vm);

        }

        public async Task<FileContentResult> DirectDownload(Guid reportId)
        {
            var reportDetails = await _staffReportsApiClient.GetReportDetailsFromId(reportId);

            using (var package = new ExcelPackage())
            {

                    foreach (var ws in reportDetails.Worksheets.OrderBy(w => w.Order))
                    {
                        var worksheetToAdd = package.Workbook.Worksheets.Add(ws.Worksheet);
                        var data = await _staffReportsApiClient.GetDataFromStoredProcedure(ws.StoredProcedure);
                        var dataTable = _dataTableHelper.ToDataTable(data);
                        if (dataTable.Rows.Count > 0)
                        {
                            worksheetToAdd.Cells.LoadFromDataTable(dataTable, true);
                        }
                    }

                    return File(package.GetAsByteArray(), "application/excel", $"{reportDetails.Name}.xlsx");
              
            }
        }
    
        public async Task<FileContentResult> Export(Guid reportId)
        {
            var data = await _staffReportsApiClient.GetReport(reportId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(data), true);

                return File(package.GetAsByteArray(), "application/excel", $"report.xlsx");
            }
        }
    }
}