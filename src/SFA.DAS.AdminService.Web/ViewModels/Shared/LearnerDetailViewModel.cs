using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Consts;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Shared
{
    public class LearnerDetailViewModel
    {
        public LearnerDetailResult Learner { get; set; }

        public DateTime CertificateStatusDateValue
        {
            get
            {
                return CertificateStatus.HasPrintNotificateStatus(Learner.CertificateStatus) && Learner.PrintStatusAt.HasValue
                    ? Learner.PrintStatusAt.Value
                    : Learner.LastUpdatedAt ?? default;
            }
        }

        public string CertificateStatusDateTitle
        {
            get
            {
                var dateTitle = "Date ";
                switch (Learner?.CertificateStatus)
                {
                    case "Submitted":
                    case "ToBeApproved":
                        dateTitle += "submitted";
                        break;

                    case "SentToPrinter":
                        dateTitle += "sent to printer";
                        break;

                    case "Printed":
                        dateTitle += "printed";
                        break;

                    case "NotDelivered":
                        dateTitle += "delivery attempted";
                        break;

                    case "Delivered":
                        dateTitle += "delivered";
                        break;

                    case "Deleted":
                        dateTitle += "deleted";
                        break;

                    case "Reprint":
                        dateTitle += "reprint requested";
                        break;

                    default:
                        dateTitle += Learner?.CertificateStatus?.ToLower();
                        break;
                }

                return dateTitle;
            }
        }

        public string CertificateStatusDeliveryTitle
        {
            get
            {
                var title = string.Empty;

                switch (Learner?.CertificateStatus)
                {
                    case nameof(CertificateStatus.Submitted):
                    case nameof(CertificateStatus.SentToPrinter):
                    case nameof(CertificateStatus.Printed):
                    case nameof(CertificateStatus.Reprint):
                        title = "Delivery address";
                        break;
                    
                    case nameof(CertificateStatus.Delivered):
                        title = "Delivered to";
                        break;

                    case nameof(CertificateStatus.NotDelivered):
                        title = "Delivery attempted to";
                        break;
                }

                return title;
            }
        }
    }
}
