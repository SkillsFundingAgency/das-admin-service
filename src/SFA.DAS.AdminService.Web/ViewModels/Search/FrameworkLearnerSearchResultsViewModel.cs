using System.Collections.Generic;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkLearnerSearchResultsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<FrameworkLearnerSummaryViewModel> FrameworkResults { get; set; }
        public int FrameworkResultCount => FrameworkResults?.Count ?? 0;
        public Guid SelectedResult { get; set; }
    }
}
