using System;
using System.Text.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateCheckViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public long Uln { get; set; }
        public int? Ukprn { get; set; }

        public string Status { get; set; }
        public string CertificateReference { get; set; }
        
        
        public string Option { get; set; }
        public string SelectedGrade { get; set; }
        public DateTime? AchievementDate { get; set; }
        public DateTime? LearnerStartDate { get; set; }

        public CertificateSendTo SendTo { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }
        public string Employer { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        
        public bool RedirectToCheck { get; set; }

        public string SearchString { get; set; }
        public int Page { get; set; }

        public bool StandardHasMultipleVersions { get; set; }

        public bool ShowOptionsChangeLink { get; set; }

        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);

            Uln = cert.Uln;
            Ukprn = cert.ProviderUkPrn;

            Status = cert.Status;
            PrivatelyFundedStatus = cert.PrivatelyFundedStatus;
            CertificateReference = cert.CertificateReference;

            Level = CertificateData.StandardLevel;
            Option = CertificateData.CourseOption;
            SelectedGrade = CertificateData.OverallGrade;
            AchievementDate = CertificateData.AchievementDate;
            LearnerStartDate = CertificateData.LearningStartDate;

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

        public bool CanRequestReprint => CertificateStatus.CanRequestReprintCertificate(Status);
    }
}