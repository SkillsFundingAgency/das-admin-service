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
                FeatureToggles = new Settings.FeatureToggles { }
            };

            var filterLogger = Mock.Of<ILogger<FeatureToggleFilter>>();

            _FeatureToggleFilter = new FeatureToggleFilter(filterLogger, _WebConfiguration);

            var actionContext = new ActionContext(Mock.Of<HttpContext>(), Mock.Of<RouteData>(), Mock.Of<ActionDescriptor>());

            _ActionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), Mock.Of<RoatpGatewayControllerBase>());
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void FeatureToggleFilter_Filters_Correctly(bool enableRoatpApply, bool shouldRedirect)
        {
            _WebConfiguration.FeatureToggles.EnableRoatpApply = enableRoatpApply;

            _FeatureToggleFilter.OnActionExecuting(_ActionExecutingContext);

            var actualResult = _ActionExecutingContext.Result;

            if(shouldRedirect)
            {
                Assert.IsNotNull(actualResult);
                Assert.IsInstanceOf<RedirectToActionResult>(actualResult);
            }
            else
            {
                Assert.Null(actualResult);
                Assert.IsNotInstanceOf<RedirectToActionResult>(actualResult);
            }
        }
    }
}
