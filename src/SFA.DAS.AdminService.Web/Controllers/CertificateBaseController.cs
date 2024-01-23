using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class CertificateBaseController : Controller
    {
        protected readonly ILogger<CertificateBaseController> Logger;
        protected readonly IHttpContextAccessor ContextAccessor;

        private readonly ICertificateApiClient _certificateApiClient;
        private readonly ILearnerDetailsApiClient _learnerDetailsApiClient;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IScheduleApiClient _scheduleApiClient;
        private readonly IStandardVersionApiClient _standardVersionApiClient;

        public CertificateBaseController(
            ILogger<CertificateBaseController> logger, 
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            ILearnerDetailsApiClient learnerDetailsApiClient,
            IOrganisationsApiClient organisationsApiClient,
            IScheduleApiClient scheduleApiClient,
            IStandardVersionApiClient standardVersionApiClient)
        {
            Logger = logger;
            ContextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
            _learnerDetailsApiClient = learnerDetailsApiClient;
            _organisationsApiClient = organisationsApiClient;
            _scheduleApiClient = scheduleApiClient;
            _standardVersionApiClient = standardVersionApiClient;
        }

        protected ICertificateApiClient CertificateApiClient => _certificateApiClient;
        protected ILearnerDetailsApiClient LearnerDetailsApiClient => _learnerDetailsApiClient;
        protected IOrganisationsApiClient OrganisationsApiClient => _organisationsApiClient;
        protected IScheduleApiClient ScheduleApiClient => _scheduleApiClient;
        protected IStandardVersionApiClient StandardVersionApiClient => _standardVersionApiClient;

        protected async Task<IActionResult> LoadViewModel<T>(Guid id, string view) where T : ICertificateViewModel, new()
        {
            var username = ContextAccessor.HttpContext.User.UserId();

            Logger.LogInformation($"Load View Model for {typeof(T).Name} for {username}");
            
            var viewModel = new T();
            var certificate = await CertificateApiClient.GetCertificate(id);
            var organisation = await _organisationsApiClient.Get(certificate.OrganisationId);
            certificate.Organisation = organisation;

            Logger.LogInformation($"Got Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id}");

            viewModel.FromCertificate(certificate);

            Logger.LogInformation($"Got View Model of type {typeof(T).Name} requested by {username}");

            return View(view, viewModel);
        }

        protected async Task<IActionResult> SaveViewModel<T>(T vm, string returnToIfModelNotValid, RedirectToActionResult nextAction, string action) where T : ICertificateViewModel
        {
            var username = ContextAccessor.HttpContext.User.UserId();

            Logger.LogInformation($"Save View Model for {typeof(T).Name} for {username} with values: {GetModelValues(vm)}");

            var certificate = await CertificateApiClient.GetCertificate(vm.Id);
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            if(vm.RequiresReasonForChange && string.IsNullOrEmpty(vm.ReasonForChange))
            {
                ModelState.AddModelError(nameof(vm.ReasonForChange), "Please enter a reason");
            }

            if (!ModelState.IsValid)
            {
                vm.FamilyName = certData.LearnerFamilyName;
                vm.GivenNames = certData.LearnerGivenNames;
                Logger.LogInformation($"Model State not valid for {typeof(T).Name} requested by {username} with Id {certificate.Id}. Errors: {ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)}");
                return View(returnToIfModelNotValid, vm);
            }

            // If we are changing the version then blank out the option.
            if(action == "Version" && vm is CertificateVersionViewModel)
            {
                var cvvm = vm as CertificateVersionViewModel;
                if(cvvm.StandardUId != certificate.StandardUId)
                {
                    certData.CourseOption = null;
                }
            }

            var updatedCertificate = vm.GetCertificateFromViewModel(certificate, certData);

            await CertificateApiClient.UpdateCertificate(new UpdateCertificateRequest(updatedCertificate) { Username = username, Action = action, ReasonForChange = vm.ReasonForChange });

            Logger.LogInformation($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} updated.");

            Logger.LogInformation($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} redirecting to {nextAction.ControllerName} {nextAction.ActionName}");
            return nextAction;
        }

        private string GetModelValues<T>(T viewModel)
        {
            var properties = typeof(T).GetProperties().ToList();

            return properties.Aggregate("", (current, prop) => current + $"{prop.Name}: {prop.GetValue(viewModel)}, ");
        }
    }
}