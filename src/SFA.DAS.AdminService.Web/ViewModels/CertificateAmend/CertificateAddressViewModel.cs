using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateAddressViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public CertificateSendTo SendTo { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }
        public string Employer { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public bool EditForm { get; set; }

        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);

            SendTo = CertificateData.SendTo;
            Name = CertificateData.ContactName;
            Dept = CertificateData.Department;
            Employer = CertificateData.ContactOrganisation;
            AddressLine1 = CertificateData.ContactAddLine1;
            AddressLine2 = CertificateData.ContactAddLine2;
            AddressLine3 = CertificateData.ContactAddLine3;
            City = CertificateData.ContactAddLine4;
            Postcode = CertificateData.ContactPostCode;
        }
        
        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));

            certData.ContactName = Name;
            certData.Department = Dept;
            certData.ContactOrganisation = Employer;
            certData.ContactAddLine1 = AddressLine1;
            certData.ContactAddLine2 = AddressLine2;
            certData.ContactAddLine3 = AddressLine3;
            certData.ContactAddLine4 = City;
            certData.ContactPostCode = Postcode;

            certificate.CertificateData = certData;

            return certificate;
        }
    }
}