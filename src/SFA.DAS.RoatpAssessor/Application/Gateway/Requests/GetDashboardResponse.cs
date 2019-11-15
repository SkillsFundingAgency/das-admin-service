using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetDashboardResponse
    {
        public DashboardTab Tab { get; set; }
        public List<Domain.DTOs.Application> NewApplications { get; set; }
        public string SortedBy { get; set; }
        public bool SortedDescending { get; set; }
        public int NewApplicationsCount { get; set; }
        public int InProgressCount { get; set; }
    }
}
