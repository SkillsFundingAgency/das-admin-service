using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.JsonData;

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
        public bool CanDeleteCertificate => Learner.CertificateReference != null && Learner.CertificateStatus != CertificateStatus.Deleted;
        public string DateStatusTitle => GetDateStatusTitle(Learner.CertificateStatus);
        public string AddressedTo => GetAddressedTo(Learner.CertificateLogs);
        public DateTime? UpdatedStatusDate => GetUpdatedStatusDate(Learner.CertificateLogs);
        public string Explanation => GetExplanation(Learner.CertificateLogs);
        private string GetAddressedTo(List<CertificateLogSummary> learnerCertificateLogs)
        {
            if(learnerCertificateLogs.Any())
            {
                var learnerLog = learnerCertificateLogs.OrderByDescending(e => e.EventTime)
                    .FirstOrDefault(l => l.Status == Learner.CertificateStatus);
                var certData = JsonConvert.DeserializeObject<CertificateData>(learnerLog?.CertificateData);
                return certData.ContactAddLine1
                       + Environment.NewLine
                       + certData.ContactAddLine2
                       + Environment.NewLine
                       + certData.ContactAddLine3
                       + Environment.NewLine
                       + certData.ContactAddLine4
                       + Environment.NewLine
                       + certData.ContactPostCode;
            }
            return string.Empty;
        }

        private DateTime? GetUpdatedStatusDate(List<CertificateLogSummary> learnerCertificateLogs)
        {
            if (learnerCertificateLogs.Any())
            {
                var learnerLog = learnerCertificateLogs.OrderByDescending(e => e.EventTime)
                    .FirstOrDefault(l => l.Status == Learner.CertificateStatus);
                return learnerLog?.EventTime.UtcToTimeZoneTime();
            }

            return null;
        }
        
        private string GetExplanation(List<CertificateLogSummary> learnerCertificateLogs)
        {
            throw new System.NotImplementedException();
        }

        private string GetDateStatusTitle(string learnerCertificateStatus)
        {
            var dateTitle = "Date ";
            switch (learnerCertificateStatus)
            {
                case "Submitted":
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
            }
            return string.Empty;
        }
    }
}