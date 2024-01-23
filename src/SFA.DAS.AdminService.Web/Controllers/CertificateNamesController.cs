using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class CertificateNamesController : CertificateBaseController
    {
        public CertificateNamesController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            ILearnerDetailsApiClient learnerDetailsApiClient,
            IOrganisationsApiClient organisationsApiClient,
            IScheduleApiClient scheduleApiClient,
            IStandardVersionApiClient standardVersionApiClient) : base(logger, contextAccessor, certificateApiClient, learnerDetailsApiClient, organisationsApiClient, scheduleApiClient, standardVersionApiClient)
        { 
        }

        [HttpGet]
        [Route("certificate/givennames")]
        public async Task<IActionResult> GivenNames(Guid certificateId)
        {
            var viewModel = await LoadViewModel<CertificateGivenNamesViewModel>(certificateId, "~/Views/CertificateAmend/GivenNames.cshtml");
            return viewModel;
        }

        [HttpPost]
        [Route("certificate/givennames")]
        public async Task<IActionResult> GivenNames(CertificateGivenNamesViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/GivenNames.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.GivenNames);

            return actionResult;
        }

        [HttpGet]
        [Route("certificate/familyname")]
        public async Task<IActionResult> FamilyName(Guid certificateId)
        {
            var viewModel = await LoadViewModel<CertificateFamilyNameViewModel>(certificateId, "~/Views/CertificateAmend/FamilyName.cshtml");
            return viewModel;
        }

        [HttpPost]
        [Route("certificate/familyname")]
        public async Task<IActionResult> FamilyName(CertificateFamilyNameViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/FamilyName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.GivenNames);

            return actionResult;
        }
    }
}
