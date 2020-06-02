using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.FeatureToggles;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Tests.Infrastructure.FeatureToggles
{
    [TestFixture]
    public class FeatureToggleFilterTests
    {
        private IWebConfiguration _WebConfiguration;
        private FeatureToggleFilter _FeatureToggleFilter;
        private ActionExecutingContext _ActionExecutingContext;

        [SetUp]
        public void Setup()
        {
            _WebConfiguration = new WebConfiguration
            {
                FeatureToggles = new Common.Settings.FeatureToggles { }
            };

            var filterLogger = Mock.Of<ILogger<FeatureToggleFilter>>();

            _FeatureToggleFilter = new FeatureToggleFilter(filterLogger, _WebConfiguration);

            var actionContext = new ActionContext(Mock.Of<HttpContext>(), Mock.Of<RouteData>(), Mock.Of<ActionDescriptor>());

            _ActionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), Mock.Of<RoatpGatewayControllerBase<RoatpGatewayController>>());
        }

        [Test]
        public void FeatureToggleFilter_Redirects_When_FeatureToggles_Not_Enabled()
        {
            _WebConfiguration.FeatureToggles.EnableRoatpGatewayReview = false;

            _FeatureToggleFilter.OnActionExecuting(_ActionExecutingContext);

            var actualResult = _ActionExecutingContext.Result;

            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf<RedirectToActionResult>(actualResult);
        }

        [Test]
        public void FeatureToggleFilter_Does_Not_Redirect_When_FeatureToggles_Is_Enabled()
        {
            _WebConfiguration.FeatureToggles.EnableRoatpGatewayReview = true;

            _FeatureToggleFilter.OnActionExecuting(_ActionExecutingContext);

            var actualResult = _ActionExecutingContext.Result;

            Assert.Null(actualResult);
            Assert.IsNotInstanceOf<RedirectToActionResult>(actualResult);
        }
    }
}
