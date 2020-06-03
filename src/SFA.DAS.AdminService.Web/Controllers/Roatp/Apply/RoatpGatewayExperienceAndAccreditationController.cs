using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    public class RoatpGatewayExperienceAndAccreditationController : RoatpGatewayControllerBase<RoatpGatewayExperienceAndAccreditationController>
    {
        private readonly IGatewayExperienceAndAccreditationOrchestrator _orchestrator;
        
        public RoatpGatewayExperienceAndAccreditationController( IHttpContextAccessor contextAccessor, IRoatpApplicationApiClient roatpApiClient, IRoatpGatewayPageValidator validator, IGatewayExperienceAndAccreditationOrchestrator orchestrator, ILogger<RoatpGatewayExperienceAndAccreditationController> logger) : base(contextAccessor, roatpApiClient, logger, validator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclaration")]
        public async Task<ViewResult> SubcontractorDeclaration(Guid applicationId)
        {
            var userName = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetSubcontractorDeclarationViewModel(new GetSubcontractorDeclarationRequest(applicationId, userName));
            return View($"{GatewayViewsLocation}/SubcontractorDeclaration.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclarationFile")]
        public async Task<FileStreamResult> SubcontractorDeclarationContractFile(Guid applicationId)
        {
            return await _orchestrator.GetSubcontractorDeclarationContractFile(new GetSubcontractorDeclarationContractFileRequest(applicationId));
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclaration")]
        public async Task<IActionResult> EvaluateSubcontractorDeclarationPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<SubcontractorDeclarationViewModel>> viewModelBuilder = () => _orchestrator.GetSubcontractorDeclarationViewModel(new GetSubcontractorDeclarationRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/SubcontractorDeclaration.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OfficeForStudents")]
        public async Task<ViewResult> OfficeForStudents(Guid applicationId)
        {
            var userName = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOfficeForStudentsViewModel(new GetOfficeForStudentsRequest(applicationId, userName));
            return View($"{GatewayViewsLocation}/OfficeForStudents.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OfficeForStudents")]
        public async Task<IActionResult> EvaluateOfficeForStudentsPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OfficeForStudentsViewModel>> viewModelBuilder = () => _orchestrator.GetOfficeForStudentsViewModel(new GetOfficeForStudentsRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OfficeForStudents.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/InitialTeacherTraining")]
        public async Task<ViewResult> InitialTeacherTraining(Guid applicationId)
        {
            var userName = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetInitialTeacherTrainingViewModel(new GetInitialTeacherTrainingRequest(applicationId, userName));
            return View($"{GatewayViewsLocation}/InitialTeacherTraining.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/InitialTeacherTraining")]
        public async Task<IActionResult> EvaluateInitialTeacherTrainingPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<InitialTeacherTrainingViewModel>> viewModelBuilder = () => _orchestrator.GetInitialTeacherTrainingViewModel(new GetInitialTeacherTrainingRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/InitialTeacherTraining.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Ofsted")]
        public async Task<ViewResult> OfstedDetails(Guid applicationId)
        {
            var userName = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOfstedDetailsViewModel(new GetOfstedDetailsRequest(applicationId, userName));
            return View($"{GatewayViewsLocation}/OfstedDetails.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Ofsted")]
        public async Task<IActionResult> EvaluateOfstedDetailsPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OfstedDetailsViewModel>> viewModelBuilder = () => _orchestrator.GetOfstedDetailsViewModel(new GetOfstedDetailsRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OfstedDetails.cshtml");
        }
    }
}