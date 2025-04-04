using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Consts;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Shared
{
    public class LearnerDetailViewModel : CertificateHistoryViewModel
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

        public string ReasonForChange => GetReasonForChange(Learner.ReasonForChange);

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
