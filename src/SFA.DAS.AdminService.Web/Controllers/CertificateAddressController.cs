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
    public class CertificateAddressController : CertificateBaseController
    {
        public CertificateAddressController(
            ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            ILearnerDetailsApiClient learnerDetailsApiClient,
            IOrganisationsApiClient organisationsApiClient,
            IScheduleApiClient scheduleApiClient,
            IStandardVersionApiClient standardVersionApiClient) : base(logger, contextAccessor, certificateApiClient, learnerDetailsApiClient, organisationsApiClient, scheduleApiClient, standardVersionApiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Address(Guid certificateId, bool editForm = true)
        {
            var actionResult = await LoadViewModel<CertificateAddressViewModel>(certificateId, "~/Views/CertificateAmend/Address.cshtml");

            var viewResult = (actionResult as ViewResult);
            var viewModel = viewResult.Model as CertificateAddressViewModel;
            viewModel.EditForm = editForm;

            return actionResult;
        }

        [HttpPost(Name = "Grade")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Address.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Address);
        }
    }
}