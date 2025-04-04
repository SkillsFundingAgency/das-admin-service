using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkLearnerDetailsViewModel : CertificateHistoryViewModel
    {
        public GetFrameworkLearnerResponse Learner { get; set; }

        public List<string> Qualifications { get; set; }

        public string QualificationsDisplay
        {
            get
            {
                if (Qualifications == null || !Qualifications.Any())
                {
                    return string.Empty;
                }
                else if (Qualifications.Count == 1)
                {
                    return Qualifications[0];
                }
                
                return $"<ul class=\"govuk-list govuk-list--bullet\"><li>{string.Join("</li><li>", Qualifications)}</li></ul>";
            }
        }

        public bool ShowDetails { get; set; }
        public int? BatchNumber { get; set; }

        public string ReasonForChange => GetReasonForChange(Learner.CertificatePrintReasonForChange);

        public Dictionary<string,string> CertificateHistoryButtonRouteData(bool allLogs)
        {
            var routeValues = new Dictionary<string, string>()
            {
                { "allLogs", allLogs.ToString()}
            };
            
            if (BatchNumber.HasValue)
            {
                routeValues.Add("frameworkLearnerId", Learner.Id.ToString());
                routeValues.Add("batchNumber", BatchNumber.ToString());
            }
            
            return routeValues;
        }
    }
}
