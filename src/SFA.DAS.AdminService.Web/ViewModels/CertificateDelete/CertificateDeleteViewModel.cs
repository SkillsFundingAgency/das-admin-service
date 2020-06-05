using System;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateDelete
{
    public class CertificateDeleteViewModel 
    {
        public Guid CertificateId { get; set; }
        public string ReasonForChange { get; set; }
        public string IncidentNumber { get; set; }
        public bool IsDeleteConfirmed { get; set; }
    }
}