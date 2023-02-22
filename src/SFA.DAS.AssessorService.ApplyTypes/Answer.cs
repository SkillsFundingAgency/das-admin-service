using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class Answer
    {
        public string QuestionId { get; set; }

        [JsonIgnore]
        public string Value
        {
            get { return JsonValue as string; }
            set { JsonValue = value; }
        }

        [JsonPropertyName("Value")]
        public dynamic JsonValue { get; set; }

        public override string ToString()
        {
            if (JsonValue == null)
                return null;

            if (JsonValue is string stringValue)
            {
                return stringValue;
            }
            var jsonValue = new JObject(JsonValue);
            return string.Join(", ", jsonValue.Properties().
                Where(p => !string.IsNullOrEmpty(p.Value.ToString())).
                Select(p => p.Value.ToString()));
        }
    }
}
 