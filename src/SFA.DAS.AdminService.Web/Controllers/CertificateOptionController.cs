﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class CertificateOptionController : CertificateBaseController
    {
        public CertificateOptionController(
            ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            ILearnerDetailsApiClient learnerDetailsApiClient,
            IOrganisationsApiClient organisationsApiClient,
            IScheduleApiClient scheduleApiClient,
            IStandardVersionApiClient standardVersionApiClient) : base(logger, contextAccessor, certificateApiClient, learnerDetailsApiClient, organisationsApiClient, scheduleApiClient, standardVersionApiClient)
        { 
        }

        [HttpGet]
        public async Task<IActionResult> Option(Guid certificateId)
        {
            var actionResult = await LoadViewModel<CertificateOptionViewModel>(certificateId, "~/Views/CertificateAmend/Option.cshtml");
            if (actionResult is ViewResult viewResult && viewResult.Model is CertificateOptionViewModel certificateOptionViewModel)
            {
                var standardOption = await StandardVersionApiClient.GetStandardOptions(certificateOptionViewModel.GetStandardId());

                certificateOptionViewModel.Options = standardOption != null ? standardOption.CourseOption : new List<string>();
                certificateOptionViewModel.SelectedOption = certificateOptionViewModel.Option;
            }

            return actionResult;
        }

        [HttpPost(Name = "Option")]
        public async Task<IActionResult> Option(CertificateOptionViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Option.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Option);
        }
    }
}