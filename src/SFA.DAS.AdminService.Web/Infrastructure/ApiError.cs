using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class ApiError
    {
        public int StatusCode { get; private set; }

        public string StatusDescription { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Message { get; private set; }

        public ApiError()
        {
        }

        public ApiError(int statusCode, string statusDescription)
            : this()
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        public ApiError(int statusCode, string statusDescription, string message)
            : this(statusCode, statusDescription)
        {
            Message = message;
        }
    }
}
