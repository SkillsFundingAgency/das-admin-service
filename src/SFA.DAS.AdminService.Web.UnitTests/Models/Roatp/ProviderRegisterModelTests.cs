using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;
using SFA.DAS.AdminService.Web.Models.Roatp;

namespace SFA.DAS.AdminService.Web.UnitTests.Models.Roatp;

public class ProviderRegisterModelTests
{
    [Test, AutoData]
    public void ImplicitOperator_MapsPropertiesCorrectly(OrganisationModel source)
    {
        // Act
        ProviderRegisterModel sut = source;
        // Assert
        Assert.That(sut.ProviderType, Is.EqualTo(source.ProviderType));
        Assert.That(sut.Ukprn, Is.EqualTo(source.Ukprn));
        Assert.That(sut.LegalName, Is.EqualTo(source.LegalName));
        Assert.That(sut.TradingName, Is.EqualTo(source.TradingName));
        Assert.That(sut.OrganisationType, Is.EqualTo(source.OrganisationType));
        Assert.That(sut.CompanyNumber, Is.EqualTo(source.CompanyNumber));
        Assert.That(sut.CharityNumber, Is.EqualTo(source.CharityNumber));
        Assert.That(sut.Status, Is.EqualTo(source.Status));
        Assert.That(sut.StatusDate, Is.EqualTo(source.StatusDate.ToString("dd/MM/yyyy")));
        Assert.That(sut.RemovedReason, Is.EqualTo(source.RemovedReason));
        Assert.That(sut.StartDate, Is.EqualTo(source.StartDate?.ToString("dd/MM/yyyy")));
    }
}
