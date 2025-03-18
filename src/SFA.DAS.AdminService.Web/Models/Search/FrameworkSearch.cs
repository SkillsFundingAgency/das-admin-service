using SFA.DAS.AdminService.Web.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Models.Search
{
    public class FrameworkSearch
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<FrameworkLearnerSummaryViewModel> FrameworkResults { get; set; }
        public Guid? SelectedResult { get; set; }
    }
}
