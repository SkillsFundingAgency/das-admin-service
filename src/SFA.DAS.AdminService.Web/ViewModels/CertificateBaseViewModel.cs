using System;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels
{
    public class CertificateBaseViewModel
    {
        protected CertificateData CertificateData;

        public virtual void FromCertificate(Certificate cert)
        {
            CertificateData = cert.CertificateData;
            Id = cert.Id;
            GivenNames = CertificateData.LearnerGivenNames;
            FamilyName = CertificateData.LearnerFamilyName;
            Standard = CertificateData.StandardName;
            FullName = CertificateData.FullName;
            Level = CertificateData.StandardLevel;
            IsPrivatelyFunded = cert.IsPrivatelyFunded;
            StandardUId = cert.StandardUId;
            StandardCode = cert.StandardCode;
            Version = CertificateData.Version;
        }

        public virtual Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            return certificate;
        }

        public string PrivatelyFundedStatus { get; set; }
        public Guid Id { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string Standard { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public int Level { get; set; }    
        public string Version { get; set; }
        public string StandardUId { get; set; }
        public int StandardCode { get; set; }
        public bool IsPrivatelyFunded { get; set; }
        public bool BackToCheckPage { get; set; }
        public string ReasonForChange { get; set; }
        public bool RequiresReasonForChange { get; set; } = true;

        public string GetStandardId() => string.IsNullOrWhiteSpace(StandardUId) ? StandardCode.ToString() : StandardUId;
    }
}