using SFA.DAS.RoatpAssessor.Application.Gateway;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway
{
    public class DashboardViewModel
    {
        public const string SortDescending = "desc";
        private const string SelectedTabCss = "govuk-tabs__tab--selected";

        public DashboardTab Tab { get; set; }
        public List<DashboardApplication> NewApplications { get; set; }
        public int NewApplicationsCount { get; set; }
        public int InProgressCount { get; set; }
        public string SortedBy { get; set; }
        public bool SortedDescending { get; set; }

        public string NewApplicationsTabCss => Tab == DashboardTab.New
            ? SelectedTabCss : "";

        public string InProgressTabCss => Tab == DashboardTab.InProgress
            ? SelectedTabCss : "";

        public string OutcomesTabCss => Tab == DashboardTab.Outcomes
            ? SelectedTabCss : "";


        public string SearchTabCss => Tab == DashboardTab.Search
            ? SelectedTabCss : "";

        public string GetRouteSort(string sortBy)
        {
            if (sortBy != SortedBy)
                return null;

            return SortedDescending ? null : SortDescending;
        }

        public string GetAriaSort(string sortBy)
        {
            if (sortBy != SortedBy)
                return null;

            return SortedDescending ? "descending" : "ascending";
        }
    }

    public class DashboardApplication
    {
        public Guid Id { get; set; }
        public string OrganisationName { get; set; }
        public string Ukprn { get; set; }
        public string ApplicationRef { get; set; }
        public string ProviderRoute { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
