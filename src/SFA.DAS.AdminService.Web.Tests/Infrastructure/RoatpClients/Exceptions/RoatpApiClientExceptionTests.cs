using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

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
            Assert.AreEqual(HttpMethod, ex.HttpMethod, "HttpMethod");
            Assert.AreEqual(HttpStatusCode, ex.StatusCode, "StatusCode");
            Assert.AreEqual(RequestUri, ex.RequestUri, "RequestUri");

            // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, ex);
                ms.Seek(0, 0);
                ex = bf.Deserialize(ms) as RoatpApiClientException;
            }

            // Make sure custom properties are preserved after serialization
            Assert.AreEqual(HttpMethod, ex.HttpMethod, "HttpMethod");
            Assert.AreEqual(HttpStatusCode, ex.StatusCode, "StatusCode");
            Assert.AreEqual(RequestUri, ex.RequestUri, "RequestUri");
        }
    }
}
