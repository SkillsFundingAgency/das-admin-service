namespace SFA.DAS.AdminService.Web.UnitTests.Infrastructure
{
    using FluentAssertions;
    using NUnit.Framework;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using static SFA.DAS.AdminService.Web.Infrastructure.ModelStatePersistAttribute;

    public class ModelStateTransferValueTests
    {
        [Test]
        public void Deserialisation_Works_As_Expected()
        {
            const string json = @"{
                                    ""Key"": ""some key"",
                                    ""AttemptedValue"": ""123"",
                                    ""RawValue"": 123,
                                    ""ErrorMessages"": [ 
                                                         ""Something broke"",
                                                         ""Something is not working"",
                                                         ""Nothing is working""
                                                       ]
                                   }";

            var sut = JsonSerializer.Deserialize<ModelStateTransferValue>(json);

            Assert.Multiple(() =>
            {
                Assert.That(sut.Key, Is.EqualTo("some key"));
                Assert.That(sut.AttemptedValue, Is.EqualTo("123"));
                Assert.That(sut.RawValue.ToString(), Is.EqualTo("123"));
                Assert.That(sut.ErrorMessages, Has.Count.EqualTo(3));
            });
        }
    }
}
