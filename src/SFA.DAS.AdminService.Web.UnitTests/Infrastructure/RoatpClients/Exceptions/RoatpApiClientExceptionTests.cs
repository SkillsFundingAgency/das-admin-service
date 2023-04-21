using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace SFA.DAS.AdminService.Web.Tests.Infrastructure.RoatpClients.Exceptions
{
    [TestFixture]
    public class RoatpApiClientExceptionTests
    {
        private readonly string HttpMethod = System.Net.Http.HttpMethod.Patch.ToString();
        private readonly HttpStatusCode HttpStatusCode = HttpStatusCode.NotAcceptable;
        private readonly Uri RequestUri = new Uri("http://www.google.co.uk");

        [Test]
        public void Ensure_RoatpApiClientException_Seralizes_and_Deserializes_Correctly()
        {
            RoatpApiClientException ex = new RoatpApiClientException(HttpMethod, HttpStatusCode, RequestUri, null);

            // Sanity check: Make sure custom properties are set before serialization
            Assert.That(ex.HttpMethod, Is.EqualTo(HttpMethod), "HttpMethod");
            Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode), "StatusCode");
            Assert.That(ex.RequestUri, Is.EqualTo(RequestUri), "RequestUri");

            // Round-trip the exception: Serialize and de-serialize with a JsonSerializer
            var jsonConfig = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreReadOnlyProperties = false
            };
            using (MemoryStream ms = new MemoryStream())
            {
                JsonSerializer.Serialize(ms, ex, jsonConfig);
                ms.Seek(0, 0);
                ex = JsonSerializer.Deserialize<RoatpApiClientException>(ms, jsonConfig);
            }

            // Make sure custom properties are preserved after serialization
            Assert.That(ex.HttpMethod, Is.EqualTo(HttpMethod), "HttpMethod");
            Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode), "StatusCode");
            Assert.That(ex.RequestUri, Is.EqualTo(RequestUri), "RequestUri");
        }
    }
}
