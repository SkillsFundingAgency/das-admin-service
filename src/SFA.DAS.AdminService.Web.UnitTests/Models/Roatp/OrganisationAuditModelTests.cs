using System;
using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;
using SFA.DAS.AdminService.Web.Models.Roatp;

namespace SFA.DAS.AdminService.Web.UnitTests.Models.Roatp;

public class OrganisationAuditModelTests
{
    [Test, AutoData]
    public void ImplicitOperator_MapsPropertiesCorrectly(OrganisationAuditRecord expected, DateTime expectedPreviousStatusDate, DateTime expectedUpdateAtDate)
    {
        expected.PreviousStatusDate = expectedPreviousStatusDate;
        expected.UpdatedAt = expectedUpdateAtDate;
        // Act
        OrganisationAuditModel sut = expected;
        // Assert
        Assert.That(sut.Ukprn, Is.EqualTo(expected.Ukprn));
        Assert.That(sut.LegalName, Is.EqualTo(expected.LegalName));
        Assert.That(sut.FieldChanged, Is.EqualTo(expected.FieldChanged));
        Assert.That(sut.PreviousValue, Is.EqualTo(expected.PreviousValue));
        Assert.That(sut.NewValue, Is.EqualTo(expected.NewValue));
        Assert.That(sut.PreviousStatusDate, Is.EqualTo(expectedPreviousStatusDate.ToString("yyyy-MM-dd HH:mm:ss")));
        Assert.That(sut.UpdatedAt, Is.EqualTo(expectedUpdateAtDate.ToString("yyyy-MM-dd HH:mm:ss")));
        Assert.That(sut.UpdatedBy, Is.EqualTo(expected.UpdatedBy));
    }
}
