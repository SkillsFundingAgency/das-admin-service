using System;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.DigitalAccess
{
    public class CertificatePrintRequestViewModel
    {
        public string ReferenceNumber { get; set; } = string.Empty;
        public string RequestType { get; set; } = "Reprint request";
        public string CourseName { get; set; } = string.Empty;
        public Guid CertificateId { get; set; }
        public CertificateType CertificateType { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public long? Uln { get; set; }
        public int? StandardCode { get; set; }
        public string ViewCertificateText => "View certificate";
        public string? ReturnUrl { get; set; }
    }
}
