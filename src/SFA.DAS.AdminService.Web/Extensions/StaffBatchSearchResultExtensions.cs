using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class StaffBatchSearchResultExtensions
    {
        public static string DisplayUln(this StaffBatchSearchResult searchResult)
        { 
            return searchResult.Uln > 0 ? searchResult.Uln.ToString() : "None";  
        }
        public static string DisplayTrainingCourse(this StaffBatchSearchResult searchResult)
        {
            if (searchResult.StandardCode > 0)
            {
                return $"{searchResult.CertificateData.StandardName}({searchResult.StandardCode})";
            }
            else
            {
                var trainingCourse = searchResult.CertificateData.FrameworkName;
                if (!string.IsNullOrEmpty(searchResult.CertificateData.TrainingCode))
                {
                    trainingCourse = trainingCourse + $"({searchResult.CertificateData.TrainingCode})";
                }
                return trainingCourse;
            }
        } 
        public static string ViewCertificateLinkAction(this StaffBatchSearchResult searchResult)
        {
            if (searchResult.StandardCode > 0)
            {
                return "LearnerDetails";
            }
            else
            {
                return "FrameworkLearnerDetails";
            }
        }
        public static Dictionary<string,string> ViewCertificateLinkRouteData(this StaffBatchSearchResult searchResult)
        { 
            
            if (searchResult.StandardCode > 0)
            {
                return new Dictionary<string, string>()
                {
                    { "stdcode", searchResult.StandardCode.ToString() },
                    { "uln", searchResult.Uln.ToString() },
                    { "searchstring", searchResult.CertificateReference },
                    { "batchNumber", searchResult.BatchNumber?.ToString() }
                };
            }
            else
            { 
                return new Dictionary<string, string>()
                {
                    { "batchNumber", searchResult.BatchNumber?.ToString() },
                    { "frameworkLearnerId", searchResult.FrameworkLearnerId?.ToString() },
                };
            } 
        }

    }
}
