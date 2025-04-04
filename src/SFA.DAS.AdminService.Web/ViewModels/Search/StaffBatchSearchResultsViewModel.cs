using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class StaffBatchSearchResultViewModel
    {
        public int? BatchNumber { get; set; }
        public DateTime StatusAt { get; set; }
        public string CertificateReference { get; set; }
        public string Status { get; set; }
        public CertificateData CertificateData { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public Guid? FrameworkLearnerId { get; set; }    
        public string DisplayUln { get; set; }
        public string TrainingCourse { get; set; }
        public string ViewCertificateLinkAction { get; set; }
        public Dictionary<string, string> ViewCertificateLinkRouteData { get; set; }

        public StaffBatchSearchResultViewModel(StaffBatchSearchResult searchResult)
        {
            BatchNumber = searchResult.BatchNumber;
            StatusAt = searchResult.StatusAt;
            CertificateReference = searchResult.CertificateReference;
            Status = searchResult.Status;
            CertificateData = searchResult.CertificateData;
            Uln = searchResult.Uln;
            StandardCode = searchResult.StandardCode;
            FrameworkLearnerId = searchResult.FrameworkLearnerId;

            DisplayUln = searchResult.Uln > 0 ? searchResult.Uln.ToString() : "None";
            TrainingCourse = FormatTrainingCourse(searchResult);
            ViewCertificateLinkAction = searchResult.StandardCode > 0 ? "LearnerDetails" : "FrameworkLearnerDetails";
            ViewCertificateLinkRouteData = GenerateRouteData(searchResult);
        }

        private static string FormatTrainingCourse(StaffBatchSearchResult searchResult)
        {
            if (searchResult.StandardCode > 0)
            {
                return $"{searchResult.CertificateData?.StandardName}({searchResult.StandardCode})";
            }
            else
            {
                if (searchResult.CertificateData != null)
                { 
                    var trainingCourse = searchResult.CertificateData?.FrameworkName;
                    if (!string.IsNullOrEmpty(searchResult.CertificateData?.TrainingCode))
                    {
                        trainingCourse = trainingCourse + $"({searchResult.CertificateData.TrainingCode})";
                    }
                    return trainingCourse;
                }  
            }
                return string.Empty;
        }

        private static Dictionary<string, string> GenerateRouteData(StaffBatchSearchResult searchResult)
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
