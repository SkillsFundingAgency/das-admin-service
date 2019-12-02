using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Application;
using SFA.DAS.RoatpAssessor.Application.Gateway.Requests;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RoatpApplication = SFA.DAS.RoatpAssessor.Domain.DTOs.Application;
using RoatpGateway = SFA.DAS.RoatpAssessor.Domain.DTOs.Gateway;

namespace SFA.DAS.RoatpAssessor.Tests.Application.Gateway.Requests
{
    [TestFixture]
    public class GetDashboardHandlerTests
    {
        [TestCase(nameof(RoatpApplication.OrganisationName), false, "a", "b", "c", "d","e")]
        [TestCase(nameof(RoatpApplication.OrganisationName), true, "e", "d", "c", "b", "a")]
        [TestCase(nameof(RoatpApplication.Ukprn), false, "e", "a", "d", "c", "b")]
        [TestCase(nameof(RoatpApplication.Ukprn), true, "b", "c", "d", "a", "e")]
        [TestCase(nameof(RoatpApplication.ApplicationRef), false, "d", "b", "a", "e", "c")]
        [TestCase(nameof(RoatpApplication.ApplicationRef), true, "c", "e", "a", "b", "d")]
        [TestCase(nameof(RoatpApplication.ProviderRoute), false, "b", "d", "e", "c", "a")]
        [TestCase(nameof(RoatpApplication.ProviderRoute), true, "a", "c", "e", "d", "b")]
        [TestCase(nameof(RoatpApplication.SubmittedAt), false, "c", "e", "b", "a", "d")]
        [TestCase(nameof(RoatpApplication.SubmittedAt), true, "d", "a", "b", "e", "c")]
        public async Task ShouldOrderNewApplications(string sortBy, bool sortDescending, string first, string second, string third, string fourth, string fifth)
        {
            var applyApiClientMock = new Mock<IApplyApiClient>();
            applyApiClientMock.Setup(a => a.GetGatewayCountsAsync())
                .ReturnsAsync(new GatewayCounts());

            applyApiClientMock.Setup(a => a.GetSubmittedApplicationsAsync())
                .ReturnsAsync(new List<RoatpApplication>
                {
                    new RoatpApplication{OrganisationName = "b", Ukprn = "5", ApplicationRef = "refb", ProviderRoute = "rte1", SubmittedAt = DateTime.Parse("2003-01-01")},
                    new RoatpApplication{OrganisationName = "e", Ukprn = "1", ApplicationRef = "refd", ProviderRoute = "rte3", SubmittedAt = DateTime.Parse("2002-01-01")},
                    new RoatpApplication{OrganisationName = "a", Ukprn = "2", ApplicationRef = "refc", ProviderRoute = "rte5", SubmittedAt = DateTime.Parse("2004-01-01")},
                    new RoatpApplication{OrganisationName = "c", Ukprn = "4", ApplicationRef = "refe", ProviderRoute = "rte4", SubmittedAt = DateTime.Parse("2001-01-01")},
                    new RoatpApplication{OrganisationName = "d", Ukprn = "3", ApplicationRef = "refa", ProviderRoute = "rte2", SubmittedAt = DateTime.Parse("2005-01-01")}
                });

            var handler = new GetDashboardHandler(applyApiClientMock.Object);

            var request = new GetDashboardRequest(RoatpAssessor.Application.Gateway.DashboardTab.New, sortBy, sortDescending);

            var response = await handler.Handle(request, new CancellationToken());

            response.NewApplications[0].OrganisationName.Should().Be(first);
            response.NewApplications[1].OrganisationName.Should().Be(second);
            response.NewApplications[2].OrganisationName.Should().Be(third);
            response.NewApplications[3].OrganisationName.Should().Be(fourth);
            response.NewApplications[4].OrganisationName.Should().Be(fifth);
        }

        [TestCase(nameof(RoatpGateway.OrganisationName), false, "a", "b", "c", "d", "e")]
        [TestCase(nameof(RoatpGateway.OrganisationName), true, "e", "d", "c", "b", "a")]
        [TestCase(nameof(RoatpGateway.Ukprn), false, "e", "a", "d", "c", "b")]
        [TestCase(nameof(RoatpGateway.Ukprn), true, "b", "c", "d", "a", "e")]
        [TestCase(nameof(RoatpGateway.ApplicationRef), false, "d", "b", "a", "e", "c")]
        [TestCase(nameof(RoatpGateway.ApplicationRef), true, "c", "e", "a", "b", "d")]
        [TestCase(nameof(RoatpGateway.ProviderRoute), false, "b", "d", "e", "c", "a")]
        [TestCase(nameof(RoatpGateway.ProviderRoute), true, "a", "c", "e", "d", "b")]
        [TestCase(nameof(RoatpGateway.SubmittedAt), false, "c", "e", "b", "a", "d")]
        [TestCase(nameof(RoatpGateway.SubmittedAt), true, "d", "a", "b", "e", "c")]
        [TestCase(nameof(RoatpGateway.AssignedToName), false, "a", "b", "e", "d", "c")]
        [TestCase(nameof(RoatpGateway.AssignedToName), true, "c", "d", "e", "b", "a")]
        public async Task ShouldOrderInProgress(string sortBy, bool sortDescending, string first, string second, string third, string fourth, string fifth)
        {
            var applyApiClientMock = new Mock<IApplyApiClient>();
            applyApiClientMock.Setup(a => a.GetGatewayCountsAsync())
                .ReturnsAsync(new GatewayCounts());

            applyApiClientMock.Setup(a => a.GetInProgressAsync())
                .ReturnsAsync(new List<RoatpGateway>
                {
                    new RoatpGateway{OrganisationName = "b", Ukprn = "5", ApplicationRef = "refb", ProviderRoute = "rte1", SubmittedAt = DateTime.Parse("2003-01-01"), AssignedToName = "a2"},
                    new RoatpGateway{OrganisationName = "e", Ukprn = "1", ApplicationRef = "refd", ProviderRoute = "rte3", SubmittedAt = DateTime.Parse("2002-01-01"), AssignedToName = "a3"},
                    new RoatpGateway{OrganisationName = "a", Ukprn = "2", ApplicationRef = "refc", ProviderRoute = "rte5", SubmittedAt = DateTime.Parse("2004-01-01"), AssignedToName = "a1"},
                    new RoatpGateway{OrganisationName = "c", Ukprn = "4", ApplicationRef = "refe", ProviderRoute = "rte4", SubmittedAt = DateTime.Parse("2001-01-01"), AssignedToName = "a5"},
                    new RoatpGateway{OrganisationName = "d", Ukprn = "3", ApplicationRef = "refa", ProviderRoute = "rte2", SubmittedAt = DateTime.Parse("2005-01-01"), AssignedToName = "a4"}
                });

            var handler = new GetDashboardHandler(applyApiClientMock.Object);

            var request = new GetDashboardRequest(RoatpAssessor.Application.Gateway.DashboardTab.InProgress, sortBy, sortDescending);

            var response = await handler.Handle(request, new CancellationToken());

            response.InProgress[0].OrganisationName.Should().Be(first);
            response.InProgress[1].OrganisationName.Should().Be(second);
            response.InProgress[2].OrganisationName.Should().Be(third);
            response.InProgress[3].OrganisationName.Should().Be(fourth);
            response.InProgress[4].OrganisationName.Should().Be(fifth);
        }
    }
}
