﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Private;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.AdminService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/uln")]
    public class CertificatePrivateUlnController : CertificateBaseController
    {
        public CertificatePrivateUlnController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClientFactory<ApiClient> apiClient) : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Uln(Guid certificateId, bool fromApproval)
        {       
            var viewModel = await LoadViewModel<CertificateUlnViewModel>(certificateId, "~/Views/CertificateAmend/Uln.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateUlnViewModel certificateUlnViewModel)
                certificateUlnViewModel.FromApproval = fromApproval;

            return viewModel;
        }

        [HttpPost(Name = "Uln")]
        public async Task<IActionResult> Uln(CertificateUlnViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Uln.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id, fromapproval = vm.FromApproval }), action: CertificateActions.Uln);

            return actionResult;
        }
    }
}