namespace SFA.DAS.AdminService.Common.UnitTests.Infrastructure
{
    using System.Text.Json;
    using NUnit.Framework;
    using SFA.DAS.AdminService.Common.Infrastructure.Firewall;

    public class ApiErrorTests
    {
        const string json = @"{
                                ""StatusCode"": 123,
                                ""StatusDescription"": ""Bad things happened"",
                                ""Message"": ""But I will survive""
                              }";


        [Test]
        public void Deserialisation_Populates_StatusCode()
        {
            var sut = JsonSerializer.Deserialize<ApiError>(json);
            Assert.That(sut!.StatusCode, Is.EqualTo(123));
        }

        [Test]
        public void Deserialisation_Populates_StatusDescription()
        {
            var sut = JsonSerializer.Deserialize<ApiError>(json);
            Assert.That(sut!.StatusDescription, Is.EqualTo("Bad things happened"));
        }

        [Test]
        public void Deserialisation_Populates_Message_When_Message_IsPopulated()
        {
            var sut = JsonSerializer.Deserialize<ApiError>(json);
            Assert.That(sut!.Message, Is.EqualTo("But I will survive"));
        }

        [Test]
        public void Deserialisation_Populates_Message_When_Message_Is_Blank()
        {
            const string jsonWithBlankMessage = @"{
                                ""StatusCode"": 123,
                                ""StatusDescription"": ""Bad things happened"",
                                ""Message"": """"
                              }";

            var sut = JsonSerializer.Deserialize<ApiError>(jsonWithBlankMessage);
            Assert.That(sut!.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Serialisation_Ignores_Message_When_Writing_Null()
        {
            var sut = new ApiError(123, "description", null);
            var serialised = JsonSerializer.Serialize(sut);
            Assert.That(serialised, Does.Not.Contain("message"));
        }
    }
}
