using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AdminService.Common.Infrastructure.Firewall
{
    /// <summary>
    /// This is raised when the WAF (Web Application Firewall) intercepts what it thinks is a suspicious request
    /// or there is an internal network issue.
    /// </summary>
    public class ApiError
    {
        [JsonInclude]
        public int StatusCode { get; private set; }

        [JsonInclude]
        public string StatusDescription { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Message { get; private set; }

        public ApiError()
        {
        }

        public ApiError(int statusCode, string statusDescription)
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
