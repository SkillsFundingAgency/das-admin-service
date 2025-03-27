using System.Collections.Generic;
using System;
using System.Linq;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkLearnerDetailsViewModel
    {
        public Guid Id { get; set; }
        public string ApprenticeForename { get; set; }
        public string ApprenticeSurname { get; set; }
        public DateTime ApprenticeDoB { get; set; }
        public long? ApprenticeULN { get; set; }
        public string FrameworkCertificateNumber { get; set; }
        public string FrameworkName { get; set; }
        public string PathwayName { get; set; }
        public string ApprenticeshipLevelName { get; set; }
        public List<string> Qualifications { get; set; }
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public DateTime? ApprenticeStartdate { get; set; }
        public DateTime? ApprenticeLastdateInLearning { get; set; }
        public DateTime CertificationDate { get; set; }
        public string BackAction { get; set; }
        public string QualificationsDisplay
        {
            get
            {
                if (Qualifications == null || !Qualifications.Any())
                {
                    return string.Empty;
                }

                if (Qualifications.Count == 1)
                {
                    return Qualifications.First();
                }
                else
                {
                    return $"<ul class=\"govuk-list govuk-list--bullet\"><li>{string.Join("</li><li>", Qualifications)}</li></ul>";
                }
            }
        } 
        public string CertificateStatus { get; set; }
        public DateTime? CertificateStatusDate { get; set; }
        public string CertificateReference { get; set; }
        public List<CertificateLogSummary> CertificateLogs{ get; set; }
        public bool ShowDetails { get; set; }
        public int? BatchNumber { get; set; }

        public Dictionary<string,string> CertificateHistoryButtonRouteData(bool allLogs)
        {
            var routeValues = new Dictionary<string, string>()
            {
                { "allLogs", allLogs.ToString()}
            };
            if (BatchNumber.HasValue)
            {
                routeValues.Add("frameworkLearnerId", Id.ToString());
                routeValues.Add("batchNumber", BatchNumber.ToString());
            }
            return routeValues;
        }
    }
}
