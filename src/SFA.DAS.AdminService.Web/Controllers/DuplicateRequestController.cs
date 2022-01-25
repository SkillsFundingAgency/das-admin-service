﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class DuplicateRequestController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public DuplicateRequestController(ApiClient apiClient,
            IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmReprint(Guid certificateId,
            int stdCode,
            long uln,
            string searchString,
            int page = 1)
        {
            var vm = await ReprintCertificate(certificateId, searchString, stdCode, uln, page);
            return View(vm);
        }

        private async Task<DuplicateRequestViewModel> ReprintCertificate(Guid certificateId, string searchString, int larsCode, long uln, int page)
        {
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var certificate = await _apiClient.PostReprintRequest(new StaffCertificateDuplicateRequest
            {
                Id = certificateId,
                Username = username
            });
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            var nextScheduledRun = await _apiClient.GetNextScheduledRun((int)ScheduleType.PrintRun);
            var vm = new DuplicateRequestViewModel
            {
                CertificateId = certificate.Id,
                NextBatchDate = nextScheduledRun?.RunTime.ToString("dd/MM/yyyy"),
                CertificateReference = certificate.CertificateReference,
                SearchString = searchString,
                StdCode = larsCode,
                Uln = uln,
                Status = certificate.Status,
                FullName = certificateData.FullName,
                Page = page
            };

            return vm;
        }
    }
}