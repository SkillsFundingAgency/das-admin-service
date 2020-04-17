using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Attributes;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure.FeatureToggles;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [ExternalApiExceptionFilter]
    [Authorize(Roles = Roles.RoatpGatewayAssessorTeam)]
    [FeatureToggle(FeatureToggles.EnableRoatpGatewayReview, "Dashboard", "Index")]
    public class RoatpGatewayControllerBase<T> : Controller
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        protected readonly IRoatpApplicationApiClient _applyApiClient;
        protected readonly ILogger<T> _logger;
        private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
        protected const string GatewayViewsLocation = "~/Views/Roatp/Apply/Gateway/pages";

        public RoatpGatewayControllerBase()
        {

        }

        public RoatpGatewayControllerBase(IHttpContextAccessor contextAccessor, IRoatpApplicationApiClient applyApiClient, ILogger<T> logger, IRoatpGatewayPageViewModelValidator gatewayValidator)
        {
            _contextAccessor = contextAccessor;
            _applyApiClient = applyApiClient;
            _logger = logger;
            _gatewayValidator = gatewayValidator;
        }

        public string SetupGatewayPageOptionTexts(RoatpGatewayPageViewModel viewModel)
        {
            if (viewModel?.Status == null) return string.Empty;
            viewModel.OptionInProgressText = viewModel.Status == SectionReviewStatus.InProgress && !string.IsNullOrEmpty(viewModel.OptionInProgressText) ? viewModel.OptionInProgressText : string.Empty;
            viewModel.OptionPassText = viewModel.Status == SectionReviewStatus.Pass && !string.IsNullOrEmpty(viewModel.OptionPassText) ? viewModel.OptionPassText : string.Empty;
            viewModel.OptionFailText = viewModel.Status == SectionReviewStatus.Fail && !string.IsNullOrEmpty(viewModel.OptionFailText) ? viewModel.OptionFailText : string.Empty;

            switch (viewModel.Status)
            {
                case SectionReviewStatus.Pass:
                    return viewModel.OptionPassText;
                case SectionReviewStatus.Fail:
                    return viewModel.OptionFailText;
                case SectionReviewStatus.InProgress:
                    return viewModel.OptionInProgressText;
                default:
                    return string.Empty;
            }
        }

        protected async Task<IActionResult> SubmitGatewayPageAnswer(RoatpGatewayPageViewModel viewModel, string errorViewName)
        {
            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View(errorViewName, viewModel);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var comments = SetupGatewayPageOptionTexts(viewModel);

            _logger.LogInformation($"{typeof(T).Name}-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - Comments '{comments}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{typeof(T).Name}-SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }
    }
}
