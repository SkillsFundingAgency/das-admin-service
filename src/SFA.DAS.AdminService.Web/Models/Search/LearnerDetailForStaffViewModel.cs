using System;
using System.Text;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AdminService.Web.Models.Search
{
    public class LearnerDetailForStaffViewModel
    {
        public LearnerDetailResult Learner { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public bool ShowDetail { get; set; }
        public int? BatchNumber { get; set; }
        public bool CanRequestDuplicate => CertificateStatus.CanRequestDuplicateCertificate(Learner.CertificateStatus);
        public bool CanAmendCertificate => CertificateStatus.CanAmendCertificate(Learner.CertificateStatus);
        public bool CanDeleteCertificate => Learner.CertificateReference != null &&
                                            Learner.CertificateStatus != CertificateStatus.Deleted;

        public bool ShowToAdress => Learner.CertificateStatus == CertificateStatus.NotDelivered ||
                                    Learner.CertificateStatus == CertificateStatus.Delivered;
        public string DateStatusTitle => GetDateStatusTitle(Learner.CertificateStatus);
        public string AddressedTo => GetAddressedTo(Learner);
        public DateTime UpdatedStatusDate => GetUpdatedStatusDate(Learner.CertificateStatus);
        public string ReasonForChange => GetReasonForChange(Learner.ReasonForChange);

        private string GetReasonForChange(string learnerReasonForChange)
        {
            if (string.IsNullOrWhiteSpace(learnerReasonForChange))
                return "No reason entered";
            return learnerReasonForChange;
        }

        public string RecipientTitle => GetRecipientTitle(Learner.CertificateStatus);
        private string GetAddressedTo(LearnerDetailResult learnerDetailResult)
        {
            string message = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (learnerDetailResult != null)
            {
                sb.Append(learnerDetailResult.ContactName);
                sb.Append(Environment.NewLine);
                sb.Append(learnerDetailResult.ContactOrganisation);
                sb.Append(Environment.NewLine);
                sb.Append(learnerDetailResult.ContactAddLine1);
                sb.Append(Environment.NewLine);
                sb.Append(learnerDetailResult.ContactAddLine2);
                sb.Append(Environment.NewLine);
                sb.Append(learnerDetailResult.ContactAddLine3);
                sb.Append(Environment.NewLine);
                sb.Append(learnerDetailResult.ContactAddLine4);
                sb.Append(Environment.NewLine);
                sb.Append(learnerDetailResult.ContactPostCode);
                message = sb.ToString();
            }
            return message;
        }

        private DateTime GetUpdatedStatusDate(string learnerCertificateStatus)
        {
            if (CertificateStatus.HasPrintNotificateStatus(learnerCertificateStatus))
                if (Learner.PrintStatusAt != null)
                    return (DateTime)Learner.PrintStatusAt;
            if (Learner.LastUpdatedAt != null)
                return (DateTime)Learner.LastUpdatedAt;
            return default;
        }

        private string GetDateStatusTitle(string learnerCertificateStatus)
        {
            var dateTitle = "Date ";
            switch (learnerCertificateStatus)
            {
                case "Submitted":
                case "ToBeApproved":
                    return dateTitle + "submitted";
                case "SentToPrinter":
                    return dateTitle + "sent to printer";
                case "Printed":
                    return dateTitle + "printed";
                case "NotDelivered":
                    return dateTitle + "delivery attempted";
                case "Delivered":
                    return dateTitle + "delivered";
                case "Deleted":
                    return dateTitle + "deleted";
                case "Reprint":
                    return dateTitle + "reprint requested";
            }

            return string.Empty;
        }

        private string GetRecipientTitle(string learnerCertificateStatus)
        {
            switch (learnerCertificateStatus)
            {
                case "Delivered":
                    return "Delivered to";
                case "NotDelivered":
                    return "Delivery attempted to";
            }
            return string.Empty;
        }
    }
}