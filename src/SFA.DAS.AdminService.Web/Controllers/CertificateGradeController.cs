using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class CertificateGradeController : CertificateBaseController
    {
        public CertificateGradeController(
            ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            ILearnerDetailsApiClient learnerDetailsApiClient,
            IOrganisationsApiClient organisationsApiClient,
            IScheduleApiClient scheduleApiClient,
            IStandardVersionApiClient standardVersionApiClient) : base(logger, contextAccessor, certificateApiClient, learnerDetailsApiClient, organisationsApiClient, scheduleApiClient, standardVersionApiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Grade(Guid certificateId)
        {
            var actionResult = await LoadViewModel<CertificateGradeViewModel>(certificateId, "~/Views/CertificateAmend/Grade.cshtml");
            return actionResult;
        }

        [HttpPost(Name = "Grade")]
        public async Task<IActionResult> Grade(CertificateGradeViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Grade.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Grade);
        }
    }
}