using System;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class ExternalApiException : Exception
    {
        public ExternalApiException() : base()
        {

        }

        public ExternalApiException(string message) : base (message)
        {

        }

        public ExternalApiException(string message, Exception innerException) : base(message, innerException)
        {

        }

    }
}
