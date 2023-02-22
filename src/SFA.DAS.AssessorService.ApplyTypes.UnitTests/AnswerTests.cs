using System.Dynamic;

namespace SFA.DAS.AssessorService.ApplyTypes.UnitTests
{
    public class AnswerTests
    {
        [Test]
        public void ToString_ReturnsNull_If_JsonValue_Is_Null()
        {
            var sut = new Answer { JsonValue = null };

            Assert.That(sut.ToString(), Is.Null);
        }

        [Test]
        public void ToString_ReturnsIdenticalString_If_JsonValue_Is_String()
        {
            const string randomString = "random string carburettor";
            var sut = new Answer { JsonValue = randomString };

            Assert.That(sut.ToString(), Is.EqualTo(randomString));
        }

        [Test]
        public void ToString_ReturnsExpected_If_JsonValue_IsNot_Null_Or_String()
        {
            const string testJsonValue = @"{ 
                                              ""QuestionId"": ""CC-120"", 
                                              ""Value"": ""123"" 
                                           }";

            dynamic testExpando = new ExpandoObject();
            testExpando.QuestionId = "123";
            testExpando.Value = "Yes";

            var sut = new Answer { JsonValue = testExpando };

            var t = sut.ToString();
        }
    }
}