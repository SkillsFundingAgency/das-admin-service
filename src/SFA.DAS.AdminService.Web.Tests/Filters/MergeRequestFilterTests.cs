using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Attributes;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Tests.Filters
{
    public class MergeRequestFilterTests
    {
        private Fixture _autoFixture;
        private MergeRequest _mergeRequest;

        private Mock<IMergeOrganisationSessionService> _mockSessionService;
        private Mock<IServiceProvider> _mockServiceProvider;
        private Mock<HttpContext> _mockHttpContext;


        [SetUp]
        public void Arrange()
        {
            _autoFixture = new Fixture();
        }

        [Test]
        public void When_MergeRequestIsComplete_Then_RedirectToConfirmationScreen()
        {
            _mergeRequest = _autoFixture.Build<MergeRequest>()
               .With(r => r.Completed, true).Create();

            SetupMockServices(_mergeRequest);

            var actionFilter = new MergeRequestFilter();

            var actionContext = new ActionContext(_mockHttpContext.Object,
                new RouteData(),
                new ActionDescriptor(),
                new ModelStateDictionary());

            var actionExecutingContext = new ActionExecutingContext(actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controller: null);

            actionFilter.OnActionExecuting(actionExecutingContext);

            var result = actionExecutingContext.Result as RedirectToActionResult;

            result.ActionName.Should().Be("MergeComplete");

        }

        private void SetupMockServices(MergeRequest mergeRequest)
        {
            _mockSessionService = new Mock<IMergeOrganisationSessionService>();

            _mockSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(_mergeRequest);

            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceProvider.Setup(provider => provider.GetService(typeof(IMergeOrganisationSessionService)))
                .Returns(_mockSessionService.Object);

            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpContext.SetupGet(context => context.RequestServices)
                .Returns(_mockServiceProvider.Object);
        }
    }
}
